
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    public partial class Form2 : Form
    {
        private readonly ToodedContext _db;
        private string? valitudPilt;

        public Form2()
        {
            InitializeComponent();

            _db = new ToodedContext();

            LaeKategooriad();
            LaeTooted();
        }

        private void LaeKategooriad()
        {
            comboKategooria.DataSource =
                _db.Kategooriatabel.ToList();

            comboKategooria.DisplayMember =
                "Kategooria_nimetus";

            comboKategooria.ValueMember = "Id";
        }

        private void LaeTooted()
        {
            dataGridViewTooted.DataSource = _db.Toodetabel
                .Include(t => t.Kategooria)
                .Select(t => new
                {
                    t.Id,
                    t.Toodenimetus,
                    t.Kogus,
                    t.Hind,
                    Kategooria =
                        t.Kategooria.Kategooria_nimetus,
                    t.Pilt
                })
                .ToList();
        }

        private void txtKogus_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnValiPilt_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Pildid|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                valitudPilt = ofd.FileName;

                picturePilt.Image?.Dispose();
                picturePilt.Image = Image.FromFile(valitudPilt);
            }
        }

        private void btnLisa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNimetus.Text) ||
                !int.TryParse(txtKogus.Text, out int kogus) ||
                kogus < 0 ||
                !float.TryParse(txtHind.Text, out float hind) ||
                hind < 0 ||
                comboKategooria.SelectedValue is not int kategooriaId)
            {
                MessageBox.Show("Kontrolli toote andmeid ja kategooriat!");
                return;
            }

            string? pildiNimi = null;

            if (!string.IsNullOrEmpty(valitudPilt))
            {
                Directory.CreateDirectory("Images");

                pildiNimi = Guid.NewGuid().ToString() +
                            Path.GetExtension(valitudPilt);

                File.Copy(
                    valitudPilt,
                    Path.Combine("Images", pildiNimi),
                    true
                );
            }

            var uus = new Toode
            {
                Toodenimetus = txtNimetus.Text,
                Kogus = kogus,
                Hind = hind,
                KategooriaId = kategooriaId,
                Pilt = pildiNimi ?? ""
            };

            _db.Toodetabel.Add(uus);
            _db.SaveChanges();

            LaeTooted();

            MessageBox.Show("Toode on lisatud!");
        }

    }
}

