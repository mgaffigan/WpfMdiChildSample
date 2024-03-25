using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfMdiClientWindow
{
    public static class Main
    {
        static Main()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
        }

        [UnmanagedCallersOnly]
        public static void ShowNetcoreWpfMdiChild(IntPtr hWndMdiChild)
        {
            var window = new MainWindow();
            window.Show(hWndMdiChild);
        }
    }
}
