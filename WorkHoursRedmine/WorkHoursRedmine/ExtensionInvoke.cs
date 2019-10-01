using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{
    static class ExtensionInvoke
    {
        public static void InvokeIfNeeded(this Control control, Action method)
        {
            if (control.InvokeRequired)
                control.Invoke(method);
            else
                method();
        }
    }
}
