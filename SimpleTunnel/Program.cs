using Nancy.Scaffolding;
using Nancy.Scaffolding.Models;
using System.Net;

namespace SimpleTunnel
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new ApiBasicConfiguration
            {
                ApplicationContainer = null,
                RequestContainer = null,
                Pipelines = null,
                Mapper = null,
                ApiName = "Simple Tunnel API",
                ApiPort = 8087,
                EnvironmentVariablesPrefix = "SimpleTunnel_"
            };

            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            Api.Run(config);
        }
    }
}
