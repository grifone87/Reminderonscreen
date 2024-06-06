using System;
using System.Windows.Forms;

namespace ImageOverlayApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                LogException((Exception)args.ExceptionObject);
            };

            Application.ThreadException += (sender, args) =>
            {
                LogException(args.Exception);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void LogException(Exception ex)
        {
            MessageBox.Show($"Unhandled exception: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
