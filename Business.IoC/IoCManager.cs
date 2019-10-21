using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using SimpleSOAPClient.Handlers;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient;
using SimpleExceptionHandling;
using SimplePersistence.UoW.Helper;
using System.IO;
using Business.Contracts.Managers;
using Business.Contracts.Services;
using Business.IoC.Factories;
using Configurations;
using UoW;
using UoW.Implementation;
using UoW.Implementation.Mapping;

namespace Business.IoC
{
	public static class IoCManager
	{
		public static IServiceCollection AddPmsBusiness(this IServiceCollection services,
			string pmsConnectionString, string pmsSecurityConnectionString, string pmsCacheConnectionString)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (pmsConnectionString == null)
				throw new ArgumentNullException(nameof(pmsConnectionString));
			if (pmsSecurityConnectionString == null)
				throw new ArgumentNullException(nameof(pmsSecurityConnectionString));
			if (pmsCacheConnectionString == null)
				throw new ArgumentNullException(nameof(pmsCacheConnectionString));
			if (string.IsNullOrWhiteSpace(pmsConnectionString))
				throw new ArgumentException("Value cannot be whitespace.", nameof(pmsConnectionString));
			if (string.IsNullOrWhiteSpace(pmsSecurityConnectionString))
				throw new ArgumentException("Value cannot be whitespace.", nameof(pmsSecurityConnectionString));
			if (string.IsNullOrWhiteSpace(pmsCacheConnectionString))
				throw new ArgumentException("Value cannot be whitespace.", nameof(pmsCacheConnectionString));

			return services
				.AddMemoryCache()
				.AddEntityFramework()
				.AddEntityFrameworkSqlServer()
				.AddDatabaseUnitOfWork<PmsUnitOfWork, IPmsDatabaseSession>(
					k => k.GetRequiredService<PmsDatabaseSessionFactory>()
						.Create(k.GetRequiredService<PmsSqlServerConnection>()),
					s =>
					{
						s.AddSingleton<PmsXmlSerializer>();
						s.AddSingleton(k =>
							new PmsDatabaseSessionFactory(
								pmsConnectionString, k.GetRequiredService<ILoggerFactory>()));
						s.AddSingleton<IPmsUnitOfWorkFactory, PmsUnitOfWorkFactory>();
						s.AddScoped(k => new PmsSqlServerConnection(pmsConnectionString));
						s.AddScoped<PmsQueryRunner>();
					})
				.AddDatabaseUnitOfWork<PmsSecurityUnitOfWork, PmsSecurityContext>(extraCfg: s =>
				{
					s.AddDbContext<PmsSecurityContext>(options =>
					{
						options.UseSqlServer(pmsSecurityConnectionString);
					}).AddScoped<PmsSecurityQueryRunner>();
				})
				.AddDatabaseUnitOfWork<PmsCacheScopedUnitOfWork, PmsCacheContext>(extraCfg: s =>
				{
					s.AddDbContext<PmsCacheContext>(options => { options.UseSqlServer(pmsCacheConnectionString); })
						.AddScoped<IPmsCacheSqlRunner, PmsCacheSqlRunner>();
				})
				.AddSingleton<IPmsCacheUnitOfWorkFactory, PmsCacheUnitOfWorkFactory>()
				.AddDatabaseUnitOfWorkFactory()
				.AddPmsBusinessManagers()
				.AddPmsConfigurationManager()
				.AddPmsManagers()
				.AddPmsBusinessBrokers()
				.AddPmsBusinessValidators()
				.AddPmsBusinessServices();
		}

		private static IServiceCollection AddPmsConfigurationManager(this IServiceCollection services)
		{
			services.Scan(s => s.FromAssemblyOf<ConfigurationsRoot>()
				.AddClasses(classes =>
					classes.Where(e =>
						e.IsClass && !e.IsAbstract && e.Name.EndsWith("Config", StringComparison.Ordinal)
					))
				.AsImplementedInterfaces()
				.WithSingletonLifetime());

			return services;
		}

		private static IServiceCollection AddPmsBusinessManagers(this IServiceCollection services)
		{
			services.Scan(scan => scan.FromAssemblyOf<ConfigurationManager>()
				.AddClasses(classes => classes
					.AssignableTo<IManager>())
				.AsImplementedInterfaces()
				.WithScopedLifetime());

			return services;
		}

