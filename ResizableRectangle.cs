namespace ScreenTranslator
{
	internal class ResizableRectangle : Control
	{
		private Rectangle rectangle;
		private RectangleHandle activeHandle;

		private const int HandleSize = 10;
		private const int HandleOffset = HandleSize / 2;

		public bool IsResizing { get; private set; }

		public ResizableRectangle()
		{
			SetStyle(ControlStyles.ResizeRedraw, true);
			Cursor = Cursors.SizeAll;
			this.Location = new Point(500, 500);
			this.Size = new Size(750, 500);
		}

		public void StartResize(Point startPoint)
		{
			IsResizing = true;
			activeHandle = GetHandle(startPoint);
		}

		public void UpdateResize(Point mouseDownLocation, Point endPoint)
		{
			if (IsResizing)
			{
				if (activeHandle != RectangleHandle.None)
				{
					// Adjust the rectangle size based on the active handle
					switch (activeHandle)
					{
						case RectangleHandle.TopLeft:
							rectangle = new Rectangle(endPoint.X, endPoint.Y, rectangle.Right - endPoint.X, rectangle.Bottom - endPoint.Y);
							break;
						case RectangleHandle.TopRight:
							rectangle = new Rectangle(rectangle.Left, endPoint.Y, endPoint.X - rectangle.Left, rectangle.Bottom - endPoint.Y);
							break;
						case RectangleHandle.BottomRight:
							rectangle = new Rectangle(rectangle.Left, rectangle.Top, endPoint.X - rectangle.Left, endPoint.Y - rectangle.Top);
							break;
						case RectangleHandle.BottomLeft:
							rectangle = new Rectangle(endPoint.X, rectangle.Top, rectangle.Right - endPoint.X, endPoint.Y - rectangle.Top);
							break;
					}

					Invalidate();
				}
				else
				{
					// Move the rectangle based on the mouse movement
					rectangle.Location = new Point(rectangle.Left + (endPoint.X - rectangle.X), rectangle.Top + (endPoint.Y - rectangle.Y));
					System.Console.WriteLine($"{endPoint.X} -> {mouseDownLocation.X}");

					Invalidate();
				}
			}
		}

		public void EndResize()
		{
			IsResizing = false;
			activeHandle = RectangleHandle.None;
		}

		public void Draw(Graphics graphics)
		{
			// Draw the rectangle
			graphics.DrawRectangle(Pens.Red, rectangle);

			// Draw the resizable handles
			DrawHandle(graphics, RectangleHandle.TopLeft);
			DrawHandle(graphics, RectangleHandle.TopRight);
			DrawHandle(graphics, RectangleHandle.BottomRight);
			DrawHandle(graphics, RectangleHandle.BottomLeft);
		}

		private void DrawHandle(Graphics graphics, RectangleHandle handle)
		{
			Rectangle handleRect = GetHandleRectangle(handle);
			graphics.FillRectangle(Brushes.Red, handleRect);
		}

		private Rectangle GetHandleRectangle(RectangleHandle handle)
		{
			int x = 0, y = 0;
			switch (handle)
			{
				case RectangleHandle.TopLeft:
					x = rectangle.Left - HandleOffset;
					y = rectangle.Top - HandleOffset;
					break;
				case RectangleHandle.TopRight:
					x = rectangle.Right - HandleOffset;
					y = rectangle.Top - HandleOffset;
					break;
				case RectangleHandle.BottomRight:
					x = rectangle.Right - HandleOffset;
					y = rectangle.Bottom - HandleOffset;
					break;
				case RectangleHandle.BottomLeft:
					x = rectangle.Left - HandleOffset;
					y = rectangle.Bottom - HandleOffset;
					break;
			}

			return new Rectangle(x, y, HandleSize, HandleSize);
		}

		private RectangleHandle GetHandle(Point point)
		{
			if (GetHandleRectangle(RectangleHandle.TopLeft).Contains(point))
				return RectangleHandle.TopLeft;
			if (GetHandleRectangle(RectangleHandle.TopRight).Contains(point))
				return RectangleHandle.TopRight;
			if (GetHandleRectangle(RectangleHandle.BottomRight).Contains(point))
				return RectangleHandle.BottomRight;
			if (GetHandleRectangle(RectangleHandle.BottomLeft).Contains(point))
				return RectangleHandle.BottomLeft;

			return RectangleHandle.None;
		}
	}

}
