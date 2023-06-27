namespace ScreenTranslator
{
	partial class SettingsForm
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
			this.comboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// comboBox
			// 
			this.comboBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBox.FormattingEnabled = true;
			this.comboBox.Location = new System.Drawing.Point(0, 0);
			this.comboBox.MaxDropDownItems = 12;
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new System.Drawing.Size(234, 23);
			this.comboBox.TabIndex = 0;
			this.comboBox.DropDown += new System.EventHandler(this.comboBox_DropDown);
			this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(234, 211);
			this.Controls.Add(this.comboBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "SettingsForm";
			this.Text = "Language";
			this.ResumeLayout(false);

		}

		#endregion

		private ComboBox comboBox;
	}
}