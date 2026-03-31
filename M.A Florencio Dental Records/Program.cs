using System;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ✅ KEEP SHOWING LOGIN FORM UNTIL USER SUCCESSFULLY LOGS IN
            while (true)
            {
                LoginForm loginForm = new LoginForm();

                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // User logged in successfully
                    // Exit the loop and let application continue
                    break;
                }
                // If ShowDialog returns anything else, loop continues (show login again)
            }

            // After successful login, show main form
            Application.Run(new Form1());
        }
    }
}