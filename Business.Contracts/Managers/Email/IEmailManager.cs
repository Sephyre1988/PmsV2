using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Contracts.Managers.Email
{
	public interface IEmailManager : IManager
	{
		/// <summary>
		/// Sends the email asynchronous.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="ct">The ct.</param>
		/// <returns></returns>
		Task SendEmailAsync(long id, string userName, CancellationToken ct);

		/// <summary>
		/// Sends the email template asynchronous.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="ct">The ct.</param>
		/// <returns></returns>
		Task SendEmailTemplateAsync(long id, string userName, CancellationToken ct);

	}
}