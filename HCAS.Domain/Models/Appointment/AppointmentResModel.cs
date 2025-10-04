namespace HCAS.Domain.Models.Appointment;

public class AppointmentResModel
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public int? DoctorId { get; set; }

    public int PatientId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public int AppointmentNumber { get; set; }

    public string Status { get; set; } = null!;

    public bool DelFlg { get; set; }

    // public virtual Doctor Doctor { get; set; } = null!;
}

public class AppointmentResponseModel
{
    public int Id { get; set; }
    public string DoctorName { get; set; }
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int AppointmentNumber { get; set; }
    public string Status { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int ScheduleId { get; set; }
}