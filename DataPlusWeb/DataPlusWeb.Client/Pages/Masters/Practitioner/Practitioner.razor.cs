
using DataPlus.Web.UI.Components;

namespace DataPlusWeb.Client.Pages.Masters.Practitioner;

public partial class Practitioner
{
    public class ViewContext
    {

        //public async IAsyncEnumerable<PractitionerViewModel> GetPractitionerList(CancellationToken cancellationToken)
        //{ }
    }

    private IEnumerable<PractitionerViewModel> _practitioners = new PractitionerViewModel[0];

    private void LoadPractitioners(DataGridReadDataEventArgs<PractitionerViewModel> e)
    {
        //await Task.Delay(10);
        //await Task.Delay(10000);
        //for (int i = 0; i < 5; i++)
        //{
        //    _practitioners
        //}
        //Console.WriteLine("asdsa");
    }

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }
}