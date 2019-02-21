using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.Scaffolding.Modules;
using PackUtils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models.Exceptions;
using WebApi.Models.Response;

namespace SimpleTunnel
{
    public class TunnelController : BaseModule
    {
        public TunnelController()
        {
            this.Get("", args => this.Proxy());
            this.Post("", args => this.Proxy());
            this.Put("", args => this.Proxy());
            this.Patch("", args => this.Proxy());
            this.Delete("", args => this.Proxy());

            this.Get("/{all*}", args => this.Proxy());
            this.Post("/{all*}", args => this.Proxy());
            this.Put("/{all*}", args => this.Proxy());
            this.Patch("/{all*}", args => this.Proxy());
            this.Delete("/{all*}", args => this.Proxy());
        }

        public object Proxy()
        {
            var baseUrl = this.GetBaseUrl();
            var method = this.Context.Request.Method;
            var queryAsString = this.Context.Request.Url.Query;
            var query = this.GetQueryString(queryAsString);
            var path = this.Context.Request.Url.Path;
            var headers = this.GetRequestHeaders();
            var headersAsString = this.GetRequestHeadersAsString(headers); 
            var body = "";
            
            if (method.ToLowerInvariant() != "get")
            {
                body = RequestStream.FromStream(this.Context.Request.Body).AsString();
            }
            
            this.PrintRequest(baseUrl, method, path, queryAsString, body, headersAsString);

            var restResponse = this.ExecuteRequestToTunnel(baseUrl, method, path, body, query, headers);

            return this.GetProxyResponse(restResponse);
        }

        private Response GetProxyResponse(IRestResponse restResponse)
        {
            var response = new Response
            {
                StatusCode = (HttpStatusCode)restResponse.StatusCode,
                Contents = s => s.Write(restResponse.RawBytes, 0, restResponse.RawBytes.Length),
                ContentType = restResponse.ContentType
            };

            var heardesDic = restResponse.Headers.ToDictionary();
            foreach (var header in heardesDic)
            {
                response.Headers[header.Key] = header.Value;
            }

            return response;
        }

        private string GetRequestHeadersAsString(IDictionary<string, IEnumerable<string>> headers)
        {
            return string.Join("&", headers.Select(r => $"{r.Key}={string.Join("; ", r.Value)}"));
        }

        private void PrintRequest(string baseUrl, string method, string path, string query, string body, string headers)
        {
            Console.WriteLine($"### PROXY REQUEST ###\n");
            Console.WriteLine($"TunelUrl: {baseUrl}\n");
            Console.WriteLine($"Method: {method}\n");
            Console.WriteLine($"Path: {path}\n");
            Console.WriteLine($"Query: {query}\n");
            Console.WriteLine($"Body: {body}\n");
            Console.WriteLine($"Headers: {headers}\n");
        }

        private IRestResponse ExecuteRequestToTunnel(string baseUrl, string method, string path, string body, IDictionary<string, IEnumerable<string>> query, IDictionary<string, IEnumerable<string>> headers)
        {
            var client = new RestClient(baseUrl);
            var restMethod = method.ConvertToEnum<Method>();
            var request = new RestRequest(path, restMethod);
            var contentType = this.HandleRestParametersAndGetContentType(request, headers);

            this.AddHeaders(request, headers);
            this.AddQuery(request, query);
            this.AddBody(request, body, contentType);
            
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            return response;
        }

        private void AddBody(RestRequest request, string body, string contentType)
        {
            if (body != null && request.Method != Method.GET)
            {
                request.AddParameter(contentType, body, ParameterType.RequestBody);
            }
        }

        private void AddHeaders(RestRequest request, IDictionary<string, IEnumerable<string>> headers)
        {
            foreach (var header in headers)
            {
                foreach (var headerValue in header.Value)
                {
                    request.AddHeader(header.Key, headerValue);
                }
            }
        }

        private void AddQuery(RestRequest request, IDictionary<string, IEnumerable<string>> query)
        {
            foreach (var queryItem in query)
            {
                foreach (var queryValue in queryItem.Value)
                {
                    request.AddQueryParameter(queryItem.Key, queryValue);
                }
            }
        }

        private string HandleRestParametersAndGetContentType(RestRequest request, IDictionary<string, IEnumerable<string>> headers)
        {
            request.Parameters.Clear();

            var contentType = headers.ContainsKey("Content-Type")
                                ? headers["Content-Type"].First()
                                : "application/json";

            headers.Remove("Content-Type");
            headers.Remove("Content-Length");
            headers.Remove("Host");

            return contentType;
        }

        private string GetBaseUrl()
        {
            try
            {
                var url = this.Context.Request.Query["tunnel_url"];
                var uri = new Uri(url);
                return uri.GetLeftPart(System.UriPartial.Authority);
            }
            catch (Exception)
            {
                throw new PreconditionFailedException(ErrorsResponse.WithSingleError("Invalid Tunnel Url", "tunnel_url"));
            }
        }

        private IDictionary<string, IEnumerable<string>> GetQueryString(string query)
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(query);
            queryString.Remove("tunnel_url");
            queryString.Remove(null);
            queryString.Remove("");

            return queryString.ToDictionary();
        }

        private IDictionary<string, IEnumerable<string>> GetRequestHeaders()
        {
            var headers = new Dictionary<string, IEnumerable<string>>();

            if (this.Context.Request.Headers != null)
            {
                foreach (var item in this.Context.Request.Headers)
                {
                    if (item.Value != null)
                    {
                        headers.Add(item.Key, item.Value.ToArray());
                    }
                }
            }

            return headers;
        }
    }
}
