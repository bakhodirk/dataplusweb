using DataPlus.Auth.API.Contracts.Requests;
using DataPlus.Auth.API.Contracts.Responses;

namespace DataPlusWeb.Components.Account.Services
{
    public class AuthService : IAuthService
	{
		private readonly IHttpClientFactory httpClientFactory;

		public AuthService(IHttpClientFactory httpClientFactory)
		{
			this.httpClientFactory = httpClientFactory;
		}

		#region Public methods reigon

		public async Task<ApplicationUser> FindByUser(string username, string password)
        {
			var httpClient = httpClientFactory.CreateClient("AuthAPI");
			var request = new AuthenticateAPIRequest();
			var result = new ApplicationUser();

			try
			{
				request.Record = new DataPlus.Auth.API.Contracts.Models.User()
				{
					Username = username,
					Password = password
				};

				var responseMessage = await httpClient.PostAsJsonAsync("Auth", request);
				var response = await responseMessage.Content.ReadFromJsonAsync<AuthenticateAPIResponse>();

				if (response!.Success)
				{
					result.UserName = response.Record.Username;
				}
				else
				{
					throw new Exception(response.GetMessage());
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			return result;
        }

        #endregion
    }
}
