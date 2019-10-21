using System;
using System.Collections.Generic;

namespace Business.Contracts.Managers.Email
{
	public class EmailMessage
	{
		public long Id { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }

		public DateTimeOffset? SchedulerDate { get; set; }

		public IEnumerable<EmailDestination> Destinations { get; set; }

		public IEnumerable<EmailAttachment> Attachments { get; set; }

		public EmailMessage()
		{
			Destinations = new EmailDestination[0];
			Attachments = new EmailAttachment[0];
		}
	}
}