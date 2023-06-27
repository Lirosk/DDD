using System.Drawing.Drawing2D;

namespace ScreenTranslator.Components
{
	public class ResizablePictureBox : MovablePictureBox
	{
		private Bitmap screenshot;

		private readonly ResizeHandler resizeHandlerNW;
		private readonly ResizeHandler resizeHandlerN;
		private readonly ResizeHandler resizeHandlerNE;
		private readonly ResizeHandler resizeHandlerE;
		private readonly ResizeHandler resizeHandlerSE;
		private readonly ResizeHandler resizeHandlerS;
		private readonly ResizeHandler resizeHandlerSW;
		private readonly ResizeHandler resizeHandlerW;

		public ScreenTranslatorMediumWorker.Callback SuccessfulCallback { get; set; }
		public Action UnsuccessfullCallback { get; set; }

		public ScreenTranslatorMediumWorker Worker { get; set; }

		public ScreenshotForm Owner { get; set; }

		public ResizablePictureBox(ScreenshotForm Owner) : base()
		{
			this.Owner = Owner;
			var (w, h, s) = (this.Width, this.Height, ResizeHandler.StandardSize);

			resizeHandlerW = new(0, (h - s) / 2, Cursors.SizeWE, (sender) => { this.HandleResizeW(sender); }, this.Owner.StartImageProcessing, blockDX: true, blockDY: true);
			resizeHandlerN = new(w / 2, 0, Cursors.SizeNS, (sender) => { this.HandleResizeN(sender); }, this.Owner.StartImageProcessing, blockDX: true, blockDY: true);
			resizeHandlerS = new(w / 2, h - s, Cursors.SizeNS, (sender) => { this.HandleResizeS(sender); }, this.Owner.StartImageProcessing, blockDX: true);
			resizeHandlerE = new(w - s, (h - s) / 2, Cursors.SizeWE, (sender) => { this.HandleResizeE(sender); }, this.Owner.StartImageProcessing, blockDY: true);

			resizeHandlerSE = new(w - s, h - s, Cursors.SizeNWSE, (sender) => { this.HandleResizeE(sender, true); this.HandleResizeS(sender); }, this.Owner.StartImageProcessing);
			resizeHandlerSW = new(0, h - s, Cursors.SizeNESW, (sender) => { this.HandleResizeS(sender, true); this.HandleResizeW(sender); }, this.Owner.StartImageProcessing, blockDX: true);
			resizeHandlerNE = new(w - s, 0, Cursors.SizeNESW, (sender) => { this.HandleResizeE(sender, true); this.HandleResizeN(sender); }, this.Owner.StartImageProcessing, blockDY: true);
			resizeHandlerNW = new(0, 0, Cursors.SizeNWSE, (sender) => { this.HandleResizeN(sender, true); this.HandleResizeW(sender); }, this.Owner.StartImageProcessing, blockDX: true, blockDY: true);

			//resizeHandlerW = new(0, (h - s) / 2, Cursors.SizeWE, (sender) => { this.HandleResizeW(sender); this.Owner?.StartImageProcessing(); }, blockDX: true, blockDY: true);
			//resizeHandlerN = new(w / 2, 0, Cursors.SizeNS, (sender) => { this.HandleResizeN(sender); this.Owner?.StartImageProcessing(); }, blockDX: true, blockDY: true);
			//resizeHandlerS = new(w / 2, h - s, Cursors.SizeNS, (sender) => { this.HandleResizeS(sender); this.Owner?.StartImageProcessing(); }, blockDX: true);
			//resizeHandlerE = new(w - s, (h - s) / 2, Cursors.SizeWE, (sender) => { this.HandleResizeE(sender); this.Owner?.StartImageProcessing(); }, blockDY: true);

			//resizeHandlerSE = new(w - s, h - s, Cursors.SizeNWSE, (sender) => { this.HandleResizeE(sender, true); this.HandleResizeS(sender); this.Owner?.StartImageProcessing(); });
			//resizeHandlerSW = new(0, h - s, Cursors.SizeNESW, (sender) => { this.HandleResizeS(sender, true); this.HandleResizeW(sender); this.Owner?.StartImageProcessing(); }, blockDX: true);
			//resizeHandlerNE = new(w - s, 0, Cursors.SizeNESW, (sender) => { this.HandleResizeE(sender, true); this.HandleResizeN(sender); this.Owner?.StartImageProcessing(); }, blockDY: true);
			//resizeHandlerNW = new(0, 0, Cursors.SizeNWSE, (sender) => { this.HandleResizeN(sender, true); this.HandleResizeW(sender); this.Owner?.StartImageProcessing(); }, blockDX: true, blockDY: true);

			ResizeHandler[] resizeHandlers = { resizeHandlerNW, resizeHandlerW, resizeHandlerSW, resizeHandlerN, resizeHandlerS, resizeHandlerNE, resizeHandlerE, resizeHandlerSE };

			for (int i = 0; i < resizeHandlers.Length; i++)
			{
				if (resizeHandlers[i] is not null)
				{
					Controls.Add(resizeHandlers[i]);
				}
			}
		}

