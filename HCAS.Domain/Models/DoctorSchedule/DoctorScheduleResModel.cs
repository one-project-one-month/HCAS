using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Models.DoctorSchedule
{

<<<<<<< HEAD
        public int DoctorId { get; set; }

        public DateTime ScheduleDate { get; set; }

        public int MaxPatients { get; set; }

        public bool del_flg { get; set; }
    }

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
=======
>>>>>>> 3fe08fde475f6bf541b0515d48f7202169bbdc33
}
