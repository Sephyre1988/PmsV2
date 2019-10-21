using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;


namespace UoW.Implementation.Mapping
{
	public class PmsDatabaseSessionFactory : IDisposable
	{
		private readonly ISessionFactory _sessionFactory;

		public PmsDatabaseSessionFactory(string pmsConnection, ILoggerFactory factory)
		{
			if (pmsConnection == null)
				throw new ArgumentNullException(nameof(pmsConnection));
			if (string.IsNullOrWhiteSpace(pmsConnection))
				throw new ArgumentException("Value cannot be whitespace.", nameof(pmsConnection));


			_sessionFactory =
				Fluently.Configure()
					.Database(
						() => SqlServerManagedDataClientConfiguration.SqlServer.ConnectionString(pmsConnection)
							.AdoNetBatchSize(50)
					).Mappings(m => m.FluentMappings.AddFromAssemblyOf<PmsUnitOfWork>())
					.ExposeConfiguration(SchemaMetadataUpdater.QuoteTableAndColumns)
					.ExposeConfiguration(c => c.SetProperty("command_timeout", "60"))
					.ExposeConfiguration(c => c.SetInterceptor(
						new SqlStatementInterceptor(factory.CreateLogger<SqlStatementInterceptor>())))
					.BuildConfiguration()
					.BuildSessionFactory();
		}

		public IPmsDatabaseSession Create()
		{
			return new PmsDatabaseSession(_sessionFactory.OpenSession());
		}

		public IPmsDatabaseSession Create(PmsSqlConnection connection)
		{
			connection.Init();
			return new PmsDatabaseSession(_sessionFactory.OpenSession(connection.Connection));
		}

		public void Dispose()
		{
			_sessionFactory.Dispose();
		}
	}
}