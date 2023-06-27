namespace ScreenTranslator
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
			AddItemsToComboBox();
			SetDefaultToComboBox();
		}

		private void SetDefaultToComboBox()
		{
			this.comboBox.SelectedItem = Properties.Settings.Default.SelectedLanguage;
		}


		private void AddItemsToComboBox()
		{
			this.comboBox.Items.AddRange(Languages.Items.Keys.ToArray());
		}

		private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.SelectedLanguage = (string)comboBox.SelectedItem;
			Properties.Settings.Default.Save();
		}

		private void comboBox_DropDown(object sender, EventArgs e)
		{
			int maxVisibleItems = Math.Min(comboBox.Items.Count, comboBox.MaxDropDownItems);
			int itemHeight = comboBox.GetItemHeight(0); // Get the height of a single item
			int maxHeight = maxVisibleItems * itemHeight;

			// Set the maximum height for the dropdown list
			comboBox.DropDownHeight = maxHeight;
		}
	}
}
