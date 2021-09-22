
namespace AddShParameters
{
    partial class WinForm2
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
            this.listView6 = new System.Windows.Forms.ListView();
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button7 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView6
            // 
            this.listView6.CheckBoxes = true;
            this.listView6.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader12});
            this.listView6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView6.FullRowSelect = true;
            this.listView6.GridLines = true;
            this.listView6.HideSelection = false;
            this.listView6.Location = new System.Drawing.Point(5, 5);
            this.listView6.Name = "listView6";
            this.listView6.Size = new System.Drawing.Size(237, 414);
            this.listView6.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView6.TabIndex = 41;
            this.listView6.UseCompatibleStateImageBehavior = false;
            this.listView6.View = System.Windows.Forms.View.Details;
            this.listView6.SelectedIndexChanged += new System.EventHandler(this.listView6_SelectedIndexChanged);
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Имя категории";
            this.columnHeader12.Width = 230;
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.button7.Location = new System.Drawing.Point(37, 428);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(158, 27);
            this.button7.TabIndex = 43;
            this.button7.Text = "Принять";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // WinForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 467);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.listView6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WinForm2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Категории";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListView listView6;
        public System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.Button button7;
    }
}