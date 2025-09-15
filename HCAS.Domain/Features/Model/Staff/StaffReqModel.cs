using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Model.Staff
{
    public class StaffReqModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Role { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

    }
}
