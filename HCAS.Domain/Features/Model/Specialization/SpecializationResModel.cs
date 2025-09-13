using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Model.Specialization
{
    public class SpecializationResModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;       
        public bool del_flg { get; set; }
    }
}
