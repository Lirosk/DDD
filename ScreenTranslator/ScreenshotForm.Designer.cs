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
			this.screenshotPictureBox = new System.Windows.Forms.PictureBox();
			this.resizablePictureBox = new ScreenTranslator.Components.ResizablePictureBox();
			((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).BeginInit();
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
			// resizablePictureBox
			// 
			this.resizablePictureBox.BackColor = System.Drawing.Color.White;
			this.resizablePictureBox.Location = new System.Drawing.Point(125, 80);
			this.resizablePictureBox.Margin = new System.Windows.Forms.Padding(5);
			this.resizablePictureBox.Name = "resizablePictureBox";
			this.resizablePictureBox.Screenshot = null;
			this.resizablePictureBox.Size = new System.Drawing.Size(100, 50);
			this.resizablePictureBox.TabIndex = 1;
			this.resizablePictureBox.TabStop = false;
			this.resizablePictureBox.Visible = false;
			this.resizablePictureBox.Worker = null;
			// 
			// ScreenshotForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.resizablePictureBox);
			this.Controls.Add(this.screenshotPictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ScreenshotForm";
			this.Text = "ScreenshotForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScreenshotForm_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.resizablePictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private PictureBox screenshotPictureBox;
		private ResizablePictureBox resizablePictureBox;
	}
}