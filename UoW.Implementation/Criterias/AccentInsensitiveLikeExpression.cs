using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UoW.Implementation.Criterias
{
	public class AccentInsensitiveLikeExpression : AbstractCriterion
	{
		private readonly string _propertyName;
		private readonly object _value;
		private readonly IProjection _projection;

		public AccentInsensitiveLikeExpression(IProjection projection, string value, MatchMode matchMode)
		{
			_projection = projection;
			_value = matchMode.ToMatchString(value);
		}

		public AccentInsensitiveLikeExpression(IProjection projection, object value)
		{
			_projection = projection;
			_value = value;
		}

		public AccentInsensitiveLikeExpression(string propertyName, object value)
		{
			_propertyName = propertyName;
			_value = value;
		}

		public AccentInsensitiveLikeExpression(string propertyName, string value, MatchMode matchMode)
			: this(propertyName, matchMode.ToMatchString(value))
		{
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var columnNames = CriterionUtil.GetColumnNames(_propertyName, _projection, criteriaQuery, criteria);
			var dialect = criteriaQuery.Factory.Dialect;

			if (columnNames.Length != 1)
			{
				throw new HibernateException("accent insensitive like may only be used with single-column properties");
			}

			var parameter = criteriaQuery.NewQueryParameter(GetParameterTypedValue(criteria, criteriaQuery)).Single();
			if (dialect is Oracle8iDialect)
				return GenerateOracleExpression(columnNames[0], dialect, parameter).ToSqlString();

			if (dialect is MsSql2000Dialect)
				return GenerateMsSqlExpression(columnNames[0], parameter).ToSqlString();

			throw new NotSupportedException();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var typedValues = new List<TypedValue>();

			if (_projection != null)
			{
				typedValues.AddRange(_projection.GetTypedValues(criteria, criteriaQuery));
			}
			typedValues.Add(GetParameterTypedValue(criteria, criteriaQuery));

			return typedValues.ToArray();
		}

		public TypedValue GetParameterTypedValue(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var matchValue = _value.ToString().ToLower();
			if (_projection != null)
			{
				return CriterionUtil.GetTypedValues(criteriaQuery, criteria, _projection, null, matchValue).Single();
			}
			return criteriaQuery.GetTypedValue(criteria, _propertyName, matchValue);
		}

		public override IProjection[] GetProjections()
		{
			return _projection != null ? new[] { _projection } : null;
		}

		public override string ToString()
		{
			return (_projection ?? (object)_propertyName) + " ilike " + _value;
		}

		private SqlStringBuilder GenerateOracleExpression(SqlString columnName, Dialect dialect, Parameter parameter)
		{
			var sqlBuilder = new SqlStringBuilder();
			sqlBuilder.Add("translate")
				.Add("(")
				.Add(dialect.LowercaseFunction)
				.Add("(")
				.Add(columnName)
				.Add(")")
				.Add(",")
				.Add("'âàãáéêèíîòóôõüùúûçÿ'")
				.Add(",")
				.Add("'aaaaeeeiioooouuuucy'")
				.Add(")")
				.Add(" like ")
				.Add("translate")
				.Add("(")
				.Add(dialect.LowercaseFunction)
				.Add("(")
				.Add(parameter)
				.Add(")")
				.Add(",")
				.Add("'âàãáéêèíîòóôõüùúûçÿ'")
				.Add(",")
				.Add("'aaaaeeeiioooouuuucy'")
				.Add(")");

			return sqlBuilder;
		}

		private SqlStringBuilder GenerateMsSqlExpression(SqlString columnName, Parameter parameter)
		{
			var sqlBuilder = new SqlStringBuilder();
			sqlBuilder
				.Add(columnName)
				.Add(" like ")
				.Add(parameter)
				.Add(" collate latin1_general_ci_ai ");

			return sqlBuilder;
		}
	}
}
