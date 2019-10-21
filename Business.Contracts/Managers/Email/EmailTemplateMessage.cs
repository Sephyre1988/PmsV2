using System;
using System.Collections.Generic;

namespace Business.Contracts.Managers.Email
{
	public class EmailTemplateMessage
	{
		public long Id { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }
		/// <summary>
		/// Template identifier from the template created on SendGrid account
		/// </summary>
		public string TemplateId { get; set; }
		public string From { get; set; }

		public DateTimeOffset? SchedulerDate { get; set; }

		public IEnumerable<EmailDestination> Destinations { get; set; }

		public IEnumerable<EmailAttachment> Attachments { get; set; }

		public IEnumerable<EmailParameter> Parameters { get; set; }

		public EmailTemplateMessage()
		{
			Destinations = new EmailDestination[0];
			Attachments = new EmailAttachment[0];
			Parameters = new EmailParameter[0];
		}
	}
}