using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace ServcoHackathon.Internal {
    public class XmlInputFormatter : InputFormatter {
        private static readonly IImmutableSet<Type> SupportedTypes =
            ImmutableHashSet<Type>.Empty.Add(typeof(XDocument)).Add(typeof(XmlDocument));

        public XmlInputFormatter() {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
        }

        public override bool CanRead(InputFormatterContext context) {
            return SupportedMediaTypes.Contains(
                MediaTypeHeaderValue.Parse(context.HttpContext.Request.ContentType).MediaType.Value
            );
        }

        protected override Boolean CanReadType(Type type) {
            return SupportedTypes.Contains(type);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context) {

            // Assumes UTF-8 or BOM
            if (context.ModelType == typeof(XDocument)) {
                var xml =
                    await XDocument.LoadAsync(
                        context.HttpContext.Request.Body,
                        LoadOptions.None,
                        CancellationToken.None
                    );

                return await InputFormatterResult.SuccessAsync(xml);
            } else if (context.ModelType == typeof(XmlDocument)) {
                var xml = new XmlDocument();
                xml.Load(context.HttpContext.Request.Body);
                return await InputFormatterResult.SuccessAsync(xml);
            } else {
                throw new NotSupportedException($"The specified model type ({context.ModelType.Name}) is not supported by {nameof(XmlInputFormatter)}.");
            }
        }
    }
}
