using System;
using System.Data.Common;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using Newtonsoft.Json;
using NHibernate.Engine;

namespace UoW.Implementation.Mapping.CustomTypes.Types
{
	//https://gist.github.com/phillip-haydon/1936188
	[Serializable]
	public class BlobbedJson<T> : IUserType where T : class
	{
		public new bool Equals(object x, object y)
		{
			if (x == null && y == null)
				return true;

			if (x == null || y == null)
				return false;

			var xdocX = JsonConvert.SerializeObject(x);
			var xdocY = JsonConvert.SerializeObject(y);

			return xdocY == xdocX;
		}

		public int GetHashCode(object x)
		{
			if (x == null)
				return 0;

			return x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			if (names.Length != 1)
				throw new InvalidOperationException("Only expecting one column...");

			var val = rs[names[0]] as string;

			if (!string.IsNullOrWhiteSpace(val))
			{
				return JsonConvert.DeserializeObject<T>(val);
			}

			return null;
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var parameter = cmd.Parameters[index];

			if (value == null)
			{
				parameter.Value = DBNull.Value;
			}
			else
			{
				parameter.Value = JsonConvert.SerializeObject(value);
			}
		}

		public object DeepCopy(object value)
		{
			if (value == null)
				return null;

			//Serialized and Deserialized using json.net so that I don't
			//have to mark the class as serializable. Most likely slower
			//but only done for convenience. 

			var serialized = JsonConvert.SerializeObject(value);

			return JsonConvert.DeserializeObject<T>(serialized);
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			var str = cached as string;

			if (string.IsNullOrWhiteSpace(str))
				return null;

			return JsonConvert.DeserializeObject<T>(str);
		}

		public object Disassemble(object value)
		{
			if (value == null)
				return null;

			return JsonConvert.SerializeObject(value);
		}

		public SqlType[] SqlTypes => new SqlType[] { new StringClobSqlType() };

		public Type ReturnedType => typeof(T);

		public bool IsMutable => true;
	}
}