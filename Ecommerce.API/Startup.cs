using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Microsoft.ServiceFabric.Data;
using Owin;

namespace Ecommerce.API
{
    public class Startup : IOwinAppBuilder
    {
        private readonly StatelessServiceContext _context;

        public Startup(StatelessServiceContext context)
        {
            _context = context;
        }
        public static void ConfigureFormatters(MediaTypeFormatterCollection formatters)
        {
            formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "{controller}/{id}",
                new { id = RouteParameter.Optional }
            );

            ConfigureFormatters(config.Formatters);

            appBuilder.Use(async (env, next) =>
            {
                ServiceEventSource.Current.ServiceMessage(_context, $"received request at {env.Request.Path}");
                await next();
                ServiceEventSource.Current.ServiceMessage(_context, $"Reponse sent back: {env.Response.StatusCode}");
            });

            appBuilder.UseWebApi(config);
        }
    }
}
