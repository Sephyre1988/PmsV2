using System;

namespace Configurations.Contracts
{
	public class Settings
	{
		public RoomOptions RoomOptions { get; set; }
	}

	public class RoomOptions
	{
		//adicionar as configurações necessárias aos quartos! com get e set
		public string Environment { get; set; }
		public TimeSpan Timeout { get; set; }
	}
}