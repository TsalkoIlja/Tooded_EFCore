using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Tooded_EFCore
{
    public partial class Form3 : Form
    {
        private readonly ToodedContext _db;

        // Ostukorvi loendurid kõikidel vahelehtedel
        private readonly List<Label> _korviLoendurid = new();

        public Form3()
        {
            InitializeComponent();

            // Suurem poe aken
            ClientSize = new Size(1100, 760);
            MinimumSize = new Size(900, 650);
            StartPosition = FormStartPosition.CenterScreen;

            tabPood.Dock = DockStyle.Fill;

            // Vormi kujundus
            KujundaVarvid();

            _db = new ToodedContext();
            LaeKategooriadJaPildid();
        }

        // Vormi ja kategooriate värvid
        private void KujundaVarvid()
        {
            Text = "Tooded App - Pood";

            BackColor = Color.FromArgb(245, 247, 250);

            tabPood.BackColor =
                Color.FromArgb(245, 247, 250);

            tabPood.ForeColor =
                Color.FromArgb(40, 40, 40);
        }

        // Ostukorvi toodete koguarv
        private void UuendaKorviLoendurid()
        {
            int kogus = Ostukorv.Ridad.Sum(r => r.Kogus);

            foreach (Label lbl in _korviLoendurid)
            {
                if (!lbl.IsDisposed)
                {
                    lbl.Text = $"Ostukorv: {kogus} tk";
                }
            }
        }

        // Ostukorvi tühjendamine
        private void TyhjendaOstukorv()
        {
            if (Ostukorv.Ridad.Count == 0)
            {
                MessageBox.Show(
                    "Ostukorv on juba tühi!",
                    "Ostukorv");
                return;
            }

            DialogResult vastus = MessageBox.Show(
                "Kas soovid kogu ostukorvi tühjendada?",
                "Ostukorvi tühjendamine",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (vastus != DialogResult.Yes)
                return;

            Ostukorv.Tyhjenda();

            UuendaKorviLoendurid();

            MessageBox.Show(
                "Ostukorv on tühjendatud!",
                "Ostukorv");
        }

        private void LaeKategooriadJaPildid()
        {
            tabPood.TabPages.Clear();
            _korviLoendurid.Clear();

            var kategooriad =
                _db.Kategooriatabel.ToList();

            foreach (var kat in kategooriad)
            {
                TabPage page = new TabPage(
                    kat.Kategooria_nimetus);

                page.BackColor =
                    Color.FromArgb(245, 247, 250);

                // Paigutus: ülemine menüü ja tootekaardid
                TableLayoutPanel paigutus =
                    new TableLayoutPanel();

                paigutus.Dock = DockStyle.Fill;
                paigutus.ColumnCount = 1;
                paigutus.RowCount = 2;

                paigutus.Margin = Padding.Empty;
                paigutus.Padding = Padding.Empty;

                paigutus.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 115));

                paigutus.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 100));

                // Ülemine tööriistariba
                Panel yleminePaneel = new Panel();

                yleminePaneel.Dock = DockStyle.Fill;
                yleminePaneel.Margin = Padding.Empty;
                yleminePaneel.BackColor = Color.White;

                
                // 1. SORTEERI HINNA JÄRGI
               

                Label lblSorteeri = new Label();

                lblSorteeri.Text = "Sorteeri hinna järgi:";
                lblSorteeri.Location = new Point(10, 8);
                lblSorteeri.Size = new Size(200, 22);

                lblSorteeri.Font = new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

                lblSorteeri.ForeColor =
                    Color.FromArgb(40, 40, 40);

                ComboBox comboSorteeri =
                    new ComboBox();

                comboSorteeri.Location =
                    new Point(10, 34);

                comboSorteeri.Size =
                    new Size(220, 30);

                comboSorteeri.DropDownStyle =
                    ComboBoxStyle.DropDownList;

                comboSorteeri.FlatStyle =
                    FlatStyle.Flat;

                comboSorteeri.Font =
                    new Font("Segoe UI", 9F);

                comboSorteeri.BackColor = Color.White;

                comboSorteeri.Items.Add("Vaikimisi");
                comboSorteeri.Items.Add(
                    "Hind: odavam enne");

                comboSorteeri.Items.Add(
                    "Hind: kallim enne");

                comboSorteeri.SelectedIndex = 0;

                // 2. OTSI TOODET
                

                Label lblOtsi = new Label();

                lblOtsi.Text = "Otsi toodet:";
                lblOtsi.Location = new Point(260, 8);
                lblOtsi.Size = new Size(200, 22);

                lblOtsi.Font = new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

                lblOtsi.ForeColor =
                    Color.FromArgb(40, 40, 40);

                TextBox txtOtsi = new TextBox();

                txtOtsi.Location =
                    new Point(260, 34);

                txtOtsi.Size =
                    new Size(220, 30);

                txtOtsi.Font =
                    new Font("Segoe UI", 9F);

                txtOtsi.PlaceholderText =
                    "Sisesta toote nimi...";

                txtOtsi.BackColor = Color.White;

                txtOtsi.BorderStyle =
                    BorderStyle.FixedSingle;

                
                // 3. AINULT LAOS OLEVAD TOOTED
                

                CheckBox chkLaos = new CheckBox();

                chkLaos.Text =
                    "Näita ainult laos olevaid tooteid";

                chkLaos.Location =
                    new Point(520, 34);

                chkLaos.Size =
                    new Size(290, 30);

                chkLaos.Font =
                    new Font("Segoe UI", 9F);

                chkLaos.ForeColor =
                    Color.FromArgb(40, 40, 40);

                chkLaos.BackColor = Color.White;

                
                // 4. OSTUKORVI LOENDUR
               

                Label lblOstukorv = new Label();

                lblOstukorv.Location =
                    new Point(10, 78);

                lblOstukorv.Size =
                    new Size(180, 27);

                lblOstukorv.Font = new Font(
                    "Segoe UI", 11F, FontStyle.Bold);

                lblOstukorv.ForeColor =
                    Color.FromArgb(37, 99, 235);

                lblOstukorv.BackColor = Color.White;

                _korviLoendurid.Add(lblOstukorv);

                
                // 5. TÜHJENDA OSTUKORV
                

                Button btnTyhjenda = new Button();

                btnTyhjenda.Text =
                    "Tühjenda ostukorv";

                btnTyhjenda.Location =
                    new Point(210, 74);

                btnTyhjenda.Size =
                    new Size(180, 32);

                btnTyhjenda.FlatStyle =
                    FlatStyle.Flat;

                btnTyhjenda.FlatAppearance.BorderSize = 0;

                btnTyhjenda.BackColor =
                    Color.FromArgb(220, 53, 69);

                btnTyhjenda.ForeColor = Color.White;

                btnTyhjenda.Font = new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

                btnTyhjenda.Cursor = Cursors.Hand;

                btnTyhjenda.FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(190, 40, 55);

                btnTyhjenda.Click += (s, e) =>
                {
                    TyhjendaOstukorv();
                };

                // Ülemise paneeli elemendid
                yleminePaneel.Controls.Add(lblSorteeri);
                yleminePaneel.Controls.Add(comboSorteeri);
                yleminePaneel.Controls.Add(lblOtsi);
                yleminePaneel.Controls.Add(txtOtsi);
                yleminePaneel.Controls.Add(chkLaos);
                yleminePaneel.Controls.Add(lblOstukorv);
                yleminePaneel.Controls.Add(btnTyhjenda);

                // Tootekaartide ala
                Panel kaartidePaneel = new Panel();

                kaartidePaneel.Dock = DockStyle.Fill;
                kaartidePaneel.Margin = Padding.Empty;
                kaartidePaneel.AutoScroll = true;

                kaartidePaneel.BackColor =
                    Color.FromArgb(245, 247, 250);

                paigutus.Controls.Add(
                    yleminePaneel, 0, 0);

                paigutus.Controls.Add(
                    kaartidePaneel, 0, 1);

                page.Controls.Add(paigutus);

                // Andmebaasist toodete laadimine
                var tooted = _db.Toodetabel
                    .Where(t => t.KategooriaId == kat.Id)
                    .ToList();

                // Kaardid, hinnad, nimed ja laoseis
                var kaardid =
                    new List<(
                        Panel Kaart,
                        double Hind,
                        string Nimi,
                        int Laos)>();

                int count = 0;

                foreach (var t in tooted)
                {
                    Panel p = new Panel();

                    p.Width = 165;
                    p.Height = 270;

                    p.BorderStyle =
                        BorderStyle.FixedSingle;

                    p.BackColor = Color.White;

                    // Toote pilt
                    PictureBox pb = new PictureBox();

                    pb.Width = 150;
                    pb.Height = 140;

                    pb.SizeMode =
                        PictureBoxSizeMode.Zoom;

                    pb.Location = new Point(5, 5);
                    pb.BackColor = Color.White;
                    pb.Cursor = Cursors.Hand;

                    string piltPath = Path.Combine(
                        Kaustad.Pildid,
                        t.Pilt ?? "puudub.png");

                    if (File.Exists(piltPath))
                    {
                        using var pilt =
                            Image.FromFile(piltPath);

                        pb.Image = new Bitmap(pilt);
                    }

                    // Toote nimetus
                    Label lbl = new Label();

                    lbl.Text = t.Toodenimetus;
                    lbl.AutoSize = false;

                    lbl.TextAlign =
                        ContentAlignment.MiddleCenter;

                    lbl.Location =
                        new Point(5, 149);

                    lbl.Size =
                        new Size(150, 23);

                    lbl.Font = new Font(
                        "Segoe UI", 9F, FontStyle.Bold);

                    lbl.ForeColor =
                        Color.FromArgb(40, 40, 40);

                    lbl.BackColor = Color.White;
                    lbl.AutoEllipsis = true;

                    // Toote hind
                    Label lblHind = new Label();

                    lblHind.Text =
                        $"{t.Hind:F2} EUR";

                    lblHind.AutoSize = false;

                    lblHind.TextAlign =
                        ContentAlignment.MiddleCenter;

                    lblHind.Location =
                        new Point(5, 173);

                    lblHind.Size =
                        new Size(150, 19);

                    lblHind.Font = new Font(
                        "Segoe UI", 9F, FontStyle.Bold);

                    lblHind.ForeColor =
                        Color.FromArgb(52, 152, 219);

                    lblHind.BackColor = Color.White;

                    // Toote kogus
                    NumericUpDown numKogus =
                        new NumericUpDown();

                    numKogus.Location =
                        new Point(5, 195);

                    numKogus.Width = 150;
                    numKogus.Minimum = 1;

                    numKogus.Maximum =
                        Math.Max(1, t.Kogus);

                    numKogus.Value = 1;
                    numKogus.Enabled = t.Kogus > 0;

                    numKogus.BackColor = Color.White;

                    numKogus.ForeColor =
                        Color.FromArgb(40, 40, 40);

                    // Lisa korvi nupp
                    Button btnLisaKorvi =
                        new Button();

                    btnLisaKorvi.Text = "Lisa korvi";

                    btnLisaKorvi.Location =
                        new Point(5, 228);

                    btnLisaKorvi.Size =
                        new Size(150, 30);

                    btnLisaKorvi.Enabled =
                        t.Kogus > 0;

                    btnLisaKorvi.FlatStyle =
                        FlatStyle.Flat;

                    btnLisaKorvi.FlatAppearance.BorderSize = 0;

                    btnLisaKorvi.BackColor =
                        Color.FromArgb(52, 152, 219);

                    btnLisaKorvi.ForeColor =
                        Color.White;

                    btnLisaKorvi.Font = new Font(
                        "Segoe UI", 9F, FontStyle.Bold);

                    btnLisaKorvi.Cursor =
                        Cursors.Hand;

                    btnLisaKorvi.FlatAppearance.MouseOverBackColor =
                        Color.FromArgb(37, 99, 235);

                    btnLisaKorvi.FlatAppearance.MouseDownBackColor =
                        Color.FromArgb(29, 78, 216);

                    // Ostukorvi lisamine
                    btnLisaKorvi.Click += (s, e) =>
                    {
                        int kogus =
                            (int)numKogus.Value;

                        if (!Ostukorv.Lisa(t, kogus))
                        {
                            MessageBox.Show(
                                "Laos ei ole piisavalt tooteid!");

                            return;
                        }

                        // Uuendame ostukorvi loendurit
                        UuendaKorviLoendurid();

                        MessageBox.Show(
                            $"{t.Toodenimetus} - {kogus} tk lisatud ostukorvi!",
                            "Ostukorv");
                    };

                    // Toote info
                    pb.Click += (s, e) =>
                    {
                        MessageBox.Show(
                            $"Toode: {t.Toodenimetus}\n" +
                            $"Hind: {t.Hind:F2} EUR\n" +
                            $"Laos: {t.Kogus} tk",
                            "Toote info");
                    };

                    // Elemendid tootekaardile
                    p.Controls.Add(pb);
                    p.Controls.Add(lbl);
                    p.Controls.Add(lblHind);
                    p.Controls.Add(numKogus);
                    p.Controls.Add(btnLisaKorvi);

                    // Algne paigutus: 4 kaarti reas
                    int x =
                        10 + (count % 4) * 180;

                    int y =
                        10 + (count / 4) * 280;

                    p.Location = new Point(x, y);

                    kaartidePaneel.Controls.Add(p);

                    kaardid.Add((
                        p,
                        Convert.ToDouble(t.Hind),
                        t.Toodenimetus ?? "",
                        t.Kogus
                    ));

                    count++;
                }

               
                // OTSINGU, SORTEERIMISE JA
                // LAOSEISU ÜHINE FUNKTSIOON
                

                void UuendaKaardid()
                {
                    string otsing =
                        txtOtsi.Text.Trim();

                    // Otsing ja laoseisu filter
                    var sobivadKaardid = kaardid
                        .Where(k =>
                            (string.IsNullOrEmpty(otsing) ||
                             k.Nimi.Contains(
                                 otsing,
                                 StringComparison.OrdinalIgnoreCase))
                            &&
                            (!chkLaos.Checked ||
                             k.Laos > 0))
                        .ToList();

                    // Sorteerimine hinna järgi
                    var sorteeritudKaardid =
                        comboSorteeri.SelectedIndex switch
                        {
                            1 => sobivadKaardid
                                .OrderBy(k => k.Hind)
                                .ToList(),

                            2 => sobivadKaardid
                                .OrderByDescending(k => k.Hind)
                                .ToList(),

                            _ => sobivadKaardid
                        };

                    kaartidePaneel.SuspendLayout();

                    kaartidePaneel.AutoScrollPosition =
                        new Point(0, 0);

                    // Peidame kõik kaardid
                    foreach (var kaart in kaardid)
                    {
                        kaart.Kaart.Visible = false;
                    }

                    // Paigutame sobivad kaardid
                    int index = 0;

                    foreach (var kaart in sorteeritudKaardid)
                    {
                        int x =
                            10 + (index % 4) * 180;

                        int y =
                            10 + (index / 4) * 280;

                        kaart.Kaart.Location =
                            new Point(x, y);

                        kaart.Kaart.Visible = true;

                        index++;
                    }

                    // Kerimisala suurus
                    int ridadeArv =
                        (index + 3) / 4;

                    kaartidePaneel.AutoScrollMinSize =
                        new Size(
                            0,
                            10 + ridadeArv * 280);

                    kaartidePaneel.ResumeLayout();
                }

                // Sorteerimise muutmine
                comboSorteeri.SelectedIndexChanged +=
                    (s, e) =>
                    {
                        UuendaKaardid();
                    };

                // Otsing teksti sisestamisel
                txtOtsi.TextChanged += (s, e) =>
                {
                    UuendaKaardid();
                };

                // Ainult laos olevad tooted
                chkLaos.CheckedChanged += (s, e) =>
                {
                    UuendaKaardid();
                };

                // Esialgne paigutus
                UuendaKaardid();

                tabPood.TabPages.Add(page);
            }

            UuendaKorviLoendurid();
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            _db.Dispose();

            base.OnFormClosed(e);
        }
    }
}
