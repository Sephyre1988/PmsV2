using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using NHibernate;
using UoW.Implementation.Dapper.CustomTypeHandlers;
using UoW.Implementation.Helpers;
using UoW.Implementation.Mapping;

namespace UoW.Implementation
{
	public class PmsQueryRunner
	{
		private readonly IPmsDatabaseSession _databaseSession;
		private readonly PmsXmlSerializer _xmlSerializer;
		private readonly ILogger<PmsQueryRunner> _logger;

		public PmsQueryRunner(
			IPmsDatabaseSession databaseSession, PmsXmlSerializer xmlSerializer, ILogger<PmsQueryRunner> logger)
		{
			if (databaseSession == null)
				throw new ArgumentNullException(nameof(databaseSession));
			if (xmlSerializer == null)
				throw new ArgumentNullException(nameof(xmlSerializer));
			if (logger == null)
				throw new ArgumentNullException(nameof(logger));

			_databaseSession = databaseSession;
			_xmlSerializer = xmlSerializer;
			_logger = logger;
		}

		static PmsQueryRunner()
		{
			SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
		}

		#region QueryAsync

		public async Task<IReadOnlyCollection<T>> QueryAsync<T>(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			using (var scope = BuildCommandScope(sql, param, commandTimeout, commandType, ct))
				return ToReadOnlyCollection(await _databaseSession.Session.Connection.QueryAsync<T>(scope.Command));
		}

		public async Task<IReadOnlyCollection<dynamic>> QueryAsync(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			using (var scope = BuildCommandScope(sql, param, commandTimeout, commandType, ct))
				return ToReadOnlyCollection(await _databaseSession.Session.Connection.QueryAsync(scope.Command));
		}

		#endregion

		#region QuerySingleAsync

		public async Task<T> QuerySingleAsync<T>(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			return (await QueryAsync<T>(ct, sql, param, commandTimeout, commandType)).Single();
		}

		#endregion

		#region QuerySingleOrDefaultAsync

		public async Task<T> QuerySingleOrDefaultAsync<T>(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			return (await QueryAsync<T>(ct, sql, param, commandTimeout, commandType)).SingleOrDefault();
		}

		#endregion

		#region QueryFirstOrDefaultAsync

		public async Task<T> QueryFirstOrDefaultAsync<T>(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			return (await QueryAsync<T>(ct, sql, param, commandTimeout, commandType)).FirstOrDefault();
		}

		#endregion

		#region QueryFirstAsync

		public async Task<T> QueryFirstAsync<T>(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null, CommandType? commandType = null)
		{
			return (await QueryAsync<T>(ct, sql, param, commandTimeout, commandType)).First();
		}

		#endregion

		#region ExecuteAsync

		public async Task<int> ExecuteAsync(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			using (var scope = BuildCommandScope(sql, param, commandTimeout, commandType, ct))
				return await _databaseSession.Session.Connection.ExecuteAsync(scope.Command);
		}

		#endregion

		#region ExecuteScalarAsync

		public async Task<T> ExecuteScalarAsync<T>(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			using (var scope = BuildCommandScope(sql, param, commandTimeout, commandType, ct))
				return await _databaseSession.Session.Connection.ExecuteScalarAsync<T>(scope.Command);
		}

		#endregion

		#region QueryMultiple

		public async Task<SqlMapper.GridReader> QueryMultipleAsync(
			CancellationToken ct, string sql, object param = null,
			int? commandTimeout = null,
			CommandType? commandType = null)
		{
			using (var scope = BuildCommandScope(sql, param, commandTimeout, commandType, ct))
				return await _databaseSession.Session.Connection.QueryMultipleAsync(scope.Command);
		}

		#endregion

		#region ExecuteProcedureAsync

		public async Task ExecuteProcedureAsync(
			CancellationToken ct, string procedureSql, object input, int? commandTimeout = null)
		{
			if (procedureSql == null)
				throw new ArgumentNullException(nameof(procedureSql));
			if (input == null)
				throw new ArgumentNullException(nameof(input));
			if (string.IsNullOrWhiteSpace(procedureSql))
				throw new ArgumentException("Value cannot be whitespace.", nameof(procedureSql));

			await ExecuteAsync(
				ct, procedureSql.InsideBeginEndBlock(), input, commandTimeout, CommandType.Text);
		}

		#endregion

		#region ExecuteFunctionAsync

		public async Task<T> ExecuteFunctionAsync<T>(
			CancellationToken ct, string functionSql, object input = null, int? commandTimeout = null)
		{
			if (functionSql == null)
				throw new ArgumentNullException(nameof(functionSql));
			if (string.IsNullOrWhiteSpace(functionSql))
				throw new ArgumentException("Value cannot be whitespace.", nameof(functionSql));

			var result = await QuerySingleAsync<T>(
				ct, functionSql.FromDual(), input, commandTimeout);

			return result;
		}

		#endregion

		#region Helpers

		private static IReadOnlyCollection<T> ToReadOnlyCollection<T>(IEnumerable<T> items) =>
			items as IReadOnlyCollection<T> ?? items.ToList();

		private CommandScope BuildCommandScope(
			string sql, object parameters, int? timeout, CommandType? commandType, CancellationToken ct) =>
			new CommandScope(_logger, _databaseSession.Session, sql, parameters, timeout, commandType, ct);

		private class CommandScope : IDisposable
		{
			private readonly DateTimeOffset _createdOn = DateTimeOffset.Now;
			private readonly ILogger<PmsQueryRunner> _logger;
			private CancellationTokenSource _cts;
			private bool _disposed;

			public CommandScope(
				ILogger<PmsQueryRunner> logger, ISession session,
				string sql, object parameters, int? timeout,
				CommandType? commandType, CancellationToken ct)
			{
				_logger = logger;
				_cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
				_disposed = false;

				IDbTransaction tx;
				using (var cmd = session.Connection.CreateCommand())
				{
					session.Transaction.Enlist(cmd);
					tx = cmd.Transaction;
				}

				var commandTimeout = timeout ?? session.Connection.ConnectionTimeout;

				Command = new CommandDefinition(
					sql, parameters, tx, commandTimeout,
					commandType, CommandFlags.Buffered, _cts.Token);

				if (_logger.IsEnabled(LogLevel.Debug))
					_logger.LogDebug(@"
SQL command scope created [Transaction:{isInTransaction} Timeout:{commandTimeout}]
{sqlStatement}", tx != null, commandTimeout, sql);
			}

			public CommandDefinition Command { get; }

			public void Dispose()
			{
				if (_disposed)
					return;

				_cts.Dispose();
				_cts = null;
				_disposed = true;

				if (_logger.IsEnabled(LogLevel.Debug))
					_logger.LogDebug(
						"SQL command scope disposed after {disposeTime}ms",
						(DateTimeOffset.Now - _createdOn).TotalMilliseconds);
			}
		}

		#endregion

	}
}