using System.Drawing.Drawing2D;

namespace ScreenTranslator.Components
{
	public class ResizeHandler : MovablePictureBox
	{
		private readonly HandleResize resize;
		private readonly Cursor cursorOnHover;

		public const int StandardSize = 7;

		public delegate void HandleResize(ResizeHandler sender);

		public ResizeHandler(int x, int y, Cursor cursorOnHover, HandleResize resize, bool blockDX = false, bool blockDY = false) : base(blockDX, blockDY)
		{
			this.resize = resize;
			this.Size = new Size(StandardSize, StandardSize);
			this.BackColor = Color.White;
			this.Location = new Point(x, y);
			this.cursorOnHover = cursorOnHover;
			this.BackColor = Color.Black;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			// Define the border color and style
			Color borderColor = Color.White;
			DashStyle borderStyle = DashStyle.Solid;

			// Create a Pen with the specified color and style
			using (Pen borderPen = new Pen(borderColor))
			{
				borderPen.DashStyle = borderStyle;

				// Draw the border around the picture box
				e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.Button == MouseButtons.Left)
			{
				this.resize.Invoke(this);
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			this.Cursor = this.cursorOnHover;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this.Cursor = Cursors.Default;
		}
	}
}
