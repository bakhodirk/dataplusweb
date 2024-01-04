using Blazor.SubtleCrypto;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using DataPlusWeb.Shared.Models;
using System.Security.Claims;

namespace DataPlusWeb.Client.Provider
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {

        // inject and initialize localstorage service
        private readonly ILocalStorageService _localStorageService;
        private readonly ICryptoService _cryptoService;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService, ICryptoService cryptoService)
        {
            this._localStorageService = localStorageService;
            this._cryptoService = cryptoService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var encryptedUserSession = await _localStorageService.GetItemAsync<UserSession>("UserData");
            if (encryptedUserSession is null)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }

            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name,  await _cryptoService.DecryptAsync(encryptedUserSession.Username!.ToString())),
                new Claim(ClaimTypes.Role, await _cryptoService.DecryptAsync(encryptedUserSession.Role!.ToString()))
            }, "JwtAuth"));

            //Call the Utility class and pass in the token to decrypt
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        }

        public void NotifyAuthenticationState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
