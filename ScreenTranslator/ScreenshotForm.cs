using System.Drawing.Imaging;

namespace ScreenTranslator
{
	public partial class ScreenshotForm : Form
	{
		private Bitmap screenshot;

		public ScreenshotForm()
		{
			MakeScreenshot();
			InitializeComponent();
			PlaceDarkerScreenshot();
			this.resizablePictureBox.Screenshot = this.screenshot!;
			this.resizablePictureBox.Worker = new();
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

			this.screenshotPictureBox.Image = darkenedScreenshot;
		}

		private Point MouseDownLocation;
		private void screenshotPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			resizablePictureBox.Size = new(0, 0);
			this.MouseDownLocation = e.Location;
			this.resizablePictureBox.Visible = true;
			this.resizablePictureBox.Location = e.Location;
			this.resizablePictureBox.UpdateImage();
		}

		private void screenshotPictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{

				int x1, x2, y1, y2;

				x1 = MouseDownLocation.X;
				y1 = MouseDownLocation.Y;

				x2 = e.X;
				y2 = e.Y;

				if (x1 > x2)
				{
					(x1, x2) = (x2, x1);
				}

				if (y1 > y2)
				{
					(y1, y2) = (y2, y1);
				}

				this.resizablePictureBox.Location = new(x1, y1);
				this.resizablePictureBox.Size = new Size(x2 - x1, y2 - y1);

				this.resizablePictureBox.UpdateImage();
			}
		}

		private void ScreenshotForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				this.Close();
			}
		}

		private void screenshotPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			Bitmap bitmap = new(this.resizablePictureBox.Image);

			this.resizablePictureBox.Worker.Start(bitmap,
				(result) =>
				{
					this.resizablePictureBox.Image = result;
				},
				() =>
				{
					this.resizablePictureBox.Image = null;
				}
				);
		}
	}
}