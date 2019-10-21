using System;
using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace UoW.Implementation.Mapping.CustomTypes.Types
{
	public class NormalizedDateTimeUserType : IUserType
	{
		private readonly TimeZoneInfo _databaseTimeZone = TimeZoneInfo.Local;

		// Other standard interface  implementations omitted ...

		public Type ReturnedType => typeof(DateTimeOffset);

		public bool IsMutable => false;

		public SqlType[] SqlTypes => new[] { new SqlType(DbType.DateTime) };

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

			var storedTime = (DateTime)r;
			return new DateTimeOffset(storedTime, _databaseTimeZone.GetUtcOffset(storedTime));
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
				var parameter = (IDataParameter)cmd.Parameters[index];
				parameter.Value = dateTimeOffset.DateTime;
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

	}
}