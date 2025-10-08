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

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Phone {  get; set; }

        public string Email { get; set; }
    }
}
