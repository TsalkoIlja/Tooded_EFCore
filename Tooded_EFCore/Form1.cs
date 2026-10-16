using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tooded_EFCore
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text = "Peamenüü";
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterScreen;

            Button btnHaldus = new Button();
            btnHaldus.Text = "Toodete haldus";
            btnHaldus.Size = new Size(200, 50);
            btnHaldus.Location = new Point(90, 60);

            btnHaldus.Click += (s, e) =>
            {
                Form2 haldus = new Form2();
                haldus.ShowDialog();
            };

            Button btnPood = new Button();
            btnPood.Text = "Pood";
            btnPood.Size = new Size(200, 50);
            btnPood.Location = new Point(90, 130);

            btnPood.Click += (s, e) =>
            {
                Form3 pood = new Form3();
                pood.ShowDialog();
            };

            Controls.Add(btnHaldus);
            Controls.Add(btnPood);
        }
    }
}

