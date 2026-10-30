using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Tooded_EFCore
{
    public partial class Form3 : Form
    {
        private readonly ToodedContext _db;

        public Form3()
        {
            InitializeComponent();

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

        private void LaeKategooriadJaPildid()
        {
            tabPood.TabPages.Clear();

            var kategooriad = _db.Kategooriatabel.ToList();

            foreach (var kat in kategooriad)
            {
                TabPage page = new TabPage(
                    kat.Kategooria_nimetus);

                page.AutoScroll = true;

                // Kategooria taust
                page.BackColor =
                    Color.FromArgb(245, 247, 250);

                var tooted = _db.Toodetabel
                    .Where(t => t.KategooriaId == kat.Id)
                    .ToList();

                int count = 0;

                foreach (var t in tooted)
                {
                    Panel p = new Panel();
                    p.Width = 165;
                    p.Height = 270;
                    p.BorderStyle = BorderStyle.FixedSingle;
                    p.BackColor = Color.White;

                    // Toote pilt
                    PictureBox pb = new PictureBox();
                    pb.Width = 150;
                    pb.Height = 140;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
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

                    lbl.Location = new Point(5, 149);
                    lbl.Size = new Size(150, 23);

                    lbl.Font = new Font(
                        "Segoe UI", 9F, FontStyle.Bold);

                    lbl.ForeColor =
                        Color.FromArgb(40, 40, 40);

                    lbl.BackColor = Color.White;
                    lbl.AutoEllipsis = true;

                    // Toote hind eraldi real
                    Label lblHind = new Label();

                    lblHind.Text = $"{t.Hind:F2} EUR";
                    lblHind.AutoSize = false;
                    lblHind.TextAlign =
                        ContentAlignment.MiddleCenter;

                    lblHind.Location = new Point(5, 173);
                    lblHind.Size = new Size(150, 19);

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
                    Button btnLisaKorvi = new Button();

                    btnLisaKorvi.Text = "Lisa korvi";
                    btnLisaKorvi.Location =
                        new Point(5, 228);

                    btnLisaKorvi.Size =
                        new Size(150, 30);

                    btnLisaKorvi.Enabled = t.Kogus > 0;

                    btnLisaKorvi.FlatStyle =
                        FlatStyle.Flat;

                    btnLisaKorvi.FlatAppearance.BorderSize = 0;

                    btnLisaKorvi.BackColor =
                        Color.FromArgb(52, 152, 219);

                    btnLisaKorvi.ForeColor = Color.White;

                    btnLisaKorvi.Font = new Font(
                        "Segoe UI", 9F, FontStyle.Bold);

                    btnLisaKorvi.Cursor = Cursors.Hand;

                    btnLisaKorvi.FlatAppearance.MouseOverBackColor =
                        Color.FromArgb(37, 99, 235);

                    btnLisaKorvi.FlatAppearance.MouseDownBackColor =
                        Color.FromArgb(29, 78, 216);

                    // Ostukorvi lisamine
                    btnLisaKorvi.Click += (s, e) =>
                    {
                        int kogus = (int)numKogus.Value;

                        if (!Ostukorv.Lisa(t, kogus))
                        {
                            MessageBox.Show(
                                "Laos ei ole piisavalt tooteid!");
                            return;
                        }

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

                    // Toodete asukohad jäävad samaks
                    int x = 10 + (count % 4) * 180;
                    int y = 10 + (count / 4) * 280;

                    p.Location = new Point(x, y);
                    page.Controls.Add(p);

                    count++;
                }

                tabPood.TabPages.Add(page);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _db.Dispose();
            base.OnFormClosed(e);
        }
    }
}
