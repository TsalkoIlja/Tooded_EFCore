using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Diagnostics;

namespace Tooded_EFCore
{
    public partial class ArveForm : Form
    {
        private readonly List<string> ostukorv = new();

        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Toode? SelectedToode { get; set; }

        public ArveForm()
        {
            InitializeComponent();
        }

        private void btnLisaKorvi_Click(object sender, EventArgs e)
        {
            if (SelectedToode == null)
            {
                MessageBox.Show("Vali toode enne ostukorvi lisamist.");
                return;
            }

            string rida =
                $"{SelectedToode.Toodenimetus} - {SelectedToode.Hind} EUR - 1 tk";

            ostukorv.Add(rida);
            listOstukorv.Items.Add(rida);

            MessageBox.Show("Toode lisatud ostukorvi!");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnGenPDF_Click(object sender, EventArgs e)
        {
            if (ostukorv.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi!");
                return;
            }

            try
            {
                Directory.CreateDirectory("Arved");

                string failiTee = Path.Combine(
                    "Arved",
                    $"Arve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                );

                using PdfDocument dokument = new PdfDocument();
                dokument.Info.Title = "Arve";

                PdfPage leht = dokument.AddPage();

                using XGraphics gfx = XGraphics.FromPdfPage(leht);

                XFont pealkiri = new XFont(
                    "Arial", 20, XFontStyle.Bold);

                XFont tekst = new XFont(
                    "Arial", 12, XFontStyle.Regular);

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

                int y = 175;

                foreach (string rida in ostukorv)
                {
                    gfx.DrawString(
                        rida,
                        tekst,
                        XBrushes.Black,
                        new XPoint(50, y)
                    );

                    y += 25;
                }

                dokument.Save(failiTee);

                MessageBox.Show(
                    "PDF arve on loodud!\n" +
                    Path.GetFullPath(failiTee)
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
            string? gmail = Environment.GetEnvironmentVariable("GMAIL_ADDRESS");
            string? parool = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

            if (string.IsNullOrWhiteSpace(gmail) ||
                string.IsNullOrWhiteSpace(parool))
            {
                MessageBox.Show("Gmaili seaded puuduvad!");
                return;
            }

            string kaust = Path.GetFullPath("Arved");

            if (!Directory.Exists(kaust))
            {
                MessageBox.Show("Genereeri kõigepealt PDF!");
                return;
            }

            string? pdfPath = Directory.GetFiles(kaust, "*.pdf")
                .OrderByDescending(File.GetLastWriteTime)
                .FirstOrDefault();

            if (pdfPath == null)
            {
                MessageBox.Show("PDF arvet ei leitud!");
                return;
            }

            string saaja = Microsoft.VisualBasic.Interaction.InputBox(
                "Sisesta saaja e-mail:",
                "Saada arve",
                ""
            );

            if (string.IsNullOrWhiteSpace(saaja))
                return;

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

                mail.Attachments.Add(new Attachment(pdfPath));

                using SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

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
                    "E-maili saatmisel tekkis viga:\n" + ex.Message
                );
            }
        }



    }
}



