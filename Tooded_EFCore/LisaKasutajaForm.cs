using System;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace Tooded_EFCore
{
    public class LisaKasutajaForm : Form
    {
        private TextBox txtEmail = new TextBox();
        private TextBox txtParool = new TextBox();

        public LisaKasutajaForm()
        {
            if (AktiivneKasutaja.Roll != "admin")
            {
                throw new UnauthorizedAccessException(
                    "Ainult admin saab kasutajaid lisada!");
            }

            Text = "Lisa kasutaja";
            ClientSize = new Size(380, 280);
            StartPosition = FormStartPosition.CenterScreen;

            Label lblEmail = new Label();
            lblEmail.Text = "E-mail:";
            lblEmail.Location = new Point(40, 30);
            lblEmail.AutoSize = true;

            txtEmail.Location = new Point(40, 60);
            txtEmail.Width = 280;

            Label lblParool = new Label();
            lblParool.Text = "Parool:";
            lblParool.Location = new Point(40, 105);
            lblParool.AutoSize = true;

            txtParool.Location = new Point(40, 135);
            txtParool.Width = 280;
            txtParool.UseSystemPasswordChar = true;

            Button btnLisa = new Button();
            btnLisa.Text = "Lisa kasutaja";
            btnLisa.Location = new Point(40, 190);
            btnLisa.Size = new Size(280, 35);
            btnLisa.Click += btnLisa_Click;

            Controls.AddRange(new Control[]
            {
                lblEmail,
                txtEmail,
                lblParool,
                txtParool,
                btnLisa
            });
        }

        private void btnLisa_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string parool = txtParool.Text;

            if (!MailAddress.TryCreate(email, out _) ||
                parool.Length < 8)
            {
                MessageBox.Show(
                    "Sisesta korrektne e-mail ja vähemalt 8 märgiga parool!");
                return;
            }

            using var db = new ToodedContext();

            if (db.Kasutajad.Any(k => k.Email == email))
            {
                MessageBox.Show("See kasutaja on juba olemas!");
                return;
            }

            Kasutaja kasutaja = new Kasutaja
            {
                Nimi = "Kasutaja",
                Email = email,
                ParoolHash = PasswordHelper.Hash(parool),
                Roll = "kasutaja"
            };

            db.Kasutajad.Add(kasutaja);
            db.SaveChanges();

            MessageBox.Show("Kasutaja on lisatud!");

            Close();
        }
    }
}
