using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net.Mail;

namespace Tooded_EFCore
{
    public partial class LoginForm : Form
    {
        private TextBox txtEmail = new TextBox();
        private TextBox txtParool = new TextBox();
        private Label lblVeateade = new Label();

        public LoginForm()
        {

            Text = "Logi sisse";
            ClientSize = new Size(400, 300);
            StartPosition = FormStartPosition.CenterScreen;

            Label lblEmail = new Label();
            lblEmail.Text = "E-mail:";
            lblEmail.Location = new Point(50, 35);
            lblEmail.AutoSize = true;

            txtEmail.Location = new Point(50, 60);
            txtEmail.Width = 280;

            Label lblParool = new Label();
            lblParool.Text = "Parool:";
            lblParool.Location = new Point(50, 105);
            lblParool.AutoSize = true;

            txtParool.Location = new Point(50, 130);
            txtParool.Width = 280;
            txtParool.UseSystemPasswordChar = true;

            Button btnLogin = new Button();
            btnLogin.Text = "Logi sisse";
            btnLogin.Location = new Point(50, 180);
            btnLogin.Size = new Size(280, 35);
            btnLogin.Click += btnLogin_Click;

            lblVeateade.Location = new Point(50, 230);
            lblVeateade.Size = new Size(300, 40);
            lblVeateade.ForeColor = Color.Red;

            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblParool);
            Controls.Add(txtParool);
            Controls.Add(btnLogin);
            Controls.Add(lblVeateade);

            AcceptButton = btnLogin;

            Button btnLooAdmin = new Button();
            btnLooAdmin.Text = "Loo esimene admin";
            btnLooAdmin.Location = new Point(50, 280);
            btnLooAdmin.Size = new Size(280, 35);

            btnLooAdmin.Click += btnLooAdmin_Click;

            Controls.Add(btnLooAdmin);

            ClientSize = new Size(400, 340);

            using (var db = new ToodedContext())
            {
                btnLooAdmin.Visible = !db.Kasutajad.Any();
            }

        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string parool = txtParool.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(parool))
            {
                lblVeateade.Text = "Sisesta e-mail ja parool!";
                return;
            }

            try
            {
                using var db = new ToodedContext();

                var kasutaja = db.Kasutajad
                    .FirstOrDefault(k => k.Email == email);

                if (kasutaja == null ||
                    !PasswordHelper.Verify(
                        parool, kasutaja.ParoolHash))
                {
                    lblVeateade.Text =
                        "Vale e-mail või parool!";
                    return;
                }

                AktiivneKasutaja.Roll = kasutaja.Roll;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Sisselogimisel tekkis viga:\n" + ex.Message);
            }
        }

        private void btnLooAdmin_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string parool = txtParool.Text;

            if (!MailAddress.TryCreate(email, out _) ||
                parool.Length < 8)
            {
                lblVeateade.Text =
                    "Sisesta korrektne e-mail ja vähemalt 8 märgiga parool!";
                return;
            }

            try
            {
                using var db = new ToodedContext();

                if (db.Kasutajad.Any())
                {
                    MessageBox.Show("Kasutaja on juba olemas!");
                    return;
                }

                Kasutaja admin = new Kasutaja
                {
                    Nimi = "Admin",
                    Email = email,
                    ParoolHash = PasswordHelper.Hash(parool),
                    Roll = "admin"
                };

                db.Kasutajad.Add(admin);
                db.SaveChanges();

                txtParool.Clear();

                MessageBox.Show("Admin kasutaja on loodud!");

                if (sender is Button nupp)
                    nupp.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga: " + ex.Message);
            }
        }

    }
}
