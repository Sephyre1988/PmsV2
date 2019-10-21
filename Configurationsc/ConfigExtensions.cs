using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using Entities;
using Entities.PmsUser;

namespace Configurations
{
	internal static class ConfigExtensions
	{
		private const string InvalidValueForDomainWithNameFormat = "Valor '{0}' inválido para dominio com nome '{1}'";
		private const string DomainParseErrorFormat = "Valor inválido para configuração com chave '{0}.";
		

		#region Domain

		public static string DescriptionAsRequired(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			if (string.IsNullOrWhiteSpace(domain.Description))
				throw new AccessViolationException($"Domínio de nome '{domain.Name}' tem descrição vazia");

			return domain.Description;
		}

		public static string ValueAsRequired(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			if (string.IsNullOrWhiteSpace(domain.Value))
				throw new AccessViolationException($"Domínio de nome '{domain.Name}' tem valor vazio");

			return domain.Value;
		}

		public static long ValueAsLong(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.ValueAsRequired();
			long result;
			if (long.TryParse(value, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static long? ValueAsNullableLong(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.Value;
			if (string.IsNullOrWhiteSpace(value))
				return null;

			long result;
			if (long.TryParse(value, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static int ValueAsInt(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.ValueAsRequired();
			int result;
			if (int.TryParse(value, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static int? ValueAsNullableInt(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.Value;
			if (string.IsNullOrWhiteSpace(value))
				return null;

			int result;
			if (int.TryParse(value, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static short ValueAsShort(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.ValueAsRequired();
			short result;
			if (short.TryParse(value, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static short? ValueAsNullableShort(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.Value;
			if (string.IsNullOrWhiteSpace(value))
				return null;

			short result;
			if (short.TryParse(value, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static decimal ValueAsDecimal(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.ValueAsRequired();
			decimal result;
			if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static decimal? ValueAsNullableDecimal(this Domain domain)
		{
			if (domain == null) throw new ArgumentNullException(nameof(domain));

			var value = domain.Value;
			if (string.IsNullOrWhiteSpace(value))
				return null;

			decimal result;
			if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				return result;
			throw new AccessViolationException(string.Format(InvalidValueForDomainWithNameFormat, value, domain.Name));
		}

		public static Domain SingleByName(this IReadOnlyCollection<Domain> domains, string name)
		{
			if (domains == null) throw new ArgumentNullException(nameof(domains));
			if (name == null) throw new ArgumentNullException(nameof(name));

			if (domains.Count == 0)
				throw new AccessViolationException("Coleção de dominios está vazia");

			Domain parameter = null;
			foreach (var p in domains)
			{
				if (name.Equals(p.Name))
				{
					if (parameter == null)
						parameter = p;
					else
						throw new AccessViolationException($"Parâmetro com nome '{name}' duplicado");
				}
			}

			if (parameter == null)
				throw new AccessViolationException($"Parâmetro com nome '{name}' não encontrado");

			return parameter;
		}

		public static T ConvertAllWithName<T>(this IReadOnlyCollection<Domain> domains, string name, Func<List<Domain>, T> mapper)
		{
			if (domains == null) throw new ArgumentNullException(nameof(domains));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (mapper == null) throw new ArgumentNullException(nameof(mapper));

			if (domains.Count == 0)
				throw new AccessViolationException("Coleção de dominios está vazia");

			var namedDomains = new List<Domain>(domains.Count);
			namedDomains.AddRange(domains.Where(e => name.Equals(e.Name)));
			if (namedDomains.Count == 0)
				throw new AccessViolationException($"Não foram encontrados domínios para mapear o nome '{name}'");

			try
			{
				return mapper(namedDomains);
			}
			catch (AccessViolationException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new AccessViolationException($"Falha ao converter dominios com o nome '{name}'", e);
			}
		}

		public static void ThrowExceptionOnFailedDomainParse(string domainKey, IDictionary<long, string> parsedValues)
		{
			if (!(parsedValues.Count > 0))
			{
				throw new AccessViolationException(string.Format(DomainParseErrorFormat, domainKey));
			}
		}

		/// <summary>
		/// Wraps parsing code, catching any exception and throwing one that represents a domain loading exception.
		/// </summary>
		/// <typeparam name="T">The type of the parsing result.</typeparam>
		/// <param name="name">The name of the domain being parsed.</param>
		/// <param name="domainParser">The <see cref="Func{T}"/> that parses the domain and returns the result.</param>
		/// <returns>The parsing result.</returns>
		internal static T ThrowExceptionOnFailedDomainParse<T>(string name, Func<T> domainParser)
		{
			return ThrowExceptionOnFailedParse(name, domainParser, DomainParseErrorFormat);
		}

		private static T ThrowExceptionOnFailedParse<T>(string key, Func<T> parser, string messageFormat)
		{
			try
			{
				return parser();
			}
			catch (Exception e)
			{
				throw new AccessViolationException(string.Format(messageFormat, key), e);
			}
		}

		#endregion

	}

}