using DataPlus.API.Contracts.Requests;
using DataPlusWeb.Shared.Models;
using DataPlusWeb.Shared.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace DataPlusWeb.Client.Pages.Masters.Practitioner;

public partial class PractitionerEdit
{
    private PractitionerViewModel _model = new();

    [Parameter]
    public long? Id { get; set; }

    [Inject, AllowNull]
    public NavigationManager NavigationManager { get; set; }

    [Inject, AllowNull]
    public IDataPlusService DataPlusService { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Id is not null)
        {
            var response = await DataPlusService.GetPractitioner(Id.Value);
            _model = new(){
                Id = response.Record.Id,
                Name = response.Record.FirstName,
                Address = response.Record.LastName
            };
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected async Task Save()
    {
        var response = await DataPlusService.CreateOrUpdatePractitioner(new UpdatePractitionerAPIRequest {
            Record = new DataPlus.API.Contracts.Models.Practitioner {

                Id = _model?.Id ?? 0,
                FirstName = _model?.Name!,
                LastName = _model?.Address!
            }
        });

        if (response.Success)
        {
            NavigationManager.NavigateTo("/Masters/Practitioner", true);
        }
    }

    protected async Task Delete()
    {
        var response = await DataPlusService.DeletePractitioner(_model.Id, null!);

        if (response.Success)
        {
            NavigationManager.NavigateTo("/Masters/Practitioner", true);
        }
    }
}