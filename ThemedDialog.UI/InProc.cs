using System;

namespace ThemedDialog.UI
{
    class InProc
    {
        [STAThread]
        void ShowModal()
        {
            var window = new MainWindow();
            window.ShowModal();
        }
    }
}
