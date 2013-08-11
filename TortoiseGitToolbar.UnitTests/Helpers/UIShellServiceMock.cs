using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;

namespace TortoiseGitToolbar.UnitTests.Helpers
{
    static class UIShellServiceMock
    {
        private static GenericMockFactory _uiShellFactory;

        internal static BaseMock GetUiShellInstance()
        {
            if (_uiShellFactory == null)
            {
                _uiShellFactory = new GenericMockFactory("UiShell", new[] { typeof(IVsUIShell), typeof(IVsUIShellOpenDocument) });
            }
            var uiShell = _uiShellFactory.GetInstance();
            return uiShell;
        }

        internal static BaseMock GetUiShellInstance0()
        {
            var uiShell = GetUiShellInstance();
            var name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "SetWaitCursor");
            uiShell.AddMethodCallback(name, SetWaitCursorCallBack);

            name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "SaveDocDataToFile");
            uiShell.AddMethodCallback(name, SaveDocDataToFileCallBack);

            name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox");
            uiShell.AddMethodCallback(name, ShowMessageBoxCallBack);
            return uiShell;
        }

        private static void SetWaitCursorCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void SaveDocDataToFileCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void ShowMessageBoxCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
            arguments.SetParameter(10, (int)System.Windows.Forms.DialogResult.Yes);
        }
    }
}