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

            Text = "Peamen¸¸";
            Size = new Size(400, 420);
            StartPosition = FormStartPosition.CenterScreen;

            // Toodete haldus
            Button btnHaldus = new Button();
            btnHaldus.Text = "Toodete haldus";
            btnHaldus.Size = new Size(200, 50);
            btnHaldus.Location = new Point(90, 60);
            btnHaldus.Enabled = AktiivneKasutaja.Roll == "admin";


            btnHaldus.Click += (s, e) =>
            {
                if (AktiivneKasutaja.Roll != "admin")
                {
                    MessageBox.Show("Ligip‰‰s keelatud!");
                    return;
                }

                using Form2 haldus = new Form2();
                haldus.ShowDialog();
            };


            // Pood
            Button btnPood = new Button();
            btnPood.Text = "Pood";
            btnPood.Size = new Size(200, 50);
            btnPood.Location = new Point(90, 130);

            btnPood.Click += (s, e) =>
            {
                using Form3 pood = new Form3();
                pood.ShowDialog();
            };

            // Ostukorv ja arve
            Button btnArve = new Button();
            btnArve.Text = "Ostukorv ja arve";
            btnArve.Size = new Size(200, 50);
            btnArve.Location = new Point(90, 200);

            btnArve.Click += (s, e) =>
            {
                using ArveForm arve = new ArveForm();
                arve.ShowDialog();
            };

            Controls.Add(btnHaldus);
            Controls.Add(btnPood);
            Controls.Add(btnArve);


            Button btnLisaKasutaja = new Button();

            btnLisaKasutaja.Text = "Lisa kasutaja";
            btnLisaKasutaja.Size = new Size(200, 50);
            btnLisaKasutaja.Location = new Point(90, 270);

            btnLisaKasutaja.Visible =
                AktiivneKasutaja.Roll == "admin";

            btnLisaKasutaja.Click += (sender, e) =>
            {
                if (AktiivneKasutaja.Roll != "admin")
                {
                    MessageBox.Show("Ligip‰‰s keelatud!");
                    return;
                }

                using (LisaKasutajaForm vorm = new LisaKasutajaForm())
                {
                    vorm.ShowDialog();
                }
            };

            Controls.Add(btnLisaKasutaja);

        }
    }
}


