using Blazored.LocalStorage;
using DataPlusWeb.Shared.Models;

namespace DataPlusWeb.Handlers
{
    public class CustomHttpHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;
        public CustomHttpHandler(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri!.AbsolutePath.ToLower().Contains("auth"))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var token = await _localStorageService.GetItemAsync<UserSession>("UserData", cancellationToken = default);
            if (token.Token is not null)
            {
                request.Headers.Add("Authorization", $"Bearer {token.Token}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
