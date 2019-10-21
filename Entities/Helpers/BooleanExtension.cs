using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Helpers
{
	public static class BooleanExtension
	{
		public static string ToBooleanString(this bool value)
		{
			return value.ToString().ToLower();
		}

		public static string ToPortugueseYesNoInitialString(this bool value)
		{
			return value ? "S" : "N";
		}

		public static bool ToBoolFromPortugueseYesNoInitialString(this string value)
		{
			return (value == "S");
		}

		public static int ToBooleanInt(this bool value)
		{
			return value ? 1 : 0;
		}

	}
}
