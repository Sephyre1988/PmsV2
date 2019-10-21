using System;
using System.Text;

namespace UoW.Implementation.Helpers
{
	public static class StringExtensions
	{
		private static string Translate(this string text)
		{
			if (text == null)
				return null;

			if (text.Length == 0)
				return string.Empty;

			var normalizedCharacters = new char[text.Length];
			for (var i = 0; i < text.Length; i++)
			{
				//  optimized code
				char c;
				switch (text[i])
				{
					case 'â':
					case 'à':
					case 'ã':
					case 'á':
					case 'ä':
					case 'Â':
					case 'À':
					case 'Ã':
					case 'Á':
					case 'Ä':
						c = 'A';
						break;
					case 'é':
					case 'ê':
					case 'è':
					case 'ë':
					case 'É':
					case 'Ê':
					case 'È':
					case 'Ë':
						c = 'E';
						break;
					case 'í':
					case 'î':
					case 'ì':
					case 'ï':
					case 'Í':
					case 'Î':
					case 'Ì':
					case 'Ï':
						c = 'I';
						break;
					case 'ó':
					case 'ô':
					case 'ò':
					case 'õ':
					case 'ö':
					case 'Ó':
					case 'Ô':
					case 'Ò':
					case 'Õ':
					case 'Ö':
						c = 'O';
						break;
					case 'ü':
					case 'ù':
					case 'ú':
					case 'û':
					case 'Ü':
					case 'Ù':
					case 'Ú':
					case 'Û':
						c = 'U';
						break;
					case 'ç':
					case 'Ç':
						c = 'C';
						break;
					case 'ÿ':
					case 'Ÿ':
						c = 'Y';
						break;
					default:
						c = char.ToUpperInvariant(text[i]);
						break;
				}

				normalizedCharacters[i] = c;
			}

			return new string(normalizedCharacters);
		}

		public static string TranslateAsFilter(this string text)
		{
			if (text == null)
				return null;

			if (string.IsNullOrWhiteSpace(text))
				return "%%";

			var sb = new StringBuilder(text.Length + 2);
			sb.Append("%");

			foreach (var keyword in text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
			{
				sb.Append(keyword.Translate());
				sb.Append("%");
			}

			return sb.ToString();
		}
	}
}