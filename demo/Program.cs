using System;
using System.Windows.Forms;

namespace ScanBridge.Demo
{
    /// <summary>نقطة دخول تطبيق العرض التجريبي.</summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
