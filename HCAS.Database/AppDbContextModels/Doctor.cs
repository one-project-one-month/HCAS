using System;
using System.Collections.Generic;

namespace HCAS.Database.AppDbContextModels;

public partial class Doctor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? SpecializationId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    public virtual Specialization? Specialization { get; set; }

    public bool del_flg { get; set; }

}
