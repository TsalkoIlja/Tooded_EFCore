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
            _db = new ToodedContext();
            LaeKategooriadJaPildid();
        }

        private void LaeKategooriadJaPildid()
        {
            tabPood.TabPages.Clear();

            var kategooriad = _db.Kategooriatabel.ToList();

            foreach (var kat in kategooriad)
            {
                TabPage page = new TabPage(kat.Kategooria_nimetus);
                page.AutoScroll = true;

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

                    PictureBox pb = new PictureBox();
                    pb.Width = 150;
                    pb.Height = 140;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Location = new Point(5, 5);

                    string piltPath = Path.Combine(
                        Kaustad.Pildid,
                        t.Pilt ?? "puudub.png");

                    if (File.Exists(piltPath))
                    {
                        using var pilt = Image.FromFile(piltPath);
                        pb.Image = new Bitmap(pilt);
                    }

                    Label lbl = new Label();
                    lbl.Text = $"{t.Toodenimetus}\n{t.Hind:F2} EUR";
                    lbl.AutoSize = false;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Location = new Point(5, 150);
                    lbl.Width = 150;
                    lbl.Height = 40;

                    NumericUpDown numKogus = new NumericUpDown();
                    numKogus.Location = new Point(5, 195);
                    numKogus.Width = 150;
                    numKogus.Minimum = 1;
                    numKogus.Maximum = Math.Max(1, t.Kogus);
                    numKogus.Value = 1;
                    numKogus.Enabled = t.Kogus > 0;

                    Button btnLisaKorvi = new Button();
                    btnLisaKorvi.Text = "Lisa korvi";
                    btnLisaKorvi.Location = new Point(5, 228);
                    btnLisaKorvi.Size = new Size(150, 30);
                    btnLisaKorvi.Enabled = t.Kogus > 0;

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

                    pb.Click += (s, e) =>
                    {
                        MessageBox.Show(
                            $"Toode: {t.Toodenimetus}\n" +
                            $"Hind: {t.Hind:F2} EUR\n" +
                            $"Laos: {t.Kogus} tk",
                            "Toote info");
                    };

                    p.Controls.Add(pb);
                    p.Controls.Add(lbl);
                    p.Controls.Add(numKogus);
                    p.Controls.Add(btnLisaKorvi);

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