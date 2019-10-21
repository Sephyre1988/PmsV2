using NHibernate.SqlTypes;
using NHibernate.Type;

namespace UoW.Implementation.Mapping.CustomTypes.Types
{
	public class ActiveInactiveBoolean : CharBooleanType
	{
		public ActiveInactiveBoolean()
			: base(new AnsiStringFixedLengthSqlType(1))
		{
		}

		protected override string TrueString => "A";

		protected override string FalseString => "I";
	}
}