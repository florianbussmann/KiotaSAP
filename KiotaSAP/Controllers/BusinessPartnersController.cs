using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using S4HANABusinessPartners.Client;

namespace KiotaSAP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusinessPartnersController : ControllerBase
    {
        private readonly S4HANABusinessPartnersClient _businessPartnersClient;
        public BusinessPartnersController()
        {
            var handler = new SapClientHandler
            {
                InnerHandler = new HttpClientHandler()
            };

            var httpClient = new HttpClient(handler);

            var authProvider = new BasicAuthenticationProvider("sandbox", "password");
            // Create request adapter using the HttpClient-based implementation
            var adapter = new HttpClientRequestAdapter(authProvider , httpClient: httpClient);
            // Create the API client
            _businessPartnersClient = new S4HANABusinessPartnersClient(adapter);
        }

        [HttpGet(Name = "GetBusinessPartners")]
        public async Task<S4HANABusinessPartners.Client.Models.A_BusinessPartnerType> Get(String id)
        {
            System.Console.WriteLine("Test");
            
            var response = await _businessPartnersClient.A_BusinessPartner.GetAsA_BusinessPartnerGetResponseAsync(config =>
            {
                config.QueryParameters.Filter = $"BusinessPartner eq '{id}'";
            });
            
            return response!.D!.Results!.Single();
        }
    }

    public class SapClientHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var builder = new UriBuilder(request.RequestUri!);

            var query = System.Web.HttpUtility.ParseQueryString(builder.Query);
            query["saml2"] = "disabled";
            query["sap-client"] = "500";

            builder.Query = query.ToString();

            request.RequestUri = builder.Uri;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
