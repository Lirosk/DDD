using System.Text;

using Newtonsoft.Json.Linq;

namespace ScreenTranslator
{
	public class ScreenTranslatorMediumWorker
	{
		private Bitmap screenshot;
		private Callback callback;
		private Action errorCallback;
		private Task task;
		private CancellationTokenSource cts;

		public delegate void Callback(Bitmap result, List<RecognizedText> recognizedTexts, List<string> translatedTexts);

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

					var jsonContent = new JObject();
					jsonContent["image_bytes"] = Convert.ToBase64String(imageBytes);

					var jsonString = jsonContent.ToString();
					var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

					HttpResponseMessage response = await client.PostAsync(url, stringContent);

					if (response.IsSuccessStatusCode)
					{
						var responseJsonString = await response.Content.ReadAsStringAsync();
						var responseJson = JObject.Parse(responseJsonString);
						var updatedImageBytes = Convert.FromBase64String(responseJson["image_bytes"].ToString());

						List<RecognizedText> recognizedTexts = new();

						JArray texts = (JArray)responseJson["recognized_texts"]!;
						foreach (JToken recognizedTextToken in texts)
						{
							var recognizedTextObject = recognizedTextToken.ToList<Object>()!;
							var text = recognizedTextObject[0].ToString();
							var coordinates = new List<int[]>();

							for (int i = 1; i < 3; i++)
							{
								var coordObj = recognizedTextObject[i];
								var coord = ((JArray)coordObj).ToObject<int[]>();
								coordinates.Add(coord);
							}

							recognizedTexts.Add(new RecognizedText { Text = text, Coordinates = coordinates });
						}

						var translatedTexts = ((JArray)responseJson["translated_texts"]!).Select<JToken, string>((token) => token.ToObject<string>()!).ToList();

						// Update the screenshot with the updated image bytes
						using (MemoryStream updatedMs = new MemoryStream(updatedImageBytes))
						{
							screenshot = new Bitmap(updatedMs);
						}

						this.callback(screenshot, recognizedTexts, translatedTexts);
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
