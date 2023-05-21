using System.ComponentModel;

namespace ScreenTranslator
{
	public class ResizablePictureBox : PictureBox
	{
		private Point MouseDownLocation;

		public ResizablePictureBox(IContainer container)
		{
			container.Add(this);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.MouseDownLocation = e.Location;
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.Left += e.X - this.MouseDownLocation.X;
				this.Top += e.Y - this.MouseDownLocation.Y;
			}
			base.OnMouseMove(e);
		}
	}
}
