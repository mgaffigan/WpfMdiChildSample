using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace WpfMdiClientWindow
{
    internal class MdiChildWindow : UserControl
    {
        private HwndSourceEx? Source;

        static MdiChildWindow()
        {
            BackgroundProperty.OverrideMetadata(typeof(MdiChildWindow), new FrameworkPropertyMetadata(Brushes.White));
        }

        #region Show/Close/DefWndProc

        public void Show(IntPtr mdiFrameHwnd)
        {
            Dispatcher.VerifyAccess();
            if (Source is not null)
            {
                throw new InvalidOperationException("Already shown");
            }

            var src = new HwndSourceEx(classStyle: 8, style: 0x56cc0000, exStyle: 0x00050140,
                x: 100, y: 100, width: 600, height: 400, Title ?? "", mdiFrameHwnd);
            src.AddHookLast(MdiChildHook);
            src.RootVisual = this;
            src.Disposed += (_, _) => Closed?.Invoke(this, EventArgs.Empty);
            Source = src;
        }

        public event EventHandler? Closed;

        public void Close()
        {
            Source?.Dispose();
        }

        private static nint MdiChildHook(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            // https://learn.microsoft.com/en-us/windows/win32/winmsg/using-the-multiple-document-interface#writing-the-child-window-procedure
            handled = true;
            return PInvoke.DefMDIChildProc((HWND)hwnd, (uint)msg, new WPARAM((nuint)wParam), (LPARAM)lParam);
        }

        #endregion

        #region Title

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MdiChildWindow),
                new FrameworkPropertyMetadata("", Title_Changed));

        private static void Title_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (MdiChildWindow)d;
            var source = @this.Source;
            if (source is null || source.IsDisposed) return;

            _ = PInvoke.SetWindowText((HWND)source.Handle, ((string?)e.NewValue) ?? "");
        }

        #endregion
    }
}
