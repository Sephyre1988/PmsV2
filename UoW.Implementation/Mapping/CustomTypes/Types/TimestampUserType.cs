using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace UoW.Implementation.Mapping.CustomTypes.Types
{
	public class TimestampUserType : IUserType, IUserVersionType
	{
		private static readonly string[] Formats = { "yyyyMMddHHmmss", "yyyyMMdd" };

		private readonly TimeZoneInfo _databaseTimeZone = TimeZoneInfo.Local;

		// Other standard interface  implementations omitted ...

		public Type ReturnedType => typeof(DateTimeOffset);

		public bool IsMutable => true;

		public SqlType[] SqlTypes => new[] { new SqlType(DbType.Int64) };

		public new bool Equals(object x, object y)
		{
			return object.Equals(x, y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			var r = rs[names[0]];
			if (r == DBNull.Value)
			{
				return null;
			}

			var storedTime = (long)r;
			try
			{
				return DateTimeOffset.ParseExact(storedTime.ToString(), Formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			if (value == null)
			{
				NHibernateUtil.DateTime.NullSafeSet(cmd, null, index, session);
			}
			else
			{
				var dateTimeOffset = (DateTimeOffset)value;
				var paramVal = dateTimeOffset.ToString("yyyyMMddHHmmss");

				var parameter = (IDataParameter)cmd.Parameters[index];
				parameter.Value = long.Parse(paramVal);
			}
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public object Seed(ISessionImplementor session)
		{
			return DateTimeOffset.Now;
		}

		public object Next(object current, ISessionImplementor session)
		{
			return DateTimeOffset.Now;
		}

		public int Compare(object x, object y)
		{
			return string.Compare(((string)x), (string)y, StringComparison.InvariantCulture);
		}
	}
}