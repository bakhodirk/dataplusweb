using DataPlus.API.Contracts.Requests;
using DataPlus.Web.UI.Components;
using Microsoft.AspNetCore.Components;

namespace DataPlusWeb.Client.Pages.Masters.Practitioner;

public partial class PractitionerOverview
{
    [Inject]
    public NavigationManager NavigationManager {  get; set; } 

    private IEnumerable<PractitionerViewModel>? _items = new PractitionerViewModel[] { };

    async Task LoadItems(DataGridReadDataEventArgs<PractitionerViewModel> e)
    {
        var response = await DataPlusService.GetPractitionerList(BaseGetListAPIRequest.GetAllRecordsListRequest<GetPractitionerListAPIRequest>());

        _items = (from d in response.Records
                  select new PractitionerViewModel()
                  {
                      Id = d.Id,
                      Name = d.FirstName,
                      Address = d.MiddleName
                  }).ToArray();
    }

    private void OnPractitionerClicked(DataGridRowMouseEventArgs<PractitionerViewModel> e)
    {
        NavigationManager.NavigateTo($"/Masters/Practitioner/{e.Item.Id}/Edit", true);
    }
}