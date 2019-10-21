using System;

namespace UoW.Implementation.Helpers
{
	public static class TryParseExtensions
	{
		private static readonly TimeZoneInfo DatabaseTimeZone = TimeZoneInfo.Local;

		public static int TryParseToInt(this object obj)
		{
			var res = default(int);
			if (obj == null) return res;
			int.TryParse(obj.ToString(), out res);
			return res;
		}
		public static decimal TryParseToDecimal(this object obj)
		{
			var res = default(decimal);
			if (obj == null) return res;
			decimal.TryParse(obj.ToString(), out res);
			return res;
		}

		public static DateTime TryParseToDateTime(this object obj)
		{
			var res = default(DateTime);
			if (obj == null) return res;
			DateTime.TryParse(obj.ToString(), out res);
			return res;
		}

		public static DateTimeOffset ParseDateTimeAdjustedWithDefaultOffset(this object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			var result = DateTime.Parse(obj.ToString());
			return new DateTimeOffset(result, DatabaseTimeZone.GetUtcOffset(result));
		}

		public static bool TryParseActiveInactiveToBoolean(this string strValue)
		{
			return strValue == "ACTIVO";
		}
	}
}