		private static IServiceCollection AddPmsBusinessBrokers(this IServiceCollection services)
		{
			//Add Singletons Of each service broker interface
			services.AddApiBrokerFactory();
			return services;
		}

		private static IServiceCollection AddPmsBusinessValidators(this IServiceCollection services)
		{
			services.AddSingleton<IValidatorFactory, ValidatorFactory>();

			services.Scan(scan => scan.FromAssemblyOf<PmsValidator>()
				.AddClasses(classes => classes
					.AssignableTo<IValidator>().Where(t => !t.IsAbstract))
				.AsImplementedInterfaces()
				.WithSingletonLifetime());

			return services;
		}

		private static IServiceCollection AddPmsBusinessServices(this IServiceCollection services)
		{
			services.Scan(scan => scan.FromAssemblyOf<PmsService>()
				.AddClasses(classes => classes
					.AssignableTo<IService>())
				.AsImplementedInterfaces()
				.WithScopedLifetime());

			return services;
		}

		private static TBroker AddRequestExceptionHandling<TBroker>(this TBroker broker, ILogger logger)
		   where TBroker : IApiBroker
		{
			var handlingConfiguration = Handling.Prepare<string, Exception>()
				.On<UnprocessableEntityApiResponseException>((ex) =>
				{
					logger.LogWarning(0, ex,
						"HTTP REST Outbound Response -> Converted into UnprocessableEntityApiResponseException");

					var errors = ex.Error?.Error?.ModelState != null
						? ex.Error.Error.ModelState.Select(
							error => new ValidationExceptionError(error.Key, error.Value))
						: Enumerable.Empty<ValidationExceptionError>();

					return Handling.Handled(new Common.Exceptions.ValidationException(ex, errors));
				})
				.On<BadRequestApiResponseException>((ex) =>
				{
					logger.LogWarning(0, ex,
						"HTTP REST Outbound Response -> Converted into BadRequestApiResponseException");

					if (ex.Error?.Error?.ModelState == null)
					{
						return Handling.Handled(new BusinessException(ex.Message, ex));
					}

					if (ex.Error.Error.ModelState.Count == 1 && ex.Error.Error.ModelState.First().Key == string.Empty)
					{
						//Número de cartão recebido não é válido [ErrorCode: 2270]
						return Handling.Handled(
							new BusinessException(string.Join(",", ex.Error.Error.ModelState.First().Value[0]), ex));
					}

					return Handling.Handled(new BusinessException.WithKeyCollection(ex.Message, ex,
						ex.Error.Error.ModelState
							.Select(o =>
								new ExceptionWithKeyCollectionItem(!string.IsNullOrEmpty(o.Key) ? o.Key : ex.Message,
									string.Join(",", o.Value))))
					);

				}).On<ConflictApiResponseException>((ex, i) =>
				{
					logger.LogWarning(0, ex,
						"HTTP REST Outbound Response -> Converted into ConflictApiResponseException");

					if (ex.Error?.Error?.ModelState == null)
					{
						return Handling.Handled(new BusinessException(ex.Message, ex));
					}

					if (ex.Error.Error.ModelState.Count == 0 && !string.IsNullOrEmpty(ex.Error.Error.Message))
					{
						//O movimento não pode ser anulado pois foi feito em contexto familiar diferente
						return Handling.Handled(new BusinessException(ex.Error.Error.Message, ex));
					}

					return Handling.Handled(new BusinessException.WithKeyCollection(ex.Message, ex,
						ex.Error.Error.ModelState
							.Select(o =>
								new ExceptionWithKeyCollectionItem(!string.IsNullOrEmpty(o.Key) ? o.Key : ex.Message,
									string.Join(",", o.Value))))
					);
				})
				.On<InternalServerErrorApiResponseException>((ex, i) =>
				{
					logger.LogWarning(0, ex,
						"HTTP REST Outbound Response -> Converted into GenericException");

					return Handling.Handled(new GenericException(
						"Ocorreu um erro na comunicação com o servidor. Por favor tente novamente.", ex));
				});

			return broker.OnRequestException((e, r, ct) =>
			{
				logger.LogWarning(0, e, "HTTP REST Outbound Request -> Failed");
				return Task.CompletedTask;
			})
			.OnApiResponseException((e, ct) =>
			{
				var result = handlingConfiguration.Catch(e, throwIfNotHandled: false);
				if (result.Handled)
					throw result.Result;


				logger.LogWarning(0, e, "HTTP REST Outbound Response -> Converted into ApiResponseException");
				return Task.CompletedTask;
			});
		}

