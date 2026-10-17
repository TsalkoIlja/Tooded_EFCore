using System;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Kaustad.Loo();

            // Automaatne andmebaasi loomine
            try
            {
                using (var db = new ToodedContext())
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Andmebaasi loomisel tekkis viga:\n" + ex.Message,
                    "Andmebaasi viga",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            // Sisselogimine
            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            // Peamenüü
            Application.Run(new Form1());
        }
    }
}
