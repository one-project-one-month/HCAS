using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Model.DoctorSchedules;

public class DoctorScheduleResModel
{
    public int Id { get; set; }

    public int? DoctorId { get; set; }

    // public string? DoctorName { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public int? MaxPatients { get; set; }

    public bool? DelFlag { get; set; }
}
