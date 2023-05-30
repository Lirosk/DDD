namespace ScreenTranslator
{
	public class ScreenTranslatorMediumWorker
	{
		private Bitmap screenshot;
		private Callback callback;
		private Action errorCallback;
		private Task task;
		private CancellationTokenSource cts;

		public delegate void Callback(Bitmap result);

		public void Start(Bitmap screenshot, Callback callback, Action errorCallback)
		{
			this.Stop();

			this.callback = callback!;
			this.errorCallback = errorCallback;

			this.screenshot = screenshot!;
			this.cts = new CancellationTokenSource();

			task = Task.Run(StartWork, cts.Token);
		}

		public void Stop()
		{
			this.cts?.Cancel();
		}

		private async void StartWork()
		{
			byte[] imageBytes;
			using (MemoryStream ms = new MemoryStream())
			{
				this.screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				imageBytes = ms.ToArray();
			}

			using (HttpClient client = new HttpClient())
			{
				try
				{
					string url = "http://127.0.0.1:8000/main/";

					ByteArrayContent content = new ByteArrayContent(imageBytes);
					content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
					HttpResponseMessage response = await client.PostAsync(url, content);

					if (response.IsSuccessStatusCode)
					{
						imageBytes = await response.Content.ReadAsByteArrayAsync();

						using (MemoryStream ms = new(imageBytes))
						{
							screenshot = new(ms);
						}

						this.callback(screenshot);
					}
					else
					{
						throw new Exception();
					}
				}
				catch (Exception)
				{
					this.errorCallback();
				}
			}
		}
	}
}