		private static TBroker AddServicesExceptionHandling<TBroker>(this TBroker broker, ILogger logger, string applicationName)
			where TBroker : IApiBroker
		{
			var handlingApiRequestConfiguration = Handling.Prepare<string, Exception>()
				.On<TimeoutException>((ex, i) =>
				{
					logger.LogError(0, ex, "HTTP REST Outbound Request -> TimeoutException");

					return Handling.Handled(new GenericException("Não foi possível comunicar com o Saúda. Demasiado tempo a aguardar resposta", ex));
				})
				.On<HttpRequestException>((ex, i) =>
				{
					logger.LogError(0, ex, "HTTP REST Outbound Request -> System.Net.Http.HttpRequestException");

					return Handling.Handled(new GenericException("Não foi possível comunicar com o Saúda.", ex));
				});

			var handlingApiResponseConfiguration = Handling.Prepare<string, Exception>()
				.On<UnprocessableEntityApiResponseException>((ex) =>
				{
					logger.LogError(0, ex,
						"HTTP REST Outbound Response -> Converted into UnprocessableEntityApiResponseException");

					var errors = ex.Error?.Error?.ModelState != null
						? ex.Error.Error.ModelState.Select(
							error => new ValidationExceptionError(error.Key, error.Value))
						: Enumerable.Empty<ValidationExceptionError>();

					return Handling.Handled(new Common.Exceptions.ValidationException(ex, errors));
				})
				.On<BadRequestApiResponseException>((ex) =>
				{
					logger.LogError(0, ex,
						"HTTP REST Outbound Response -> Converted into BadRequestApiResponseException");

					if (ex.Error?.Error?.ModelState == null)
					{
						return Handling.Handled(new BusinessException(ex.Message, ex));
					}

					if (ex.Error.Error.ModelState.Count == 1 && ex.Error.Error.ModelState.First().Key == string.Empty)
					{
						//Número de cartão recebido não é válido [ErrorCode: 2270]
						return Handling.Handled(
							new BusinessException(string.Join(",", ex.Error.Error.ModelState.First().Value[0]), ex));
					}

					return Handling.Handled(new BusinessException.WithKeyCollection(ex.Message, ex,
						ex.Error.Error.ModelState
							.Select(o =>
								new ExceptionWithKeyCollectionItem(!string.IsNullOrEmpty(o.Key) ? o.Key : ex.Message,
									string.Join(",", o.Value))))
					);

				})
				.On<ConflictApiResponseException>((ex, i) =>
				{
					logger.LogWarning(0, ex,
						"HTTP REST Outbound Response -> Converted into ConflictApiResponseException");

					if (ex.Error?.Error?.ModelState == null)
					{
						return Handling.Handled(new BusinessException(ex.Message, ex));
					}

					if (ex.Error.Error.ModelState.Count == 0 && !string.IsNullOrEmpty(ex.Error.Error.Message))
					{
						//O movimento não pode ser anulado pois foi feito em contexto familiar diferente
						return Handling.Handled(new BusinessException(ex.Error.Error.Message, ex));
					}

					return Handling.Handled(new BusinessException.WithKeyCollection(ex.Message, ex,
						ex.Error.Error.ModelState
							.Select(o =>
								new ExceptionWithKeyCollectionItem(!string.IsNullOrEmpty(o.Key) ? o.Key : ex.Message,
									string.Join(",", o.Value))))
					);
				})
				.On<InternalServerErrorApiResponseException>((ex, i) =>
				{
					logger.LogError(0, ex,
						"HTTP REST Outbound Response -> Converted into GenericException");

					return Handling.Handled(new GenericException(
						"Ocorreu um erro na comunicação com o servidor. Por favor tente novamente.", ex));
				});

			return broker.OnRequestException((e, r, ct) =>
			{
				var result = handlingApiRequestConfiguration.Catch(e, applicationName, false);
				if (result.Handled)
					throw result.Result;

				logger.LogError(0, e,
					"HTTP REST Outbound Response -> Converted into GenericException");
				throw new GenericException(
					$"Erro a comunicar com o servidor {applicationName}. Contacte o administrador do sistema.", e);
			})
			.OnApiResponseException((e, ct) =>
			{
				var result = handlingApiResponseConfiguration.Catch(e, applicationName, false);
				if (result.Handled)
					throw result.Result;

				logger.LogError(0, e,
					"HTTP REST Outbound Response -> Converted into GenericException");
				throw new GenericException(
					$"O servidor {applicationName} devolveu erro. Contacte o administrador do sistema.", e);
			});
		}

