using HCAS.Database.AppDbContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Model.Doctors
{
    public class DoctorsReqModel
    {
        //public int? Id { get; set; }

        public string Name { get; set; } = null!;

        public int SpecializationId { get; set; }

       // public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        //public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

        //public virtual Specialization? Specialization { get; set; }
    }
}
