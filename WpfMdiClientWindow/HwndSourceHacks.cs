using System.Reflection;
using System.Windows.Interop;

namespace WpfMdiClientWindow;

internal class HwndSourceEx : HwndSource
{
    public HwndSourceEx(int classStyle, int style, int exStyle, int x, int y, int width, int height, string name, IntPtr parent)
        : base(classStyle, style, exStyle, x, y, width, height, name, parent)
    {
        // nop
    }

    public void AddHookLast(HwndSourceHook hook)
    {
        /* Implementation if added
        
        Verify.IsNotNull(hook, nameof(hook));
        CheckDisposed(true);
         
        var hookDelegate = new HwndWrapperHook(hook);
        _hwndWrapper.AddHookLast(hookDelegate);
        // Keep the delegate alive since HwndHook only takes a weak reference
        lastHooks.Add(hookDelegate);

        */

        // Implementation with Reflection:
        // Get HwndWrapper
        var a = typeof(HwndSource).GetField("_hwndWrapper", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("_hwndWrapper not found");
        var wrapper = a.GetValue(this)
            ?? throw new InvalidOperationException("_hwndWrapper null");

        // Call AddHookLast
        var miAddHookLast = wrapper.GetType().GetMethod("AddHookLast", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("AddHookLast not found");
        var tHwndWrapperHook = miAddHookLast.GetParameters().Single().ParameterType;
        var hookDelegate = Delegate.CreateDelegate(tHwndWrapperHook, hook.Method);
        miAddHookLast.Invoke(wrapper, new object[] { hookDelegate });

        // Keep the delegate alive since HwndHook only takes a weak reference
        lastHooks.Add(hookDelegate);
    }

    private List<Delegate /* HwndWrapperHook */> lastHooks = new();
}
