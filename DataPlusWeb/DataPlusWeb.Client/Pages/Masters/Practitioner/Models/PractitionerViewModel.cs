using System.Diagnostics.CodeAnalysis;

namespace DataPlusWeb.Client.Pages.Masters.Practitioner;

public class PractitionerViewModel
{
    public long Id { get; set; }

    public int DoctorID { get; set; }

    public int? ParentID { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? ZipCode { get; set; }

    public bool Active { get; set; }
}
