namespace HCAS.WebApp.Components.Model.Doctors;

public class DoctorReqModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? SpecializationId { get; set; }
}
