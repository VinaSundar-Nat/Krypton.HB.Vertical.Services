using System;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace KR.Infrastructure.Server
{
	public static class Extensions
	{
		public static void RegisterValidators<T>(this IServiceCollection services)
		{
            services.AddValidatorsFromAssemblyContaining<T>();
        }
	}
}

