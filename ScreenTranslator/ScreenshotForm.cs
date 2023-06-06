using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ScreenTranslator
{
	public partial class ScreenshotForm : Form
	{
		private Bitmap screenshot;
		private FormWindowState previousWindowState = FormWindowState.Normal;

		public ScreenshotForm()
		{
			InitializeComponent();

			//MakeScreenshot();
			//PlaceDarkerScreenshot();

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

			this.resizablePictureBox.Screenshot = this.screenshot!;
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

			this.Size = darkenedScreenshot.Size;
			this.screenshotPictureBox.Size = darkenedScreenshot.Size;

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
				HideToTray();
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.RShiftKey)
			{
				ShowFromTray();
				e.Handled = true;
			}
		}

		private void screenshotPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.resizablePictureBox.Image is null)
			{
				return;
			}

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

		private void ScreenshotForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				this.WindowState = FormWindowState.Minimized;
				this.Hide();
			}
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			ShowFromTray();
		}

		private void HideToTray()
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				return;
			}

			this.resizablePictureBox.Visible = false;
			previousWindowState = this.WindowState;
			this.WindowState = FormWindowState.Minimized;
			this.Hide();

			//this.ShowInTaskbar = false;
		}

		private void ShowFromTray()
		{
			if (this.WindowState == FormWindowState.Normal)
			{
				return;
			}

			MakeScreenshot();
			PlaceDarkerScreenshot();

			if (this.WindowState == FormWindowState.Minimized)
			{
				this.WindowState = previousWindowState;
				this.Bounds = GetValidFormBounds();
			}

			this.Show();
			this.Activate();

			//this.ShowInTaskbar = true;
			this.TopLevel = true;
		}

		private Rectangle GetValidFormBounds()
		{
			Screen primaryScreen = Screen.PrimaryScreen;
			Rectangle workingArea = primaryScreen.WorkingArea;

			int x = workingArea.X;
			int y = workingArea.Y;
			int width = workingArea.Width;
			int height = workingArea.Height;

			return new Rectangle(x, y, width, height);
		}

		private void ScreenshotForm_Load(object sender, EventArgs e)
		{
			RegisterKeyboardHook();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void ScreenshotForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			//if ((Control.ModifierKeys & Keys.Alt) != 0 && e.CloseReason == CloseReason.UserClosing)
			//{
			//	e.Cancel = true;
			//}

			UnregisterKeyboardHook();
		}

		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ShowFromTray();
		}

		// Define the key code for the right Shift key
		private const int VK_RSHIFT = 0xA1;

		// Define the delegate for the keyboard hook callback
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		// Define the keyboard hook handle and callback
		private IntPtr hookHandle;
		private LowLevelKeyboardProc hookCallback;

		// Register the keyboard hook
		private void RegisterKeyboardHook()
		{
			hookCallback = KeyboardHookCallback;
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, hookCallback, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		// Unregister the keyboard hook
		private void UnregisterKeyboardHook()
		{
			UnhookWindowsHookEx(hookHandle);
		}

		// Handle the keyboard events
		private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				if (vkCode == VK_RSHIFT)
				{
					// Right Shift key is pressed
					ShowFromTray();

					// Suppress the default behavior of the Shift key press
					return (IntPtr)1;
				}
			}

			return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
		}

		// Define the constants for the keyboard hook
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;

		// Import the necessary WinAPI functions
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SettingsForm settingsForm = new SettingsForm();
			settingsForm.Show();
		}

		private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}