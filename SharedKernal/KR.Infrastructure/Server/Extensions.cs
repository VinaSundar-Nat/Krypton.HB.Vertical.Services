using System;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KR.Infrastructure.Server
{
	public static class Extensions
	{
		public static void RegisterValidators<T>(this IServiceCollection services)
		{
			services.AddValidatorsFromAssemblyContaining<T>();
		}

		public static bool IsLocal(this IHostEnvironment hostEnvironment) =>
		hostEnvironment.EnvironmentName == "Local";

		public static bool IsDev(this IHostEnvironment hostEnvironment) =>
			 hostEnvironment.EnvironmentName == "Local" ||
			 hostEnvironment.EnvironmentName == "Development";
	}
}

