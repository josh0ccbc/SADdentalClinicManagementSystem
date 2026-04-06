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

                DialogResult result = loginForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // User logged in successfully
                    break;  // Exit loop and show main form
                }
                else if (result == DialogResult.Cancel)
                {
                    // User clicked X button or closed form
                    Application.Exit();  // ✅ EXIT APPLICATION
                    return;
                }
                // If anything else, loop continues (show login again)
            }

            // After successful login, show main form
            Application.Run(new Form1());
        }
    }
}