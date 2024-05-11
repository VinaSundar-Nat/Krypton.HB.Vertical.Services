using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KR.Offering.Services.Versioning
{
    public class MediaTypeFormatter : OutputFormatterSelector
    {
        private const string apiCode = "application/vnd.";
        private DefaultOutputFormatterSelector defaultSelector;
        private readonly List<IOutputFormatter> formatters;

        private IDictionary<string, string> _supportedContent = new Dictionary<string, string> {
            { "json", "application/json" },
            { "xml", "text/xml" }
        };
        public MediaTypeFormatter(IOptions<MvcOptions> options, ILoggerFactory loggerFactory)
        {
            defaultSelector = new DefaultOutputFormatterSelector(options, loggerFactory);
            this.formatters = new List<IOutputFormatter>(options.Value.OutputFormatters);
        }

        public override IOutputFormatter? SelectFormatter(OutputFormatterCanWriteContext context,
            IList<IOutputFormatter> formatters, MediaTypeCollection mediaTypes)
        {
            if (formatters == null || formatters.Count == 0)            
                formatters = this.formatters;
            
            context.ContentType = GetContentType(context.HttpContext.Request);

            var formatter = formatters.FirstOrDefault(x => x.CanWriteResult(context));

            context.ContentType = context.HttpContext.Request.Headers["Accept"].First();

            return formatter;
        }

        private string GetContentType(HttpRequest request)
        {
            var acceptHeaderValue = request.Headers["Accept"].First();
            if (acceptHeaderValue.IndexOf("+") > 0)
            {
                var contentType = acceptHeaderValue.Substring(acceptHeaderValue.IndexOf("+") + 1);
                if (_supportedContent.ContainsKey(contentType))
                    return _supportedContent[contentType];
            }
            return _supportedContent["json"];
        }
    }
}

