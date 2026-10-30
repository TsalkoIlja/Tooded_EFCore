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

            UiHelper.StyleForm(this);

            bool isAdmin = AktiivneKasutaja.Roll == "admin";

            Text = "Tooded App - Peamen¸¸";
            ClientSize = new Size(440, isAdmin ? 490 : 330);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Pealkiri
            Label lblPealkiri = new Label();
            lblPealkiri.Text = "TOODED APP";
            lblPealkiri.Font = new Font(
                "Segoe UI", 22, FontStyle.Bold);
            lblPealkiri.ForeColor = UiHelper.MainColor;
            lblPealkiri.TextAlign = ContentAlignment.MiddleCenter;
            lblPealkiri.SetBounds(40, 25, 360, 50);

            Controls.Add(lblPealkiri);

            // Alapealkiri
            Label lblInfo = new Label();
            lblInfo.Text = "Peamen¸¸";
            lblInfo.Font = new Font("Segoe UI", 11);
            lblInfo.ForeColor = UiHelper.TextColor;
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblInfo.SetBounds(40, 75, 360, 30);

            Controls.Add(lblInfo);

            // Toodete haldus - ainult admin
            if (isAdmin)
            {
                Button btnHaldus = new Button();
                btnHaldus.Text = "Toodete haldus";
                UiHelper.StyleButton(btnHaldus);
                btnHaldus.SetBounds(90, 125, 260, 55);

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

                Controls.Add(btnHaldus);
            }

            // Pood
            Button btnPood = new Button();
            btnPood.Text = "Pood";
            UiHelper.StyleButton(btnPood);

            btnPood.SetBounds(
                90,
                isAdmin ? 195 : 125,
                260,
                55
            );

            btnPood.Click += (s, e) =>
            {
                using Form3 pood = new Form3();
                pood.ShowDialog();
            };

            Controls.Add(btnPood);

            // Ostukorv ja arve
            Button btnArve = new Button();
            btnArve.Text = "Ostukorv ja arve";
            UiHelper.StyleButton(btnArve);

            btnArve.SetBounds(
                90,
                isAdmin ? 265 : 195,
                260,
                55
            );

            btnArve.Click += (s, e) =>
            {
                using ArveForm arve = new ArveForm();
                arve.ShowDialog();
            };

            Controls.Add(btnArve);

            // Lisa kasutaja - ainult admin
            if (isAdmin)
            {
                Button btnLisaKasutaja = new Button();
                btnLisaKasutaja.Text = "Lisa kasutaja";
                UiHelper.StyleButton(btnLisaKasutaja);
                btnLisaKasutaja.SetBounds(90, 335, 260, 55);

                btnLisaKasutaja.Click += (sender, e) =>
                {
                    if (AktiivneKasutaja.Roll != "admin")
                    {
                        MessageBox.Show("Ligip‰‰s keelatud!");
                        return;
                    }

                    using LisaKasutajaForm vorm =
                        new LisaKasutajaForm();

                    vorm.ShowDialog();
                };

                Controls.Add(btnLisaKasutaja);
            }

            // Alumine tekst
            Label lblAlumine = new Label();
            lblAlumine.Text = "Toodete haldamise rakendus";
            lblAlumine.Font = new Font("Segoe UI", 9);
            lblAlumine.ForeColor = Color.Gray;
            lblAlumine.TextAlign =
                ContentAlignment.MiddleCenter;

            lblAlumine.SetBounds(
                40,
                isAdmin ? 435 : 280,
                360,
                25
            );

            Controls.Add(lblAlumine);
        }
    }
}




