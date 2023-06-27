using ScreenTranslator.Components;

namespace ScreenTranslator
{
    partial class ScreenshotForm: Form
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenshotForm));
			this.screenshotPictureBox = new System.Windows.Forms.PictureBox();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resizablePictureBox = new ResizablePictureBox(this);
			((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).BeginInit();
			this.contextMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.resizablePictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// screenshotPictureBox
			// 
			this.screenshotPictureBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.screenshotPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.screenshotPictureBox.Location = new System.Drawing.Point(0, 0);
			this.screenshotPictureBox.Name = "screenshotPictureBox";
			this.screenshotPictureBox.Size = new System.Drawing.Size(800, 450);
			this.screenshotPictureBox.TabIndex = 0;
			this.screenshotPictureBox.TabStop = false;
			this.screenshotPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.screenshotPictureBox_MouseDown);
			this.screenshotPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.screenshotPictureBox_MouseMove);
			this.screenshotPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.screenshotPictureBox_MouseUp);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "ScreenTranslator";
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick_1);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(117, 48);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.settingsToolStripMenuItem.Text = "Settings";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click_1);
			// 
			// resizablePictureBox
			// 
			this.resizablePictureBox.Location = new System.Drawing.Point(8, 8);
			this.resizablePictureBox.Name = "resizablePictureBox";
			this.resizablePictureBox.Size = new System.Drawing.Size(100, 50);
			this.resizablePictureBox.TabIndex = 1;
			this.resizablePictureBox.TabStop = false;
			this.resizablePictureBox.Visible = false;
			// 
			// ScreenshotForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.resizablePictureBox);
			this.Controls.Add(this.screenshotPictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.Name = "ScreenshotForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ScreenshotForm";
			this.TopMost = true;
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScreenshotForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScreenshotForm_FormClosed);
			this.Load += new System.EventHandler(this.ScreenshotForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScreenshotForm_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).EndInit();
			this.contextMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.resizablePictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private PictureBox screenshotPictureBox;
		private NotifyIcon notifyIcon;
		private ContextMenuStrip contextMenuStrip;
		private ToolStripMenuItem settingsToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ResizablePictureBox resizablePictureBox;
	}
}