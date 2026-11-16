using System;
using System.Collections.Generic;

namespace QuanLyNhanSu.Models;

public partial class TaiKhoan
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? HoTen { get; set; }

    public string? Role { get; set; }

    public int? MaNv { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
