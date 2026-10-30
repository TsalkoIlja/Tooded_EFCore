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
            UiHelper.StyleForm(this);

            Text = "Tooded App - Sisselogimine";
            ClientSize = new Size(440, 420);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Pealkiri
            Label lblPealkiri = new Label();
            lblPealkiri.Text = "TERE TULEMAST!";
            lblPealkiri.Font = new Font(
                "Segoe UI", 21, FontStyle.Bold);
            lblPealkiri.ForeColor = UiHelper.MainColor;
            lblPealkiri.TextAlign =
                ContentAlignment.MiddleCenter;
            lblPealkiri.SetBounds(30, 30, 380, 50);

            // Alapealkiri
            Label lblInfo = new Label();
            lblInfo.Text = "Logi sisse oma kontoga";
            lblInfo.Font = new Font("Segoe UI", 11);
            lblInfo.ForeColor = UiHelper.TextColor;
            lblInfo.TextAlign =
                ContentAlignment.MiddleCenter;
            lblInfo.SetBounds(30, 80, 380, 30);

            // Valge sisselogimise paneel
            Panel paneel = new Panel();
            paneel.BackColor = Color.White;
            paneel.BorderStyle = BorderStyle.FixedSingle;
            paneel.SetBounds(40, 125, 360, 260);

            // E-maili silt
            Label lblEmail = new Label();
            lblEmail.Text = "E-mail";
            lblEmail.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            lblEmail.ForeColor = UiHelper.TextColor;
            lblEmail.SetBounds(22, 15, 316, 25);

            // E-maili väli
            UiHelper.StyleTextBox(txtEmail);
            txtEmail.SetBounds(22, 43, 316, 30);
            txtEmail.PlaceholderText =
                "Sisesta oma e-mail";

            // Parooli silt
            Label lblParool = new Label();
            lblParool.Text = "Parool";
            lblParool.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            lblParool.ForeColor = UiHelper.TextColor;
            lblParool.SetBounds(22, 91, 316, 25);

            // Parooli väli
            UiHelper.StyleTextBox(txtParool);
            txtParool.SetBounds(22, 119, 316, 30);
            txtParool.PlaceholderText =
                "Sisesta oma parool";
            txtParool.UseSystemPasswordChar = true;

            // Sisselogimise nupp
            Button btnLogin = new Button();
            btnLogin.Text = "Logi sisse";
            UiHelper.StyleButton(btnLogin);
            btnLogin.SetBounds(22, 172, 316, 45);
            btnLogin.Click += btnLogin_Click;

            // Veateade
            lblVeateade.SetBounds(22, 224, 316, 32);
            lblVeateade.Font = new Font(
                "Segoe UI", 9, FontStyle.Regular);
            lblVeateade.ForeColor =
                Color.FromArgb(190, 45, 55);
            lblVeateade.TextAlign =
                ContentAlignment.MiddleLeft;

            paneel.Controls.Add(lblEmail);
            paneel.Controls.Add(txtEmail);
            paneel.Controls.Add(lblParool);
            paneel.Controls.Add(txtParool);
            paneel.Controls.Add(btnLogin);
            paneel.Controls.Add(lblVeateade);

            Controls.Add(lblPealkiri);
            Controls.Add(lblInfo);
            Controls.Add(paneel);

            AcceptButton = btnLogin;

            // Esimese administraatori loomine
            Button btnLooAdmin = new Button();
            btnLooAdmin.Text = "Loo esimene admin";
            UiHelper.StyleButton(btnLooAdmin);
            btnLooAdmin.SetBounds(62, 395, 316, 42);

            btnLooAdmin.BackColor = Color.White;
            btnLooAdmin.ForeColor = UiHelper.MainColor;
            btnLooAdmin.FlatAppearance.BorderSize = 1;
            btnLooAdmin.FlatAppearance.BorderColor =
                UiHelper.MainColor;

            btnLooAdmin.Click += btnLooAdmin_Click;

            using (var db = new ToodedContext())
            {
                btnLooAdmin.Visible = !db.Kasutajad.Any();
            }

            Controls.Add(btnLooAdmin);

            ClientSize = btnLooAdmin.Visible
                ? new Size(440, 460)
                : new Size(440, 420);
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string parool = txtParool.Text;

            lblVeateade.Text = "";

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(parool))
            {
                lblVeateade.Text =
                    "Sisesta e-mail ja parool!";
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
                    "Sisselogimisel tekkis viga:\n" +
                    ex.Message);
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
                    MessageBox.Show(
                        "Kasutaja on juba olemas!");
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

                MessageBox.Show(
                    "Admin kasutaja on loodud!");

                if (sender is Button nupp)
                {
                    nupp.Visible = false;
                    ClientSize = new Size(440, 420);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Viga: " + ex.Message);
            }
        }
    }
}
