using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    public partial class ArveForm : Form
    {
        private Label lblKokku = new Label();
        private Button btnEemalda = new Button();

        // Uus nupp ostu kinnitamiseks
        private Button btnKinnitaOst = new Button();

        private string? viimanePdf;

        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Toode? SelectedToode { get; set; }

        public ArveForm()
        {
            InitializeComponent();

            Text = "Tooded App - Ostukorv ja arve";

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

            // Uus nupp - kinnita ost
            btnKinnitaOst.Text = "Kinnita ost";

            btnKinnitaOst.Size = new Size(
                Math.Max(150, listOstukorv.Width), 35);

            btnKinnitaOst.Location = new Point(
                listOstukorv.Left,
                btnEemalda.Bottom + 10
            );

            btnKinnitaOst.Click += btnKinnitaOst_Click;

            Controls.Add(btnKinnitaOst);

            // Vormi kõrgus uue nupu jaoks
            ClientSize = new Size(
                ClientSize.Width,
                Math.Max(
                    ClientSize.Height,
                    btnKinnitaOst.Bottom + 20)
            );

            // Värvide kujundamine
            KujundaVarvid();

            Shown += (s, e) => NaitaKorvi();
        }

        // Vormi ja nuppude värvid
        private void KujundaVarvid()
        {
            BackColor = Color.FromArgb(245, 247, 250);

            // Ostukorvi nimekiri
            listOstukorv.BackColor = Color.White;

            listOstukorv.ForeColor =
                Color.FromArgb(40, 40, 40);

            listOstukorv.BorderStyle =
                BorderStyle.FixedSingle;

            // Kogusumma
            lblKokku.ForeColor =
                Color.FromArgb(37, 99, 235);

            // Kõik olemasolevad nupud
            foreach (Control element in Controls)
            {
                if (element is Button nupp)
                {
                    nupp.FlatStyle = FlatStyle.Flat;
                    nupp.FlatAppearance.BorderSize = 0;
                    nupp.ForeColor = Color.White;
                    nupp.Cursor = Cursors.Hand;

                    nupp.BackColor =
                        Color.FromArgb(52, 152, 219);

                    nupp.FlatAppearance.MouseOverBackColor =
                        Color.FromArgb(37, 99, 235);

                    // PDF-i saatmise nupp on roheline
                    if (nupp.Text.Contains(
                        "Saada",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        nupp.BackColor =
                            Color.FromArgb(34, 139, 94);

                        nupp.FlatAppearance.MouseOverBackColor =
                            Color.FromArgb(25, 110, 73);
                    }
                }
            }

            // Eemaldamise nupp on punane
            btnEemalda.BackColor =
                Color.FromArgb(220, 53, 69);

            btnEemalda.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(185, 35, 50);

            // Ostu kinnitamise nupp on roheline
            btnKinnitaOst.BackColor =
                Color.FromArgb(34, 139, 94);

            btnKinnitaOst.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(25, 110, 73);

            btnKinnitaOst.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);
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

        
        // 6. EDASIARENDUS
        // AUTOMAATNE LAOSEISU VÄHENDAMINE
     

        private void btnKinnitaOst_Click(object? sender, EventArgs e)
        {
            // Kontrollime, kas ostukorvis on tooteid
            if (Ostukorv.Ridad.Count == 0)
            {
                MessageBox.Show(
                    "Ostukorv on tühi!",
                    "Ostu kinnitamine");

                return;
            }

            // Küsime kasutajalt kinnitust
            DialogResult vastus = MessageBox.Show(
                $"Kas soovid ostu kinnitada?\n\n" +
                $"Kokku: {Ostukorv.Kokku:F2} EUR\n\n" +
                "Pärast kinnitamist vähendatakse toodete laoseisu.",
                "Kinnita ost",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (vastus != DialogResult.Yes)
                return;

            try
            {
                using var db = new ToodedContext();

                // Kõik muudatused tehakse ühe tehinguna
                using var tehing =
                    db.Database.BeginTransaction();

                foreach (var rida in Ostukorv.Ridad)
                {
                    int tooteId = rida.Toode.Id;
                    int ostetavKogus = rida.Kogus;

                    if (ostetavKogus <= 0)
                    {
                        tehing.Rollback();

                        MessageBox.Show(
                            "Vigane toote kogus!",
                            "Ostu viga");

                        return;
                    }

                    // Vähendame laoseisu ainult siis,
                    // kui toodet on piisavalt.
                    int muudetud = db.Toodetabel
                        .Where(t =>
                            t.Id == tooteId &&
                            t.Kogus >= ostetavKogus)
                        .ExecuteUpdate(seaded =>
                            seaded.SetProperty(
                                t => t.Kogus,
                                t => t.Kogus - ostetavKogus)
                        );

                    if (muudetud != 1)
                    {
                        // Kui ühest tootest ei piisa,
                        // tühistame kõik muudatused.
                        tehing.Rollback();

                        MessageBox.Show(
                            $"Toode \"{rida.Toode.Toodenimetus}\" " +
                            "puudub või seda ei ole laos piisavalt!\n" +
                            "Ostu ei kinnitatud.",
                            "Laoseisu viga",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        return;
                    }
                }

                // Kõik tooted on olemas -
                // salvestame muudatused andmebaasi
                tehing.Commit();

                decimal summa = Ostukorv.Kokku;

                // Tühjendame ostukorvi
                Ostukorv.Tyhjenda();

                NaitaKorvi();

                // Jätame juba loodud PDF-i alles,
                // et seda saaks e-postiga saata.
                MessageBox.Show(
                    $"Ost on edukalt kinnitatud!\n\n" +
                    $"Tasumisele kuulus: {summa:F2} EUR\n\n" +
                    "Toodete laoseis on uuendatud.",
                    "Ost kinnitatud",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ostu kinnitamisel tekkis viga:\n" +
                    ex.Message,
                    "Ostu viga",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void listBox1_SelectedIndexChanged(
            object sender, EventArgs e)
        {
        }

       
        // PDF ARVE
       

        private void btnGenPDF_Click(object sender, EventArgs e)
        {
            if (Ostukorv.Ridad.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi!");
                return;
            }

            try
            {
                Directory.CreateDirectory(Kaustad.Arved);

                string failiTee = Path.Combine(
                    Kaustad.Arved,
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

        // GMAIL
       

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








