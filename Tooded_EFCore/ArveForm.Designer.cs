namespace Tooded_EFCore
{
    partial class ArveForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listOstukorv = new ListBox();
            btnLisaKorvi = new Button();
            btnGenPDF = new Button();
            btnSaadaEmail = new Button();
            SuspendLayout();
            // 
            // listOstukorv
            // 
            listOstukorv.FormattingEnabled = true;
            listOstukorv.Location = new Point(28, 32);
            listOstukorv.Name = "listOstukorv";
            listOstukorv.Size = new Size(173, 139);
            listOstukorv.TabIndex = 0;
            listOstukorv.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // btnLisaKorvi
            // 
            btnLisaKorvi.Location = new Point(306, 32);
            btnLisaKorvi.Name = "btnLisaKorvi";
            btnLisaKorvi.Size = new Size(75, 23);
            btnLisaKorvi.TabIndex = 1;
            btnLisaKorvi.Text = "Lisa toode korvi";
            btnLisaKorvi.UseVisualStyleBackColor = true;
            btnLisaKorvi.Click += btnLisaKorvi_Click;
            // 
            // btnGenPDF
            // 
            btnGenPDF.Location = new Point(306, 95);
            btnGenPDF.Name = "btnGenPDF";
            btnGenPDF.Size = new Size(75, 23);
            btnGenPDF.TabIndex = 2;
            btnGenPDF.Text = "Genereeri PDF";
            btnGenPDF.UseVisualStyleBackColor = true;
            btnGenPDF.Click += btnGenPDF_Click;
            // 
            // btnSaadaEmail
            // 
            btnSaadaEmail.Location = new Point(306, 148);
            btnSaadaEmail.Name = "btnSaadaEmail";
            btnSaadaEmail.Size = new Size(75, 23);
            btnSaadaEmail.TabIndex = 3;
            btnSaadaEmail.Text = "Saada emailiga";
            btnSaadaEmail.UseVisualStyleBackColor = true;
            btnSaadaEmail.Click += btnSaadaEmail_Click;
            // 
            // ArveForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(612, 378);
            Controls.Add(btnSaadaEmail);
            Controls.Add(btnGenPDF);
            Controls.Add(btnLisaKorvi);
            Controls.Add(listOstukorv);
            Name = "ArveForm";
            Text = "ArveForm";
            ResumeLayout(false);
        }

        #endregion

        private ListBox listOstukorv;
        private Button btnLisaKorvi;
        private Button btnGenPDF;
        private Button btnSaadaEmail;
    }
}