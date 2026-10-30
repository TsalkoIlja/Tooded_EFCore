using System.Drawing;
using System.Windows.Forms;

namespace Tooded_EFCore
{
    public static class UiHelper
    {
        public static Color MainColor = Color.FromArgb(52, 152, 219);
        public static Color BackColorLight = Color.FromArgb(245, 247, 250);
        public static Color CardColor = Color.White;
        public static Color TextColor = Color.FromArgb(40, 40, 40);

        public static Font MainFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font TitleFont = new Font("Segoe UI", 11F, FontStyle.Bold);

        public static void StyleForm(Form form)
        {
            form.BackColor = BackColorLight;
            form.Font = MainFont;
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        public static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = MainColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Height = 38;
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.Font = MainFont;
            txt.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleComboBox(ComboBox cb)
        {
            cb.Font = MainFont;
            cb.FlatStyle = FlatStyle.Flat;
        }

        public static void StyleLabel(Label lbl, bool bold = false)
        {
            lbl.ForeColor = TextColor;
            lbl.Font = bold ? TitleFont : MainFont;
            lbl.AutoSize = true;
        }

        public static void StyleGroupBox(GroupBox gb)
        {
            gb.Font = TitleFont;
            gb.ForeColor = TextColor;
            gb.BackColor = CardColor;
        }

        public static void StylePictureBox(PictureBox pb)
        {
            pb.BorderStyle = BorderStyle.FixedSingle;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.BackColor = Color.White;
        }

        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 30;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = MainColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 250);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = MainFont;

            dgv.GridColor = Color.FromArgb(230, 230, 230);
        }

        public static void StyleTabControl(TabControl tab)
        {
            tab.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        public static void StyleListBox(ListBox list)
        {
            list.Font = MainFont;
            list.BorderStyle = BorderStyle.FixedSingle;
        }
    }
}
