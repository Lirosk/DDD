namespace ScreenTranslator.Components
{
	public abstract class MovablePictureBox : PictureBox
	{
		private readonly bool blockDX, blockDY;

		protected Point MouseDownLocation;

		protected MovablePictureBox(bool blockDX = false, bool blockDY = false)
		{
			this.blockDX = blockDX;
			this.blockDY = blockDY;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			MouseDownLocation = e.Location;
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				int maxX = Screen.PrimaryScreen.Bounds.Width - this.Width;
				int maxY = Screen.PrimaryScreen.Bounds.Height - this.Height;

				if (!blockDX)
				{
					this.Left = Math.Max(0, Math.Min(e.X - this.MouseDownLocation.X + this.Left, maxX));
				}

				if (!blockDY)
				{
					this.Top = Math.Max(0, Math.Min(e.Y - this.MouseDownLocation.Y + this.Top, maxY));
				}
			}
			base.OnMouseMove(e);
		}
	}
}
