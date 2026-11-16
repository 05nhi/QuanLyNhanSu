using System;
using System.Collections.Generic;

namespace QuanLyNhanSu.Models;

public partial class NhanVien
{
    public int MaNv { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? ChucVu { get; set; }

    public int? MaPhong { get; set; }

    public string? DiaChi { get; set; }

    public string? AnhDaiDien { get; set; }

    public virtual PhongBan? MaPhongNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
