using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace UoW.Implementation
{
	public class PmsXmlSerializer
	{
		private readonly ILogger<PmsXmlSerializer> _logger;
		public XmlWriterSettings Settings { get; }

		public PmsXmlSerializer(ILogger<PmsXmlSerializer> logger)
		{
			_logger = logger;
			Settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = logger.IsEnabled(LogLevel.Debug)
			};
		}

		public string SerializeAsXml(object instance)
		{
			Type instanceType;

			string xmlValue;
			if (instance == null)
			{
				instanceType = null;
				xmlValue = null;
			}
			else
			{
				instanceType = instance.GetType();

				using (var stringWriter = new StringWriter())
				using (var xmlWritter = XmlWriter.Create(stringWriter, Settings))
				{
					var xmlSerializer = new XmlSerializer(instanceType);
					xmlSerializer.Serialize(xmlWritter, instance);

					xmlValue = stringWriter.ToString();
				}
			}

			_logger.LogDebug(
				"Serialized instance of type '{instanceType}' as xml text:\n{xmlValue}",
				instanceType, xmlValue);

			return xmlValue;
		}

		public T DeserializeFromXml<T>(string xmlValue) where T : class
		{
			var instanceType = typeof(T);
			_logger.LogDebug(
				"Deserializing instance of type '{instanceType}' from xml text:\n{xmlValue}",
				instanceType, xmlValue);

			T instance;
			if (string.IsNullOrWhiteSpace(xmlValue))
				instance = null;
			else
			{
				using (var textWriter = new StringReader(xmlValue))
				{
					instance = (T)new XmlSerializer(instanceType).Deserialize(textWriter);
				}
			}

			return instance;
		}

	}
}