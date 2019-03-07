using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemedDialog.UI
{
    class AdHoc
    {
        [STAThread]
        void go()
        {
            var window = new MainWindow();
            window.ShowModal();
        }
    }
}
