namespace Constellation
{
    partial class Form1
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
        	this.components = new System.ComponentModel.Container();
        	this.tmr_creation = new System.Windows.Forms.Timer(this.components);
        	this.tmr_main = new System.Windows.Forms.Timer(this.components);
        	this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        	this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.sETTINGSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.vsAIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.movieModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.changeColorsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.largeMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.randomToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.hexGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.spiralToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.mediumMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.combToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.hourglassToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.starToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.smallMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.simpleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.challenge8ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.templeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.showStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.colorDialog1 = new System.Windows.Forms.ColorDialog();
        	this.menuStrip1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// tmr_creation
        	// 
        	this.tmr_creation.Enabled = true;
        	this.tmr_creation.Interval = 250;
        	this.tmr_creation.Tick += new System.EventHandler(this.tmr_creation_Tick);
        	// 
        	// tmr_main
        	// 
        	this.tmr_main.Enabled = true;
        	this.tmr_main.Interval = 50;
        	this.tmr_main.Tick += new System.EventHandler(this.tmr_main_Tick);
        	// 
        	// menuStrip1
        	// 
        	this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
        	this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.helpToolStripMenuItem,
			this.newGameToolStripMenuItem,
			this.showStatsToolStripMenuItem,
			this.pauseToolStripMenuItem});
        	this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        	this.menuStrip1.Name = "menuStrip1";
        	this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
        	this.menuStrip1.Size = new System.Drawing.Size(1912, 35);
        	this.menuStrip1.TabIndex = 1;
        	this.menuStrip1.Text = "menuStrip1";
        	// 
        	// helpToolStripMenuItem
        	// 
        	this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        	this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
        	this.helpToolStripMenuItem.Text = "Help";
        	this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
        	// 
        	// newGameToolStripMenuItem
        	// 
        	this.newGameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.sETTINGSToolStripMenuItem,
			this.largeMapToolStripMenuItem,
			this.mediumMapToolStripMenuItem,
			this.smallMapToolStripMenuItem});
        	this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
        	this.newGameToolStripMenuItem.Size = new System.Drawing.Size(70, 29);
        	this.newGameToolStripMenuItem.Text = "Game";
        	// 
        	// sETTINGSToolStripMenuItem
        	// 
        	this.sETTINGSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.playToolStripMenuItem,
			this.vsAIToolStripMenuItem,
			this.movieModeToolStripMenuItem,
			this.changeColorsToolStripMenuItem1});
        	this.sETTINGSToolStripMenuItem.Name = "sETTINGSToolStripMenuItem";
        	this.sETTINGSToolStripMenuItem.Size = new System.Drawing.Size(204, 30);
        	this.sETTINGSToolStripMenuItem.Text = "SETTINGS";
        	// 
        	// playToolStripMenuItem
        	// 
        	this.playToolStripMenuItem.Name = "playToolStripMenuItem";
        	this.playToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
        	this.playToolStripMenuItem.Text = "2-Play";
        	this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
        	// 
        	// vsAIToolStripMenuItem
        	// 
        	this.vsAIToolStripMenuItem.Name = "vsAIToolStripMenuItem";
        	this.vsAIToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
        	this.vsAIToolStripMenuItem.Text = "Single Player";
        	this.vsAIToolStripMenuItem.Click += new System.EventHandler(this.vsAIToolStripMenuItem_Click);
        	// 
        	// movieModeToolStripMenuItem
        	// 
        	this.movieModeToolStripMenuItem.Name = "movieModeToolStripMenuItem";
        	this.movieModeToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
        	this.movieModeToolStripMenuItem.Text = "Movie Mode";
        	this.movieModeToolStripMenuItem.Click += new System.EventHandler(this.movieModeToolStripMenuItem_Click);
        	// 
        	// changeColorsToolStripMenuItem1
        	// 
        	this.changeColorsToolStripMenuItem1.Name = "changeColorsToolStripMenuItem1";
        	this.changeColorsToolStripMenuItem1.Size = new System.Drawing.Size(213, 30);
        	this.changeColorsToolStripMenuItem1.Text = "Change Colors";
        	this.changeColorsToolStripMenuItem1.Click += new System.EventHandler(this.changeColorsToolStripMenuItem1_Click);
        	// 
        	// largeMapToolStripMenuItem
        	// 
        	this.largeMapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.randomToolStripMenuItem1,
			this.hexGridToolStripMenuItem,
			this.spiralToolStripMenuItem1});
        	this.largeMapToolStripMenuItem.Name = "largeMapToolStripMenuItem";
        	this.largeMapToolStripMenuItem.Size = new System.Drawing.Size(204, 30);
        	this.largeMapToolStripMenuItem.Text = "Large Map";
        	// 
        	// randomToolStripMenuItem1
        	// 
        	this.randomToolStripMenuItem1.Name = "randomToolStripMenuItem1";
        	this.randomToolStripMenuItem1.Size = new System.Drawing.Size(165, 30);
        	this.randomToolStripMenuItem1.Text = "Random";
        	this.randomToolStripMenuItem1.Click += new System.EventHandler(this.randomToolStripMenuItem1_Click);
        	// 
        	// hexGridToolStripMenuItem
        	// 
        	this.hexGridToolStripMenuItem.Name = "hexGridToolStripMenuItem";
        	this.hexGridToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
        	this.hexGridToolStripMenuItem.Text = "Hex Grid";
        	this.hexGridToolStripMenuItem.Click += new System.EventHandler(this.hexGridToolStripMenuItem_Click);
        	// 
        	// spiralToolStripMenuItem1
        	// 
        	this.spiralToolStripMenuItem1.Name = "spiralToolStripMenuItem1";
        	this.spiralToolStripMenuItem1.Size = new System.Drawing.Size(165, 30);
        	this.spiralToolStripMenuItem1.Text = "Spiral";
        	this.spiralToolStripMenuItem1.Click += new System.EventHandler(this.spiralToolStripMenuItem1_Click);
        	// 
        	// mediumMapToolStripMenuItem
        	// 
        	this.mediumMapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.combToolStripMenuItem1,
			this.hourglassToolStripMenuItem1,
			this.starToolStripMenuItem1});
        	this.mediumMapToolStripMenuItem.Name = "mediumMapToolStripMenuItem";
        	this.mediumMapToolStripMenuItem.Size = new System.Drawing.Size(204, 30);
        	this.mediumMapToolStripMenuItem.Text = "Medium Map";
        	// 
        	// combToolStripMenuItem1
        	// 
        	this.combToolStripMenuItem1.Name = "combToolStripMenuItem1";
        	this.combToolStripMenuItem1.Size = new System.Drawing.Size(177, 30);
        	this.combToolStripMenuItem1.Text = "Comb";
        	this.combToolStripMenuItem1.Click += new System.EventHandler(this.combToolStripMenuItem1_Click);
        	// 
        	// hourglassToolStripMenuItem1
        	// 
        	this.hourglassToolStripMenuItem1.Name = "hourglassToolStripMenuItem1";
        	this.hourglassToolStripMenuItem1.Size = new System.Drawing.Size(177, 30);
        	this.hourglassToolStripMenuItem1.Text = "Hourglass";
        	this.hourglassToolStripMenuItem1.Click += new System.EventHandler(this.hourglassToolStripMenuItem1_Click);
        	// 
        	// starToolStripMenuItem1
        	// 
        	this.starToolStripMenuItem1.Name = "starToolStripMenuItem1";
        	this.starToolStripMenuItem1.Size = new System.Drawing.Size(177, 30);
        	this.starToolStripMenuItem1.Text = "Star";
        	this.starToolStripMenuItem1.Click += new System.EventHandler(this.starToolStripMenuItem1_Click);
        	// 
        	// smallMapToolStripMenuItem
        	// 
        	this.smallMapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.simpleToolStripMenuItem1,
			this.challenge8ToolStripMenuItem1,
			this.templeToolStripMenuItem1});
        	this.smallMapToolStripMenuItem.Name = "smallMapToolStripMenuItem";
        	this.smallMapToolStripMenuItem.Size = new System.Drawing.Size(204, 30);
        	this.smallMapToolStripMenuItem.Text = "Small Map";
        	// 
        	// simpleToolStripMenuItem1
        	// 
        	this.simpleToolStripMenuItem1.Name = "simpleToolStripMenuItem1";
        	this.simpleToolStripMenuItem1.Size = new System.Drawing.Size(184, 30);
        	this.simpleToolStripMenuItem1.Text = "Simple";
        	this.simpleToolStripMenuItem1.Click += new System.EventHandler(this.simpleToolStripMenuItem1_Click);
        	// 
        	// challenge8ToolStripMenuItem1
        	// 
        	this.challenge8ToolStripMenuItem1.Name = "challenge8ToolStripMenuItem1";
        	this.challenge8ToolStripMenuItem1.Size = new System.Drawing.Size(184, 30);
        	this.challenge8ToolStripMenuItem1.Text = "Challenge8";
        	this.challenge8ToolStripMenuItem1.Click += new System.EventHandler(this.challenge8ToolStripMenuItem1_Click);
        	// 
        	// templeToolStripMenuItem1
        	// 
        	this.templeToolStripMenuItem1.Name = "templeToolStripMenuItem1";
        	this.templeToolStripMenuItem1.Size = new System.Drawing.Size(184, 30);
        	this.templeToolStripMenuItem1.Text = "Temple";
        	this.templeToolStripMenuItem1.Click += new System.EventHandler(this.templeToolStripMenuItem1_Click);
        	// 
        	// showStatsToolStripMenuItem
        	// 
        	this.showStatsToolStripMenuItem.Name = "showStatsToolStripMenuItem";
        	this.showStatsToolStripMenuItem.Size = new System.Drawing.Size(111, 29);
        	this.showStatsToolStripMenuItem.Text = "Show Stats";
        	this.showStatsToolStripMenuItem.Click += new System.EventHandler(this.showStatsToolStripMenuItem_Click);
        	// 
        	// pauseToolStripMenuItem
        	// 
        	this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
        	this.pauseToolStripMenuItem.Size = new System.Drawing.Size(69, 29);
        	this.pauseToolStripMenuItem.Text = "Pause";
        	this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
        	// 
        	// colorDialog1
        	// 
        	this.colorDialog1.AllowFullOpen = false;
        	this.colorDialog1.Color = System.Drawing.Color.Silver;
        	this.colorDialog1.SolidColorOnly = true;
        	// 
        	// Form1
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.BackColor = System.Drawing.Color.Black;
        	this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.ClientSize = new System.Drawing.Size(1912, 1038);
        	this.Controls.Add(this.menuStrip1);
        	this.DoubleBuffered = true;
        	this.MainMenuStrip = this.menuStrip1;
        	this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
        	this.Name = "Form1";
        	this.Text = "Constellation";
        	this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        	this.Load += new System.EventHandler(this.Form1_Load);
        	this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
        	this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
        	this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
        	this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
        	this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
        	this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
        	this.menuStrip1.ResumeLayout(false);
        	this.menuStrip1.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmr_creation;
        private System.Windows.Forms.Timer tmr_main;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripMenuItem sETTINGSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vsAIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeColorsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem movieModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem hexGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spiralToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mediumMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem combToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem hourglassToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem starToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem smallMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simpleToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem challenge8ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem templeToolStripMenuItem1;
    }
}

