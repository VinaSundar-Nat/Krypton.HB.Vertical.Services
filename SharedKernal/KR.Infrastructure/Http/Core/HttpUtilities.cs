using KR.Infrastructure.Http.Exceptions;
using KR.Common.Extensions;
using System.Net;
using FluentValidation.Results;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using KR.Common.Gaurd;
using System.ComponentModel;

namespace KR.Infrastructure.Http;

public abstract partial class HttpBase<ErrorHandler>
{		
    //param: vnd.offering.v1
    private string contextVersion(string? text) =>
        !string.IsNullOrEmpty(text) ? $"application/{text}+json" : "application/json";

    private static MediaTypeHeaderValue CustomMediaTypeHeaderValue(string type) => new MediaTypeHeaderValue(type);

    protected static string BuildUrl<T>(T model) where T : class
    {
        var urlBuilder = new StringBuilder();
        var type = model.GetType();

        foreach (var property in type.GetProperties())
        {
            JsonPropertyAttribute? attribute = (JsonPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                                                .Gaurd("Implementor type not defined.");
            urlBuilder.Append($"{attribute.PropertyName}={property.GetValue(model)}&");
        }

        string url = urlBuilder.ToString();
        return url.Length > 0 ? url.Substring(0, url.Length - 1) : string.Empty;
    }

    private void GetMetaData<TMetaData>(HttpResponseMessage response, TMetaData? metaData) where TMetaData : class
    {
        if (response == null || metaData == null)
            return;

        var expressions = metaData.BuildMemberExpressions<TMetaData>();

        foreach (var expression in expressions)
        {
            var field = expression.GetCustomAttributeValue<TMetaData, DescriptionAttribute, string>(attr => attr.Description);
            var value = response.Headers.GetValues(field)?.FirstOrDefault() ?? string.Empty;
            metaData.ExpressionSetter<TMetaData>(expression.Member.Name, value);
        }
    }
}


