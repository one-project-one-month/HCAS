using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Models.DoctorSchedule
{

    public class DoctorScheduleResponseModel
    {
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public int? MaxPatients { get; set; }
        public int AppointmentCount { get; set; }
        public int? AvailableSlots { get; set; }
    }

}
