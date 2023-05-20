using System.Drawing.Imaging;

namespace ScreenTranslator
{
	public partial class ScreenshotForm : Form
	{
		private Bitmap screenshot;
		private ResizableRectangle resizableRectangle = new();
		private Point mouseDownLocation;

		public ScreenshotForm()
		{
			MakeScreenshot();
			InitializeComponent();
			PlaceDarkerScreenshot();
		}


		private void MakeScreenshot()
		{
			Screen screen = Screen.PrimaryScreen;
			this.screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);

			using (var graphics = Graphics.FromImage(screenshot))
			{
				graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size);
			}
		}

		private void PlaceDarkerScreenshot()
		{
			var darkenedScreenshot = new Bitmap(screenshot.Width, screenshot.Height);

			using (var graphics = Graphics.FromImage(darkenedScreenshot))
			{
				var brightness = 0.5f;
				var colorMatrix = new ColorMatrix(new float[][]
					{
						new float[] { brightness, 0, 0, 0, 0},
						new float[] { 0, brightness, 0, 0, 0},
						new float[] { 0, 0, brightness, 0, 0},
						new float[] { 0, 0, 0, 1, 0},
						new float[] { 0, 0, 0, 0, 1},
					});

				ImageAttributes imageAttributes = new();
				imageAttributes.SetColorMatrix(colorMatrix);

				Rectangle destinationRect = new Rectangle(0, 0, screenshot.Width, screenshot.Height);

				graphics.DrawImage(screenshot, destinationRect, 0, 0, screenshot.Width, screenshot.Height, GraphicsUnit.Pixel, imageAttributes);
			}

			this.pictureBox.Image = darkenedScreenshot;
		}

		private void pictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			this.mouseDownLocation = e.Location;
			Console.WriteLine($"Click! {mouseDownLocation.X} {mouseDownLocation.Y}");
			// Start resizing the rectangle
			resizableRectangle.StartResize(e.Location);
		}

		private void pictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (resizableRectangle.IsResizing)
			{
				// Update the rectangle size and position as the mouse moves
				resizableRectangle.UpdateResize(mouseDownLocation, e.Location);

				// Redraw the PictureBox
				pictureBox.Invalidate();
			}
		}

		private void pictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			// Finish resizing the rectangle
			resizableRectangle.EndResize();

			// Redraw the PictureBox
			pictureBox.Invalidate();
		}

		private void pictureBox_Paint(object sender, PaintEventArgs e)
		{
			// Draw the resizable rectangle on the PictureBox
			resizableRectangle.Draw(e.Graphics);
		}
	}
}