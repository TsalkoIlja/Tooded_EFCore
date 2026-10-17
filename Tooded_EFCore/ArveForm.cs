
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace Tooded_EFCore
{
    public partial class ArveForm : Form
    {
        private Label lblKokku = new Label();
        private Button btnEemalda = new Button();

        private string? viimanePdf;

        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Toode? SelectedToode { get; set; }

        public ArveForm()
        {
            InitializeComponent();

            Text = "Ostukorv ja arve";

            lblKokku.AutoSize = true;
            lblKokku.Font = new Font(
                Font, FontStyle.Bold);

            lblKokku.Location = new Point(
                listOstukorv.Left,
                listOstukorv.Bottom + 12
            );

            btnEemalda.Text = "Eemalda valitud";
            btnEemalda.Size = new Size(
                Math.Max(150, listOstukorv.Width), 32);

            btnEemalda.Location = new Point(
                listOstukorv.Left,
                listOstukorv.Bottom + 42
            );

            btnEemalda.Click += btnEemalda_Click;

            Controls.Add(lblKokku);
            Controls.Add(btnEemalda);

            ClientSize = new Size(
                ClientSize.Width,
                Math.Max(ClientSize.Height, btnEemalda.Bottom + 20)
            );

            Shown += (s, e) => NaitaKorvi();
        }

        private void NaitaKorvi()
        {
            listOstukorv.Items.Clear();

            foreach (var rida in Ostukorv.Ridad)
            {
                listOstukorv.Items.Add(rida);
            }

            lblKokku.Text =
                $"Kokku: {Ostukorv.Kokku:F2} EUR";
        }

        private void btnLisaKorvi_Click(object sender, EventArgs e)
        {
            if (SelectedToode == null)
            {
                MessageBox.Show(
                    "Lisa tooted poes või vali toode Form2-s!");
                return;
            }

            if (!Ostukorv.Lisa(SelectedToode, 1))
            {
                MessageBox.Show(
                    "Laos ei ole piisavalt tooteid!");
                return;
            }

            viimanePdf = null;
            NaitaKorvi();

            MessageBox.Show("Toode lisatud ostukorvi!");
        }

        private void btnEemalda_Click(object? sender, EventArgs e)
        {
            if (listOstukorv.SelectedItem is OstukorviRida rida)
            {
                Ostukorv.Ridad.Remove(rida);

                viimanePdf = null;
                NaitaKorvi();

                MessageBox.Show("Toode eemaldatud!");
            }
            else
            {
                MessageBox.Show("Vali eemaldatav toode!");
            }
        }

        private void listBox1_SelectedIndexChanged(
            object sender, EventArgs e)
        {
        }

        private void btnGenPDF_Click(object sender, EventArgs e)
        {
            if (Ostukorv.Ridad.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi!");
                return;
            }

            try
            {
                Directory.CreateDirectory("Arved");

                string failiTee = Path.Combine(
                    "Arved",
                    $"Arve_{DateTime.Now:yyyyMMdd_HHmmss_fff}.pdf"
                );

                using PdfDocument dokument = new PdfDocument();

                dokument.Info.Title = "Arve";

                PdfPage leht = dokument.AddPage();

                using XGraphics gfx =
                    XGraphics.FromPdfPage(leht);

                XFont pealkiri = new XFont(
                    "Arial", 20, XFontStyle.Bold);

                XFont tekst = new XFont(
                    "Arial", 12, XFontStyle.Regular);

                XFont paksTekst = new XFont(
                    "Arial", 14, XFontStyle.Bold);

                gfx.DrawString(
                    "ARVE",
                    pealkiri,
                    XBrushes.Black,
                    new XPoint(50, 60)
                );

                gfx.DrawString(
                    $"Kuupäev: {DateTime.Now:dd.MM.yyyy}",
                    tekst,
                    XBrushes.Black,
                    new XPoint(50, 100)
                );

                gfx.DrawString(
                    "Ostukorvi tooted:",
                    tekst,
                    XBrushes.Black,
                    new XPoint(50, 140)
                );

                double y = 175;

                foreach (var rida in Ostukorv.Ridad)
                {
                    // Uus lehekülg, kui ruum saab otsa
                    if (y > leht.Height.Point - 90)
                    {
                        MessageBox.Show(
                            "Ostukorvis on ühe PDF-lehe jaoks liiga palju tooteid.");
                        return;
                    }

                    gfx.DrawString(
                        rida.ToString().Replace("€", "EUR"),
                        tekst,
                        XBrushes.Black,
                        new XPoint(50, y)
                    );

                    y += 25;
                }

                y += 20;

                gfx.DrawString(
                    $"KOKKU: {Ostukorv.Kokku:F2} EUR",
                    paksTekst,
                    XBrushes.Black,
                    new XPoint(50, y)
                );

                dokument.Save(failiTee);

                viimanePdf = Path.GetFullPath(failiTee);

                MessageBox.Show(
                    "PDF arve on loodud!\n" + viimanePdf
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "PDF loomisel tekkis viga:\n" + ex.Message
                );
            }
        }

        private void btnSaadaEmail_Click(object sender, EventArgs e)
        {
            string? gmail =
                Environment.GetEnvironmentVariable("GMAIL_ADDRESS");

            string? parool =
                Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

            if (string.IsNullOrWhiteSpace(gmail) ||
                string.IsNullOrWhiteSpace(parool))
            {
                MessageBox.Show("Gmaili seaded puuduvad!");
                return;
            }

            if (string.IsNullOrEmpty(viimanePdf) ||
                !File.Exists(viimanePdf))
            {
                MessageBox.Show("Genereeri kõigepealt PDF!");
                return;
            }

            string saaja =
                Microsoft.VisualBasic.Interaction.InputBox(
                    "Sisesta saaja e-mail:",
                    "Saada arve",
                    ""
                );

            if (string.IsNullOrWhiteSpace(saaja))
                return;

            if (!MailAddress.TryCreate(saaja, out _))
            {
                MessageBox.Show("Vale e-maili aadress!");
                return;
            }

            DialogResult kinnitus = MessageBox.Show(
                $"Kas soovid saata arve aadressile {saaja}?",
                "Kinnita saatmine",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (kinnitus != DialogResult.Yes)
                return;

            try
            {
                using MailMessage mail = new MailMessage();

                mail.From = new MailAddress(gmail);
                mail.To.Add(saaja);
                mail.Subject = "Arve";
                mail.Body = "Tere!\n\nArve on manuses.";

                mail.Attachments.Add(
                    new Attachment(viimanePdf));

                using SmtpClient smtp =
                    new SmtpClient("smtp.gmail.com", 587);

                smtp.Credentials = new NetworkCredential(
                    gmail,
                    parool.Replace(" ", "")
                );

                smtp.EnableSsl = true;
                smtp.Timeout = 15000;

                smtp.Send(mail);

                MessageBox.Show("Arve on saadetud!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "E-maili saatmisel tekkis viga:\n" +
                    ex.Message
                );
            }
        }
    }
}




