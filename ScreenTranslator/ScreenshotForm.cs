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

			this.pictureBox1.Image = darkenedScreenshot;
		}
	}
}