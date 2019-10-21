using System;
using System.Data;
using Dapper;

namespace UoW.Implementation.Dapper.CustomTypeHandlers
{
	internal class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
	{
		private readonly TimeZoneInfo _databaseTimeZone = TimeZoneInfo.Local;

		public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
		{
			parameter.Value = value.ToString();
		}

		public override DateTimeOffset Parse(object value)
		{
			var dt = DateTime.Parse(value.ToString());

			return new DateTimeOffset(dt, _databaseTimeZone.GetUtcOffset(dt));
		}
	}
}