using System;
using System.Text;

namespace UoW.Implementation.Helpers
{
	public static class RawQueryHelpers
	{
		public static string AsCountQuery(this string sql)
		{
			return string.Concat(@"
SELECT COUNT(*) AS RowCount FROM (
", sql,
@")");
		}

		public static string AsPageQuery(this string sql, int skip, int take)
		{
			return string.Concat(@"
SELECT * FROM ( SELECT PAGEQUERY.*, ROWNUM PAGEROWNUM FROM (
", sql, @"
) PAGEQUERY WHERE ROWNUM <= ", (skip + take).ToString(), @" ) WHERE PAGEROWNUM > ", skip.ToString());
		}

		public static string AsTakeQuery(this string sql, int take)
		{
			return string.Concat(@"
SELECT PAGEQUERY.*, ROWNUM PAGEROWNUM FROM (
", sql, @"
) PAGEQUERY WHERE ROWNUM <= ", take.ToString());
		}

		public static string FromDual(this string sql)
		{
			return string.Concat(@"
SELECT 
    ", sql, @" FRESULT
FROM DUAL");
		}

		public static string InsideBeginEndBlock(this string sql)
		{
			return string.Concat(@"
BEGIN 
    ", sql, @";
END;");
		}

		/// <summary>
		/// Creates a <see cref="string"/> representing a group of "column LIKE parameter" conditions to apply as a filter.
		/// </summary>
		/// <param name="filterTarget">The target of the filter (may be a column name or a value derived from multiple one or multiple columns).</param>
		/// <param name="parameterNamePrefix">The prefix to be used for creating the parameter names.</param>
		/// <param name="filterText">The text to use as filter.</param>
		/// <param name="parameterAppender">A <see cref="Action{string, string}"/> that allows this method to setup the required parameters.</param>
		/// <param name="conditionCombiningOperator">Operator to be used for combining multiple "LIKE" conditions if applicable. Default is "AND".</param>
		/// <returns>A <see cref="string"/> containing the conditions required to perform the filter.</returns>
		public static string CreateTextLikeCondition(
			string filterTarget, string parameterNamePrefix, string filterText, Action<string, string> parameterAppender, string conditionCombiningOperator = "AND")
		{
			if (string.IsNullOrWhiteSpace(filterText))
			{
				return string.Empty;
			}
			if (string.IsNullOrWhiteSpace(filterTarget))
			{
				throw new ArgumentNullException(nameof(filterTarget), "Must provide a column to filter.");
			}
			if (string.IsNullOrWhiteSpace(parameterNamePrefix))
			{
				throw new ArgumentNullException(nameof(parameterNamePrefix), "Must provide a prefix for the parameters.");
			}
			if (parameterAppender == null)
			{
				throw new ArgumentNullException(nameof(parameterAppender), "Must provide a way to setup the parameters.");
			}
			if (string.IsNullOrWhiteSpace(conditionCombiningOperator))
			{
				throw new ArgumentNullException(nameof(conditionCombiningOperator), "Must provide an operator for combining the conditions (or allow the default to be used).");
			}

			const string WhereClauseTranslateFormat = "Translate(Upper({0}), 'âàãáéêèíîòóôõüùúûçÿ', 'aaaaeeeiioooouuuucy')";

			var keywordCount = 0;

			var searchWords = filterText.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
			int numSearchWords = searchWords.Length;

			var conditionBuilder = new StringBuilder();
			conditionBuilder.Append("(");

			foreach (var keyword in searchWords)
			{
				var parameterName = parameterNamePrefix + (keywordCount++);
				parameterAppender(parameterName, $"%{keyword}%");

				conditionBuilder.Append("(");
				conditionBuilder.AppendFormat(WhereClauseTranslateFormat, filterTarget);
				conditionBuilder.Append(" LIKE ");
				conditionBuilder.AppendFormat(WhereClauseTranslateFormat, ":" + parameterName);
				conditionBuilder.Append(")");

				if (keywordCount < numSearchWords)
				{
					conditionBuilder.AppendFormat(" {0} ", conditionCombiningOperator);
				}
			}
			conditionBuilder.Append(")");

			return conditionBuilder.ToString();
		}
	}
}