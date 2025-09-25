using System;
using System.Collections.Generic;

namespace HCAS.Database.AppDbContextModels;

public partial class Staff
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool DelFlg { get; set; }
}
