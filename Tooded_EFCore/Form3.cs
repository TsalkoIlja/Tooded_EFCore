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

                int x = 10;
                int y = 10;
                int count = 0;

                foreach (var t in tooted)
                {
                    Panel p = new Panel();
                    p.Width = 150;
                    p.Height = 200;
                    p.BorderStyle = BorderStyle.FixedSingle;

                    PictureBox pb = new PictureBox();
                    pb.Width = 140;
                    pb.Height = 140;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Location = new Point(5, 5);

                    string piltPath = Path.Combine(
                        "Images", t.Pilt ?? "puudub.png");

                    if (File.Exists(piltPath))
                    {
                        using var pilt = Image.FromFile(piltPath);
                        pb.Image = new Bitmap(pilt);
                    }

                    Label lbl = new Label();
                    lbl.Text = t.Toodenimetus;
                    lbl.AutoSize = false;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Location = new Point(5, 150);
                    lbl.Width = 140;
                    lbl.Height = 40;

                    p.Controls.Add(pb);
                    p.Controls.Add(lbl);

                    p.Location = new Point(x, y);
                    page.Controls.Add(p);

                    x += 160;
                    count++;

                    if (count % 4 == 0)
                    {
                        x = 10;
                        y += 210;
                    }

                    pb.Click += (s, e) =>
                    {
                        MessageBox.Show(
                            $"Toode: {t.Toodenimetus}\nHind: {t.Hind} €",
                            "Toote info");
                    };
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

