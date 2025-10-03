namespace WinFormsApp1
{
    //Jose Luis Silvera 8-1013-1016
    //Lenn Mendoza 8-1021-359
    partial class Buscaminas
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
            panelBoard.Location = new Point(11, 15);
            panelBoard.Margin = new Padding(3, 4, 3, 4);
            panelBoard.Name = "panelBoard";
            panelBoard.Size = new Size(686, 800);
            panelBoard.TabIndex = 0;
            panelBoard.Paint += panelBoard_Paint;
            // 
            // Buscaminas
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(597, 767);
            Controls.Add(panelBoard);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Buscaminas";
            Text = "Buscaminas - WinForms";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
