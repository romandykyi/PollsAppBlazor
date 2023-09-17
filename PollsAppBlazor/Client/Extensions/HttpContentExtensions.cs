using System.Text.Json;

namespace PollsAppBlazor.Client.Extensions
{
	public static class HttpContentExtensions
	{
		/// <summary>
		/// Extract errors from HTTP content.
		/// </summary>
		/// <remarks>
		/// Keys "" and "errors" are looked
		/// </remarks>
		/// <returns>
		/// Found errors, or "An unexpected error has occured" if no errors were found
		/// </returns>
		public static async IAsyncEnumerable<string> ExtractErrorsAsync(this HttpContent content)
		{
			using var stream = await content.ReadAsStreamAsync();
			using var document = await JsonDocument.ParseAsync(stream);

			if (!document.RootElement.TryGetProperty("errors", out var errorsElement))
			{
				errorsElement = document.RootElement;
			}

			foreach (var item in errorsElement.EnumerateObject())
			{
				if (item.Value.ValueKind == JsonValueKind.Array)
				{
					foreach (var error in item.Value.EnumerateArray())
					{
						string? s = error.GetString();
						if (s != null) yield return s;
					}
				}
				else
				{
					string? s = item.Value.GetString();
					if (s != null) yield return s;
				}
			}
		}
	}
}
