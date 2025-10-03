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
            panelBoard = new Panel();
            SuspendLayout();
            // 
            // panelBoard
            // 
            panelBoard.AutoScroll = true;
            panelBoard.BackColor = Color.LightGray;
            panelBoard.Location = new Point(10, 11);
            panelBoard.Name = "panelBoard";
            panelBoard.Size = new Size(600, 600);
            panelBoard.TabIndex = 0;
            panelBoard.Paint += panelBoard_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(522, 575);
            Controls.Add(panelBoard);
            Name = "Form1";
            Text = "Buscaminas - WinForms";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
