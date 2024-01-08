using DataPlus.API.Contracts.Requests;
using DataPlus.API.Contracts.Responses;
using System.Net.Http.Json;

namespace DataPlusWeb.Shared.Services
{
    public class DataPlusService : IDataPlusService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public DataPlusService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<GetPractitionerListAPIResponse> GetPractitionerList(GetPractitionerListAPIRequest request)
        {
            var httpClient = httpClientFactory.CreateClient("DataPlusAPI");
            GetPractitionerListAPIResponse? result;
            try
            {
                var response = await httpClient.PostAsJsonAsync("/GetPractitionerList", request);
                result = await response.Content.ReadFromJsonAsync<GetPractitionerListAPIResponse>();
            }
            catch (Exception ex)
            {

                throw;
            }

            return result!;
        }
    }
}
