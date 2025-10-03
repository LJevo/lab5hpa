namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelBoard;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelBoard = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelBoard
            // 
            this.panelBoard.Location = new System.Drawing.Point(12, 12);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(600, 600);
            this.panelBoard.AutoScroll = true;
            this.panelBoard.BackColor = System.Drawing.Color.LightGray;
            this.panelBoard.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 640);
            this.Controls.Add(this.panelBoard);
            this.Name = "Form1";
            this.Text = "Buscaminas - WinForms";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
