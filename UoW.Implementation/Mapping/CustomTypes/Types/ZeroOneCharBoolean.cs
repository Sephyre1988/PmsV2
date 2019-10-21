using NHibernate.SqlTypes;
using NHibernate.Type;

namespace UoW.Implementation.Mapping.CustomTypes.Types
{
	public class ZeroOneCharBoolean : CharBooleanType
	{
		public ZeroOneCharBoolean()
			: base(new AnsiStringFixedLengthSqlType(1))
		{
		}

		protected override string TrueString => "1";

		protected override string FalseString => "0";
	}
}