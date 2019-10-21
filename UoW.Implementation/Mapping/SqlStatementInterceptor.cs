using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.SqlCommand;

namespace UoW.Implementation.Mapping
{
	public class SqlStatementInterceptor : EmptyInterceptor
	{
		private readonly ILogger<SqlStatementInterceptor> _logger;

		public SqlStatementInterceptor(ILogger<SqlStatementInterceptor> logger)
		{
			_logger = logger;
		}

		public override SqlString OnPrepareStatement(SqlString sql)
		{
			_logger.LogDebug("Executing sql statement in database. SQL to execute:\n### SQL BEGIN ###\n{sqlStatement}\n### SQL END ###", sql.ToString());
			return sql;
		}

		/// <inheritdoc />
		public override void AfterTransactionBegin(ITransaction tx)

		{
			_logger.LogDebug("Transaction to database started");
			base.AfterTransactionBegin(tx);
		}

		/// <inheritdoc />
		public override void BeforeTransactionCompletion(ITransaction tx)
		{
			_logger.LogDebug("Transaction to database will be completed");
			base.BeforeTransactionCompletion(tx);
		}

		/// <inheritdoc />
		public override void AfterTransactionCompletion(ITransaction tx)
		{
			if (tx == null)
				_logger.LogDebug("Transaction to database has not been started");
			else
				_logger.LogDebug(
					"Transaction to database completed [WasCommitted: {wasCommitted}, WasRolledBack: {wasRolledBack}]",
					tx.WasCommitted, tx.WasRolledBack);

			base.AfterTransactionCompletion(tx);
		}
	}
}