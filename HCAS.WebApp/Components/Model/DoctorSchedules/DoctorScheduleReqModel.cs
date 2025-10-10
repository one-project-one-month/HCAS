using System;

namespace HCAS.WebApp.Components.Model.DoctorSchedules;

public class DoctorScheduleReqModel
{
        public int Id { get; set; }

        public int? DoctorId { get; set; }

        public string? DoctorName { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int? MaxPatients { get; set; }

        public bool? DelFlag { get; set; }
}
