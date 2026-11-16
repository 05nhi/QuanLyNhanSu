using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhanSu.Models;

public partial class QlnhanSuDbContext : DbContext
{
    public QlnhanSuDbContext()
    {
    }

    public QlnhanSuDbContext(DbContextOptions<QlnhanSuDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhongBan> PhongBans { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=QLNhanSuDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70A5A55BD18");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__NhanVien__MaPhon__398D8EEE");
        });

        modelBuilder.Entity<PhongBan>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__PhongBan__20BD5E5B4795AA44");

            entity.ToTable("PhongBan");

            entity.Property(e => e.MoTa).HasMaxLength(250);
            entity.Property(e => e.TenPhong).HasMaxLength(100);
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__TaiKhoan__536C85E5774A6497");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__TaiKhoan__MaNV__47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
