using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Models.Patient
{
    public class PatientReqModel
    {
        public string Name { get; set; }

<<<<<<< HEAD
        public DateOnly DateOfBirth { get; set; }
=======
        public DateTime? DateOfBirth { get; set; }
>>>>>>> 3fe08fde475f6bf541b0515d48f7202169bbdc33

        public string Gender { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}