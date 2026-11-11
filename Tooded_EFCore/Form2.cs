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
        private Button btnPood = null!;

        public Form2()
        {
            if (AktiivneKasutaja.Roll != "admin")
            {
                throw new UnauthorizedAccessException(
                    "Ainult administraator saab tooteid hallata!");
            }

            InitializeComponent();

            // Vormi ja tabeli laiendamine
            LaiendaVormiJaTabelit();

            // Pood nupu lisamine
            LisaPoodNupp();

            // Värvide kujundamine
            KujundaVarvid();

            dataGridViewTooted.AllowUserToAddRows = false;
            dataGridViewTooted.CellClick += dataGridViewTooted_CellClick;

            _db = new ToodedContext();

            LaeKategooriad();
            LaeTooted();
        }

        // Vormi ja tabeli laiendamine
        private void LaiendaVormiJaTabelit()
        {
            dataGridViewTooted.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            Width += 130;
        }

        // Pood nupu lisamine
        private void LisaPoodNupp()
        {
            btnPood = new Button();

            btnPood.Name = "btnPood";
            btnPood.Text = "Pood";
            btnPood.Size = btnArve.Size;

            btnPood.Location = new Point(
                btnArve.Right + 15,
                btnArve.Top
            );

            btnPood.Click += btnPood_Click;

            btnArve.Parent.Controls.Add(btnPood);
        }

        // Vormi värvid
        private void KujundaVarvid()
        {
            BackColor = Color.FromArgb(245, 247, 250);

            // Tabeli kujundus
            dataGridViewTooted.BackgroundColor = Color.White;
            dataGridViewTooted.EnableHeadersVisualStyles = false;

            dataGridViewTooted.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(52, 152, 219);

            dataGridViewTooted.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dataGridViewTooted.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dataGridViewTooted.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(190, 220, 245);

            dataGridViewTooted.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dataGridViewTooted.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 249, 253);

            // Nuppude värvid
            VarviNupp(btnLisa, Color.FromArgb(52, 152, 219));
            VarviNupp(btnUuenda, Color.FromArgb(37, 99, 235));
            VarviNupp(btnKustuta, Color.FromArgb(220, 53, 69));
            VarviNupp(btnValiPilt, Color.FromArgb(100, 116, 139));
            VarviNupp(btnArve, Color.FromArgb(34, 139, 94));
            VarviNupp(btnPood, Color.FromArgb(52, 152, 219));

            // Pildi taust
            picturePilt.BackColor = Color.White;
        }

        // Nupu värvi muutmine
        private void VarviNupp(Button nupp, Color varv)
        {
            nupp.FlatStyle = FlatStyle.Flat;
            nupp.FlatAppearance.BorderSize = 0;
            nupp.BackColor = varv;
            nupp.ForeColor = Color.White;
            nupp.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            nupp.Cursor = Cursors.Hand;
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

            string tee = Path.Combine(
                Kaustad.Pildid,
                toode.Pilt ?? "");

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

                using var pilt = Image.FromFile(valitudPilt);
                picturePilt.Image = new Bitmap(pilt);
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
                MessageBox.Show(
                    "Kontrolli toote andmeid ja kategooriat!");
                return;
            }

            string? pildiNimi = null;

            if (!string.IsNullOrEmpty(valitudPilt))
            {
                Directory.CreateDirectory(Kaustad.Pildid);

                pildiNimi = Guid.NewGuid().ToString() +
                            Path.GetExtension(valitudPilt);

                File.Copy(
                    valitudPilt,
                    Path.Combine(Kaustad.Pildid, pildiNimi),
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
                Directory.CreateDirectory(Kaustad.Pildid);

                string pildiNimi = Guid.NewGuid().ToString() +
                                   Path.GetExtension(valitudPilt);

                File.Copy(
                    valitudPilt,
                    Path.Combine(Kaustad.Pildid, pildiNimi),
                    true
                );

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

        private void txtNimetus_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnArve_Click(object sender, EventArgs e)
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

            using ArveForm arve = new ArveForm();
            arve.SelectedToode = toode;
            arve.ShowDialog();
        }

        // Pood nupu vajutamine
        private void btnPood_Click(object sender, EventArgs e)
        {
            using Form3 pood = new Form3();

            pood.ShowDialog(this);

            // Toodete tabeli uuendamine pärast poe sulgemist
            LaeTooted();
        }
    }
}



