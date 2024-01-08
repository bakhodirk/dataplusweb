using DataPlus.API.Contracts.Requests;
using DataPlus.API.Contracts.Responses;

namespace DataPlusWeb.Shared.Services
{
    public interface IDataPlusService
    {
        Task<GetPractitionerListAPIResponse> GetPractitionerList(GetPractitionerListAPIRequest request);
    }
}
