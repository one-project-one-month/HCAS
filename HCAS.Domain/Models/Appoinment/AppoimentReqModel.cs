using HCAS.Database.AppDbContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Models.Appoinment
{
    public class AppoimentReqModel
    {
     //  public int Id { get; set; }

        public int ScheduleId { get; set; }

        public int DoctorId { get; set; }

        public int PatientId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public int AppointmentNumber { get; set; }

        public string Status { get; set; } = null!;

        public bool DelFlg { get; set; }

     //   public virtual Doctor Doctor { get; set; } = null!;

       // public virtual Patient Patient { get; set; } = null!;

        //public virtual DoctorSchedule Schedule { get; set; } = null!;
    }
}
