using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Business.IoC.Factories
{
	public class ValidatorFactory : ValidatorFactoryBase
	{
		private readonly IServiceProvider _serviceProvider;

		public ValidatorFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public override IValidator CreateInstance(Type validatorType)
		{
			var validator = _serviceProvider.GetRequiredService(validatorType);
			return (IValidator)validator;
		}
	}
}