using System;
using System.Collections.Generic;

namespace HCAS.Database.AppDbContextModels;

public partial class Specialization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool del_flg { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
