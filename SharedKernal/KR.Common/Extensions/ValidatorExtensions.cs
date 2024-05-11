using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace KR.Common.Extensions
{
	public static class ValidatorExtensions
    {
        public static async Task<(bool IsValid,IList<ValidationFailure> Errors)> Verify<TRequest>(this IValidator<TRequest> validator, TRequest request)
		{
            var validation = await validator.ValidateAsync(request).ConfigureAwait(false);
            return (validation.IsValid, validation.Errors);             
        } 
	}
}