		public Bitmap Screenshot { get => screenshot; set => screenshot = value; }

		public void UpdateImage()
		{
			if (screenshot is null || this.Width < ResizeHandler.StandardSize * 3 || this.Height < ResizeHandler.StandardSize * 3)
			{
				return;
			}

			this.Image = CropBitmap(screenshot, Left, Top, Width, Height);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			var w = this.Width;
			var h = this.Height;
			var s = ResizeHandler.StandardSize;

			resizeHandlerW.Location = new(0, (h - s) / 2);
			resizeHandlerN.Location = new((w - s) / 2, 0);
			resizeHandlerS.Location = new((w - s) / 2, h - s);
			resizeHandlerE.Location = new(w - s, (h - s) / 2);
			resizeHandlerSE.Location = new(w - s, h - s);
			resizeHandlerSW.Location = new(0, h - s);
			resizeHandlerNE.Location = new(w - s, 0);
			resizeHandlerNW.Location = new(0, 0);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left)
			{
				this.UpdateImage();
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);

			var dashes = new (Color color, int gapAndWidth)[]
			{
				new(Color.White, 6), new(Color.Black, 3)
			};

			foreach (var dash in dashes)
			{
				var color = dash.color;
				var gapAndWidth = dash.gapAndWidth;

				using (Pen pen = new Pen(color))
				{
					pen.DashStyle = DashStyle.Dash;
					pen.DashPattern = new float[] { gapAndWidth, gapAndWidth };
					Rectangle borderRect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
					pe.Graphics.DrawRectangle(pen, borderRect);
				}
			}
		}

		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
			Cursor = Cursors.SizeAll;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Worker.Stop();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			this.Owner.StartImageProcessing();
		}

		private Bitmap CropBitmap(Bitmap sourceBitmap, int x, int y, int width, int height)
		{
			x = Math.Max(0, Math.Min(x, Screen.PrimaryScreen.Bounds.Width - width));
			y = Math.Max(0, Math.Min(y, Screen.PrimaryScreen.Bounds.Height - height));

			Rectangle cropRect = new Rectangle(x, y, width, height);
			Bitmap croppedBitmap = sourceBitmap.Clone(cropRect, sourceBitmap.PixelFormat);
			return croppedBitmap;
		}

		private void HandleResizeN(ResizeHandler sender, bool preventUpdate = false)
		{
			var deltaY = Cursor.Position.Y - this.Top;

			if (this.Height - deltaY <= ResizeHandler.StandardSize * 2)
			{
				return;
			}

			this.Top = Cursor.Position.Y;
			this.Height -= deltaY;

			if (!preventUpdate)
			{
				this.UpdateImage();
			}
		}

		private void HandleResizeS(ResizeHandler sender, bool preventUpdate = false)
		{
			if (sender.Top <= ResizeHandler.StandardSize * 2)
			{
				sender.Top = this.Height - ResizeHandler.StandardSize;
				return;
			}

			this.Height = sender.Bottom;

			if (!preventUpdate)
			{
				this.UpdateImage();
			}
		}

		private void HandleResizeW(ResizeHandler sender, bool preventUpdate = false)
		{
			var deltaX = Cursor.Position.X - this.Left;

			if (this.Width - deltaX <= ResizeHandler.StandardSize * 2)
			{
				return;
			}

			this.Left = Cursor.Position.X;
			this.Width -= deltaX;

			if (!preventUpdate)
			{
				this.UpdateImage();
			}
		}

		private void HandleResizeE(ResizeHandler sender, bool preventUpdate = false)
		{
			if (sender.Left <= ResizeHandler.StandardSize * 2)
			{
				sender.Left = this.Width - ResizeHandler.StandardSize;
				return;
			}

			this.Width = sender.Right;

			if (!preventUpdate)
			{
				this.UpdateImage();
			}
		}
	}
}
