using NHibernate.SqlTypes;
using NHibernate.Type;

namespace UoW.Implementation.Mapping.CustomTypes.Types
{
	public class YesNoCharBoolean : CharBooleanType
	{
		public YesNoCharBoolean()
			: base(new AnsiStringFixedLengthSqlType(1))
		{
		}

		protected override string TrueString => "S";

		protected override string FalseString => "N";
	}
}