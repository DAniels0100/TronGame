namespace TronFinal
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            GameCanvas = new PictureBox();
            StartBtn = new Button();
            GameTime = new System.Windows.Forms.Timer(components);
            Scoretxt = new Label();
            ((System.ComponentModel.ISupportInitialize)GameCanvas).BeginInit();
            SuspendLayout();
            // 
            // GameCanvas
            // 
            GameCanvas.BackColor = SystemColors.ActiveCaptionText;
            GameCanvas.Location = new Point(87, 90);
            GameCanvas.Name = "GameCanvas";
            GameCanvas.Size = new Size(1100, 630);
            GameCanvas.TabIndex = 0;
            GameCanvas.TabStop = false;
            GameCanvas.Paint += UpdatePictureBox;
            // 
            // StartBtn
            // 
            StartBtn.BackColor = Color.FromArgb(128, 255, 255);
            StartBtn.Location = new Point(87, 12);
            StartBtn.Name = "StartBtn";
            StartBtn.Size = new Size(175, 72);
            StartBtn.TabIndex = 1;
            StartBtn.Text = "Start";
            StartBtn.UseVisualStyleBackColor = false;
            StartBtn.Click += StartGame;
            // 
            // GameTime
            // 
            GameTime.Interval = 40;
            GameTime.Tick += TimeEvent;
            // 
            // Scoretxt
            // 
            Scoretxt.AutoSize = true;
            Scoretxt.Location = new Point(291, 41);
            Scoretxt.Name = "Scoretxt";
            Scoretxt.Size = new Size(0, 15);
            Scoretxt.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1322, 759);
            Controls.Add(Scoretxt);
            Controls.Add(StartBtn);
            Controls.Add(GameCanvas);
            ForeColor = SystemColors.ControlText;
            Name = "Form1";
            Text = "Form1";
            KeyDown += KeyIsDown;
            KeyUp += KeyIsUp;
            ((System.ComponentModel.ISupportInitialize)GameCanvas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox GameCanvas;
        private Button StartBtn;
        private System.Windows.Forms.Timer GameTime;
        private Label Scoretxt;
    }
}
