CREATE DATABASE QLNhanSuDB;
GO
USE QLNhanSuDB;
GO

-- Bảng Phòng ban
CREATE TABLE PhongBan (
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenPhong NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(250)
);

-- Bảng Nhân viên
CREATE TABLE NhanVien (
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    SoDienThoai NVARCHAR(20),
    ChucVu NVARCHAR(50),
    MaPhong INT FOREIGN KEY REFERENCES PhongBan(MaPhong) ON DELETE SET NULL
);

-- Bảng Tài khoản (Tự quản lý)
CREATE TABLE TaiKhoan (
    Username NVARCHAR(50) PRIMARY KEY,
    Password NVARCHAR(50) NOT NULL,
    HoTen NVARCHAR(100),
    Role NVARCHAR(20) DEFAULT 'User' -- 'Admin' hoặc 'User'
);

-- Chèn dữ liệu mẫu
INSERT INTO PhongBan VALUES (N'Phòng Kỹ thuật', N'Phát triển phần mềm'), (N'Phòng Nhân sự', N'Tuyển dụng và đào tạo');
INSERT INTO TaiKhoan VALUES ('admin', '123456', N'Quản trị viên', 'Admin'), ('user1', '123456', N'Nhân viên A', 'User');
INSERT INTO NhanVien VALUES (N'Nguyễn Văn A', 'nva@gmail.com', '0901234567', N'Lập trình viên', 1);

SELECT * FROM NhanVien
SELECT * FROM PhongBan