		private static TBroker AddRequestLogging<TBroker>(this TBroker broker, ILogger logger)
			where TBroker : IApiBroker
		{
			return broker
				.OnRequest(async (r, ct) =>
				{
					if (logger.IsEnabled(LogLevel.Debug))
					{
						var tb = new StringBuilder();
						foreach (var header in r.Headers)
							tb.Append(header.Key).Append("    ").AppendLine(string.Join("; ", header.Value));

						logger.LogDebug(@"HTTP REST Outbound Request
{method} {url}
{headers}
{content}",
							r.Method, r.RequestUri,
							tb.ToString(),
							r.Content == null ? string.Empty : await r.Content.ReadAsStringAsync());
					}
				})
				.OnResponse(async (r, ct) =>
				{
					if (logger.IsEnabled(LogLevel.Debug))
					{
						var tb = new StringBuilder();
						foreach (var header in r.Headers)
							tb.Append(header.Key).Append("    ").AppendLine(string.Join("; ", header.Value));

						logger.LogDebug(@"HTTP REST Outbound Response
{method} {url} 
{statusCode} {statusCodeDescription}
{headers}
{content}",
							r.RequestMessage.Method, r.RequestMessage.RequestUri, (int)r.StatusCode, r.StatusCode,
							tb.ToString(),
							r.Content == null ? string.Empty : await r.Content.ReadAsStringAsync());
					}
				});
		}

		private static TSoapBroker AddSoapRequestLogging<TSoapBroker>(this TSoapBroker broker, ILogger logger)
		   where TSoapBroker : ISoapClient
		{
			return broker
				.WithHandler(new DelegatingSoapHandler
				{
					Order = int.MaxValue,
					OnHttpRequestAsyncAction = async (c, args, ct) =>
					{
						if (logger.IsEnabled(LogLevel.Debug))
						{
							var r = args.Request;
							var tb = new StringBuilder();
							foreach (var header in r.Headers)
								tb.Append(header.Key).Append("    ").AppendLine(string.Join("; ", header.Value));

							logger.LogDebug(@"HTTP SOAP Outbound Request 
{url} ({action}) [{trackingId}]
{headers}
{content}",
								args.Url, args.Action, args.TrackingId,
								tb.ToString(),
								r.Content == null ? string.Empty : await r.Content.ReadAsStringAsync());
						}
					},
					OnHttpResponseAsyncAction = async (c, args, ct) =>
					{
						if (logger.IsEnabled(LogLevel.Debug))
						{
							var r = args.Response;

							var tb = new StringBuilder();
							foreach (var header in r.Headers)
								tb.Append(header.Key).Append("    ").AppendLine(string.Join("; ", header.Value));

							logger.LogDebug(@"HTTP SOAP Outbound Response 
{url} ({action}) [{trackingId}] 
{statusCode} {statusCodeDescription} 
{headers}
{content}",
								args.Url, args.Action, args.TrackingId, (int)r.StatusCode, r.StatusCode,
								tb.ToString(),
								r.Content == null ? string.Empty : await r.Content.ReadAsStringAsync());
						}
					}
				});
		}

		private class ErrorPayload
		{
			public System.Collections.Generic.Dictionary<string, string[]> ModelState { get; set; }
		}


	}
}