using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Model.Doctors
{
    public class DoctorsResModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SpecializationId { get; set; }
        public bool DelFlg { get; set; }
    }
}
