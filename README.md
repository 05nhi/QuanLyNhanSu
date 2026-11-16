# Hệ Thống Quản Lý Nhân Sự (QuanLyNhanSu)

Ứng dụng web quản lý thông tin nhân sự và phòng ban được xây dựng bằng nền tảng ASP.NET và SQL Server.

## Công nghệ sử dụng
* **Ngôn ngữ & Framework:** C#, ASP.NET Core MVC
* **Cơ sở dữ liệu:** Microsoft SQL Server
* **Giao diện:** HTML5, CSS3, Bootstrap, JavaScript

## Chức năng chính
* **Đăng nhập & Phân quyền:** Xác thực người dùng và phân chia quyền hạn truy cập giữa Quản trị viên (`Admin`) và Người dùng (`User`).
* **Quản lý Nhân viên:** Thêm mới, xem danh sách, cập nhật thông tin, xóa và tìm kiếm hồ sơ nhân viên.
* **Quản lý Phòng ban:** Quản lý cơ cấu phòng ban, phân bổ nhân sự theo từng phòng ban trực thuộc.

## Hướng dẫn cài đặt Cơ sở dữ liệu
1. Mở **SQL Server Management Studio (SSMS)**.
2. Mở tệp kịch bản `QLNhanSuDB.sql` có sẵn trong thư mục dự án và nhấn **Execute** để khởi tạo cơ sở dữ liệu cùng dữ liệu mẫu.
3. Cập nhật chuỗi kết nối (`ConnectionStrings`) trong tệp `appsettings.json` cho phù hợp với máy chủ SQL Server của bạn và chạy ứng dụng.
