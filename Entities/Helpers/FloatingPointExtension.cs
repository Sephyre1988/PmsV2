using System;
using System.Globalization;
using System.Linq;

namespace Entities.Helpers
{
	public static class FloatingPointExtension
	{
		const string DEFAULT_CULTURE = "en-US";
		const string DEFAULT_NUMBER_DECIMAL_SEPARATOR = ".";

		public static double WithDefaultCultureToDouble(this string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException("Value cannot be empty or whitespace.", nameof(value));
			}

			return Convert.ToDouble(value, new NumberFormatInfo
			{
				NumberDecimalSeparator = DEFAULT_NUMBER_DECIMAL_SEPARATOR
			});
		}

		public static decimal WithDefaultCultureToDecimal(this string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException("Value cannot be empty or whitespace.", nameof(value));
			}

			return Convert.ToDecimal(value, new NumberFormatInfo
			{
				NumberDecimalSeparator = DEFAULT_NUMBER_DECIMAL_SEPARATOR
			});
		}

		public static double? WithDefaultCultureToNullableDouble(this string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			return Convert.ToDouble(value, new NumberFormatInfo
			{
				NumberDecimalSeparator = DEFAULT_NUMBER_DECIMAL_SEPARATOR
			});
		}

		public static decimal? WithDefaultCultureToNullableDecimal(this string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			return Convert.ToDecimal(value, new NumberFormatInfo
			{
				NumberDecimalSeparator = DEFAULT_NUMBER_DECIMAL_SEPARATOR
			});
		}

		public static string ToStringWithDefaultCulture(this double value)
		{
			return value.ToString(CultureInfo.GetCultureInfo(DEFAULT_CULTURE));
		}

		public static string ToStringWithDefaultCulture(this double? value)
		{
			return value?.ToString(CultureInfo.GetCultureInfo(DEFAULT_CULTURE));
		}

		public static string ToStringWithDefaultCulture(this decimal value)
		{
			return value.ToString(CultureInfo.GetCultureInfo(DEFAULT_CULTURE));
		}

		public static string ToStringWithDefaultCulture(this decimal? value)
		{
			return value?.ToString(CultureInfo.GetCultureInfo(DEFAULT_CULTURE));
		}

		/// <summary>
		/// http://stackoverflow.com/questions/4094179/best-way-to-convert-string-to-decimal-separator-and-insensitive-way
		/// </summary>
		public static decimal ToDecimal(this string value)
		{
			var tempValue = value;

			var punctuation = value.Where(char.IsPunctuation).Distinct().ToList();

			switch (punctuation.Count)
			{
				case 0:
					break;
				case 1:
					tempValue = value.Replace(',', '.');
					break;
				case 2:
					if (punctuation[0] == '.')
						tempValue = value.SwapChar('.', ',');
					break;
				default:
					throw new InvalidCastException();
			}

			var number = Convert.ToDecimal(tempValue, new NumberFormatInfo
			{
				NumberDecimalSeparator = DEFAULT_NUMBER_DECIMAL_SEPARATOR
			});
			return number;
		}

		/// <summary>
		/// Swaps every occurence of the specified chars into a given string,
		/// replacing the left char parameter with the right char parameter
		/// and vice-versa.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="left">From.</param>
		/// <param name="right">To.</param>
		/// <returns></returns>
		private static string SwapChar(this string value, char left, char right)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			var result = new char[value.Length];
			for (var i = 0; i < value.Length; i++)
			{
				var c = value[i];
				if (c == left)
					c = right;
				else if (c == right)
					c = left;

				result[i] = c;
			}

			return new string(result);
		}
	}
}