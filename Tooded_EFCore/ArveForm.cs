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

                DateTime aeg = DateTime.Now;

                string arveNumber =
                    $"ARVE-{aeg:yyyyMMdd-HHmmssfff}";

                string failiTee = Path.Combine(
                    Kaustad.Arved,
                    $"{arveNumber}.pdf"
                );

                using PdfDocument dokument = new PdfDocument();

                dokument.Info.Title =
                    $"Tooded App - {arveNumber}";

                PdfPage leht = dokument.AddPage();

                using XGraphics gfx =
                    XGraphics.FromPdfPage(leht);

                // Fondid
                XFont logo = new XFont(
                    "Arial", 22, XFontStyle.Bold);

                XFont pealkiri = new XFont(
                    "Arial", 19, XFontStyle.Bold);

                XFont tekst = new XFont(
                    "Arial", 10, XFontStyle.Regular);

                XFont vaiksemTekst = new XFont(
                    "Arial", 9, XFontStyle.Regular);

                XFont paksTekst = new XFont(
                    "Arial", 11, XFontStyle.Bold);

                XFont tabeliPealkiri = new XFont(
                    "Arial", 10, XFontStyle.Bold);

                XFont kokkuFont = new XFont(
                    "Arial", 17, XFontStyle.Bold);

                // Värvid
                XColor sinineVarv =
                    XColor.FromArgb(37, 99, 235);

                XBrush sinine =
                    new XSolidBrush(sinineVarv);

                XBrush heleSinine =
                    new XSolidBrush(
                        XColor.FromArgb(237, 245, 255));

                XBrush heleHall =
                    new XSolidBrush(
                        XColor.FromArgb(247, 249, 252));

                XPen sinineJoon =
                    new XPen(sinineVarv, 2);

                XPen hallJoon =
                    new XPen(
                        XColor.FromArgb(225, 230, 238), 0.7);

                double vasak = 48;
                double parem = leht.Width.Point - 48;
                double laius = parem - vasak;

                // Teksti joondamine paremale
                void KirjutaParemal(
                    string sisu,
                    XFont font,
                    XBrush varv,
                    double x,
                    double y)
                {
                    double tekstiLaius =
                        gfx.MeasureString(sisu, font).Width;

                    gfx.DrawString(
                        sisu,
                        font,
                        varv,
                        new XPoint(x - tekstiLaius, y)
                    );
                }

                // Pika tootenime mahutamine tabelisse
                string MahutaNimi(string nimi)
                {
                    nimi = nimi.Replace("\r", " ")
                               .Replace("\n", " ");

                    if (gfx.MeasureString(nimi, tekst).Width <= 218)
                        return nimi;

                    string tulemus = nimi;

                    while (tulemus.Length > 0 &&
                           gfx.MeasureString(
                               tulemus + "...", tekst).Width > 218)
                    {
                        tulemus =
                            tulemus.Substring(0, tulemus.Length - 1);
                    }

                    return tulemus + "...";
                }

                // =====================================
                // ARVE PÄIS
                // =====================================

                gfx.DrawString(
                    "TOODED APP",
                    logo,
                    XBrushes.Black,
                    new XPoint(vasak, 60)
                );

                KirjutaParemal(
                    "ARVE",
                    pealkiri,
                    sinine,
                    parem,
                    60
                );

                gfx.DrawLine(
                    sinineJoon,
                    vasak, 82,
                    parem, 82
                );

                // Arve andmed
                gfx.DrawString(
                    $"Arve nr: {arveNumber}",
                    tekst,
                    XBrushes.Black,
                    new XPoint(vasak, 110)
                );

                KirjutaParemal(
                    $"Kuupäev: {aeg:dd.MM.yyyy HH:mm}",
                    tekst,
                    XBrushes.Black,
                    parem,
                    110
                );

                gfx.DrawString(
                    "Kauplus: Tooded App",
                    tekst,
                    XBrushes.Black,
                    new XPoint(vasak, 132)
                );

                // =====================================
                // TOODETE TABEL
                // =====================================

                gfx.DrawString(
                    "OSTUKORVI TOOTED",
                    paksTekst,
                    XBrushes.Black,
                    new XPoint(vasak, 165)
                );

                double tabeliY = 180;
                double reaKorgus = 32;

                // Veergude parempoolsed piirid
                double kogusX = vasak + 290;
                double hindX = vasak + 385;
                double summaX = parem - 12;

                // Sinine tabeli päis
                gfx.DrawRectangle(
                    sinine,
                    vasak,
                    tabeliY,
                    laius,
                    32
                );

                gfx.DrawString(
                    "Toode",
                    tabeliPealkiri,
                    XBrushes.White,
                    new XPoint(vasak + 12, tabeliY + 21)
                );

                KirjutaParemal(
                    "Kogus",
                    tabeliPealkiri,
                    XBrushes.White,
                    kogusX,
                    tabeliY + 21
                );

                KirjutaParemal(
                    "Hind",
                    tabeliPealkiri,
                    XBrushes.White,
                    hindX,
                    tabeliY + 21
                );

                KirjutaParemal(
                    "Summa",
                    tabeliPealkiri,
                    XBrushes.White,
                    summaX,
                    tabeliY + 21
                );

                double y = tabeliY + 32;
                int reaNumber = 0;

                foreach (var rida in Ostukorv.Ridad)
                {
                    // Kontrollime, et kõik mahuksid lehele
                    if (y + reaKorgus >
                        leht.Height.Point - 145)
                    {
                        MessageBox.Show(
                            "Ostukorvis on ühe PDF-lehe jaoks " +
                            "liiga palju tooteid.");

                        return;
                    }

                    // Iga teine rida helehall
                    if (reaNumber % 2 == 1)
                    {
                        gfx.DrawRectangle(
                            heleHall,
                            vasak,
                            y,
                            laius,
                            reaKorgus
                        );
                    }

                    string tooteNimi = MahutaNimi(
                        rida.Toode.Toodenimetus ?? ""
                    );

                    // Toote nimetus
                    gfx.DrawString(
                        tooteNimi,
                        tekst,
                        XBrushes.Black,
                        new XPoint(vasak + 12, y + 21)
                    );

                    // Kogus
                    KirjutaParemal(
                        rida.Kogus.ToString(),
                        tekst,
                        XBrushes.Black,
                        kogusX,
                        y + 21
                    );

                    // Ühe toote hind
                    KirjutaParemal(
                        $"{rida.Toode.Hind:F2} EUR",
                        tekst,
                        XBrushes.Black,
                        hindX,
                        y + 21
                    );

                    // Rea summa
                    KirjutaParemal(
                        $"{rida.Summa:F2} EUR",
                        tekst,
                        XBrushes.Black,
                        summaX,
                        y + 21
                    );

                    // Eraldusjoon
                    gfx.DrawLine(
                        hallJoon,
                        vasak,
                        y + reaKorgus,
                        parem,
                        y + reaKorgus
                    );

                    y += reaKorgus;
                    reaNumber++;
                }

                // =====================================
                // KOGUSUMMA
                // =====================================

                double kokkuY = y + 18;

                gfx.DrawRectangle(
                    heleSinine,
                    vasak,
                    kokkuY,
                    laius,
                    56
                );

                gfx.DrawString(
                    "KOKKU:",
                    paksTekst,
                    XBrushes.Black,
                    new XPoint(vasak + 12, kokkuY + 35)
                );

                KirjutaParemal(
                    $"{Ostukorv.Kokku:F2} EUR",
                    kokkuFont,
                    sinine,
                    parem - 12,
                    kokkuY + 37
                );

                // =====================================
                // ARVE JALUS
                // =====================================

                double jalusY = leht.Height.Point - 74;

                gfx.DrawLine(
                    hallJoon,
                    vasak,
                    jalusY,
                    parem,
                    jalusY
                );

                string tanutekst = "Täname ostu eest!";

                double tanutekstiLaius =
                    gfx.MeasureString(
                        tanutekst, vaiksemTekst).Width;

                gfx.DrawString(
                    tanutekst,
                    vaiksemTekst,
                    XBrushes.Gray,
                    new XPoint(
                        (leht.Width.Point - tanutekstiLaius) / 2,
                        jalusY + 25)
                );

                // =====================================
                // PDF SALVESTAMINE
                // =====================================

                dokument.Save(failiTee);

                viimanePdf = Path.GetFullPath(failiTee);

                MessageBox.Show(
                    "PDF arve on loodud!\n" + viimanePdf
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "PDF loomisel tekkis viga:\n" +
                    ex.Message
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

                // Arve number PDF-faili nimest
                string arveNumber =
                    Path.GetFileNameWithoutExtension(viimanePdf);

                // Viisakas e-kirja pealkiri
                mail.Subject = $"Tooded App - {arveNumber}";

                // Viisakas e-kirja sisu
                mail.Body =
                    "Tere!\n\n" +
                    "Täname Teid, et kasutate Tooded Appi!\n\n" +
                    "Saadame Teile arve PDF-failina. " +
                    "Arve leiate selle kirja manusest.\n\n" +
                    "Lugupidamisega\n" +
                    "Tooded App meeskond";

                // Eesti tähtede korrektne kuvamine
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;


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








