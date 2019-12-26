namespace winLive
{
    partial class ScreenAnts
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
            this.razorPainterWFCtl1 = new RazorGDIControlWF.RazorPainterWFCtl();
            this.SuspendLayout();
            // 
            // razorPainterWFCtl1
            // 
            this.razorPainterWFCtl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.razorPainterWFCtl1.Location = new System.Drawing.Point(0, 0);
            this.razorPainterWFCtl1.Margin = new System.Windows.Forms.Padding(0);
            this.razorPainterWFCtl1.MinimumSize = new System.Drawing.Size(1, 1);
            this.razorPainterWFCtl1.Name = "razorPainterWFCtl1";
            this.razorPainterWFCtl1.Size = new System.Drawing.Size(1067, 554);
            this.razorPainterWFCtl1.TabIndex = 0;
            // 
            // ScreenAnts
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.razorPainterWFCtl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ScreenAnts";
            this.Text = "ScreenLive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScreenLive_FormClosing);
            this.Shown += new System.EventHandler(this.ScreenAnts_Shown);
            this.ResumeLayout(false);

        }

        #endregion
        private RazorGDIControlWF.RazorPainterWFCtl razorPainterWFCtl1;
    }
}