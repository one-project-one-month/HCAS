using System;
using System.Collections.Generic;

namespace HCAS.Database.AppDbContextModels;

public partial class DoctorSchedule
{
    public int Id { get; set; }

    public int? DoctorId { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public int? MaxPatients { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Doctor? Doctor { get; set; }
}
