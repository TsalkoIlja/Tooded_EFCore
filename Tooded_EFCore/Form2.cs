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
            dataGridViewTooted.AllowUserToAddRows = false;
            dataGridViewTooted.CellClick += dataGridViewTooted_CellClick;
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
        private void dataGridViewTooted_CellClick(
            object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int id = Convert.ToInt32(
                dataGridViewTooted.Rows[e.RowIndex].Cells["Id"].Value);

            var toode = _db.Toodetabel.Find(id);

            if (toode == null)
                return;

            txtNimetus.Text = toode.Toodenimetus;
            txtKogus.Text = toode.Kogus.ToString();
            txtHind.Text = toode.Hind.ToString();
            comboKategooria.SelectedValue = toode.KategooriaId;

            valitudPilt = null;

            picturePilt.Image?.Dispose();
            picturePilt.Image = null;

            string tee = Path.Combine("Images", toode.Pilt ?? "");

            if (File.Exists(tee))
            {
                using var pilt = Image.FromFile(tee);
                picturePilt.Image = new Bitmap(pilt);
            }
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

        private void btnUuenda_Click(object? sender, EventArgs e)
        {
            if (dataGridViewTooted.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vali kõigepealt toode!");
                return;
            }

            int id = Convert.ToInt32(
                dataGridViewTooted.SelectedRows[0].Cells["Id"].Value);

            var toode = _db.Toodetabel.Find(id);

            if (toode == null)
            {
                MessageBox.Show("Toodet ei leitud!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNimetus.Text) ||
                !int.TryParse(txtKogus.Text, out int kogus) ||
                kogus < 0 ||
                !float.TryParse(txtHind.Text, out float hind) ||
                hind < 0 ||
                comboKategooria.SelectedValue is not int kategooriaId)
            {
                MessageBox.Show("Kontrolli toote andmeid!");
                return;
            }

            toode.Toodenimetus = txtNimetus.Text;
            toode.Kogus = kogus;
            toode.Hind = hind;
            toode.KategooriaId = kategooriaId;

            if (!string.IsNullOrEmpty(valitudPilt))
            {
                Directory.CreateDirectory("Images");

                string pildiNimi = Guid.NewGuid().ToString()
                                   + Path.GetExtension(valitudPilt);

                File.Copy(valitudPilt,
                    Path.Combine("Images", pildiNimi), true);

                toode.Pilt = pildiNimi;
            }

            _db.SaveChanges();
            LaeTooted();

            valitudPilt = null;

            MessageBox.Show("Toode on uuendatud!");
        }

        private void btnKustuta_Click(object? sender, EventArgs e)
        {
            if (dataGridViewTooted.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vali kõigepealt toode!");
                return;
            }

            int id = Convert.ToInt32(
                dataGridViewTooted.SelectedRows[0].Cells["Id"].Value);

            var toode = _db.Toodetabel.Find(id);

            if (toode == null)
            {
                MessageBox.Show("Toodet ei leitud!");
                return;
            }

            DialogResult vastus = MessageBox.Show(
                "Kas soovid selle toote kustutada?",
                "Kustutamine",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (vastus != DialogResult.Yes)
                return;

            _db.Toodetabel.Remove(toode);
            _db.SaveChanges();

            LaeTooted();

            txtNimetus.Clear();
            txtKogus.Clear();
            txtHind.Clear();

            picturePilt.Image?.Dispose();
            picturePilt.Image = null;

            valitudPilt = null;

            MessageBox.Show("Toode on kustutatud!");
        }


    }
}

