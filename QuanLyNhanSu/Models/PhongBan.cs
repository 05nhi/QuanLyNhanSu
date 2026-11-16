using System;
using System.Collections.Generic;

namespace QuanLyNhanSu.Models;

public partial class PhongBan
{
    public int MaPhong { get; set; }

    public string TenPhong { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
