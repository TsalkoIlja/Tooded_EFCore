namespace Tooded_EFCore
{
    partial class Form3
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
            tabPood = new TabControl();
            SuspendLayout();
            // 
            // tabPood
            // 
            tabPood.Location = new Point(10, 10);
            tabPood.Name = "tabPood";
            tabPood.SelectedIndex = 0;
            tabPood.Size = new Size(900, 600);
            tabPood.TabIndex = 0;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabPood);
            Name = "Form3";
            Text = "Pood";
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabPood;
    }
}