using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly QlnhanSuDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NhanVienController(QlnhanSuDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. Trang danh sách nhân viên (Chỉ Admin được xem toàn bộ danh bạ)
        public async Task<IActionResult> Index(string searchString)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Nếu là User thường -> chuyển thẳng sang trang Hồ sơ cá nhân
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction(nameof(HoSoCaNhan));
            }

            var nhanViens = _context.NhanViens.Include(n => n.MaPhongNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                nhanViens = nhanViens.Where(s => s.HoTen.Contains(searchString) || s.SoDienThoai!.Contains(searchString));
            }

            return View(await nhanViens.ToListAsync());
        }

        // 2. Trang Hồ sơ cá nhân dành cho Nhân viên (User)
        [HttpGet]
        public async Task<IActionResult> HoSoCaNhan()
        {
            var maNv = HttpContext.Session.GetInt32("MaNV");
            if (maNv == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.MaPhongNavigation)
                .FirstOrDefaultAsync(n => n.MaNv == maNv);

            if (nhanVien == null) return NotFound();

            return View(nhanVien);
        }

        // POST: Nhân viên tự cập nhật SĐT, Địa chỉ và Ảnh đại diện
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSoCaNhan(string? soDienThoai, string? diaChi, IFormFile? fileAnh)
        {
            var maNv = HttpContext.Session.GetInt32("MaNV");
            if (maNv == null) return RedirectToAction("Login", "Account");

            var nhanVien = await _context.NhanViens
                .Include(n => n.MaPhongNavigation)
                .FirstOrDefaultAsync(n => n.MaNv == maNv);

            if (nhanVien == null) return NotFound();

            // Cập nhật thông tin cá nhân được phép sửa
            nhanVien.SoDienThoai = soDienThoai;
            nhanVien.DiaChi = diaChi;

            // Xử lý tải ảnh đại diện mới
            if (fileAnh != null && fileAnh.Length > 0)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = $"nv_{maNv}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(fileAnh.FileName)}";
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileAnh.CopyToAsync(stream);
                }

                nhanVien.AnhDaiDien = "/images/" + fileName;
            }

            _context.Update(nhanVien);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Cập nhật thông tin cá nhân thành công!";
            return View(nhanVien);
        }

        // 3. Trang Thêm mới Nhân viên (GET - Chỉ Admin)
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction(nameof(HoSoCaNhan));

            ViewData["MaPhong"] = new SelectList(_context.PhongBans, "MaPhong", "TenPhong");
            return View("Them");
        }

        // POST: NhanVien/Create (Tự động cấp tài khoản mặc định cho nhân viên mới)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNv,HoTen,Email,SoDienThoai,ChucVu,MaPhong,DiaChi")] NhanVien nhanVien)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();

            if (ModelState.IsValid)
            {
                // 1. Lưu thông tin nhân viên trước để SQL tự sinh ra MaNv
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();

                // 2. Tự động tạo tài khoản đăng nhập mặc định theo MaNv vừa sinh
                string autoUsername = "nv" + nhanVien.MaNv;
                if (!_context.TaiKhoans.Any(t => t.Username == autoUsername))
                {
                    var taiKhoanMoi = new TaiKhoan
                    {
                        Username = autoUsername,
                        Password = "123456",
                        HoTen = nhanVien.HoTen,
                        Role = "User",
                        MaNv = nhanVien.MaNv
                    };
                    _context.TaiKhoans.Add(taiKhoanMoi);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Thêm nhân viên {nhanVien.HoTen} thành công! Tài khoản đăng nhập: {autoUsername} | Mật khẩu: 123456";
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["MaPhong"] = new SelectList(_context.PhongBans, "MaPhong", "TenPhong", nhanVien.MaPhong);
            return View("Them", nhanVien);
        }

        // 4. Trang Chỉnh sửa Nhân viên (GET - Chỉ Admin)
        public async Task<IActionResult> Edit(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction(nameof(HoSoCaNhan));

            if (id == null) return NotFound();

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null) return NotFound();

            ViewData["MaPhong"] = new SelectList(_context.PhongBans, "MaPhong", "TenPhong", nhanVien.MaPhong);
            return View("Sua", nhanVien);
        }

        // POST: NhanVien/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNv,HoTen,Email,SoDienThoai,ChucVu,MaPhong,DiaChi,AnhDaiDien")] NhanVien nhanVien)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();
            if (id != nhanVien.MaNv) return NotFound();

            if (ModelState.IsValid)
            {
                // Giữ lại ảnh đại diện cũ nếu form Sua.cshtml của Admin không có trường AnhDaiDien
                var existingNv = await _context.NhanViens.AsNoTracking().FirstOrDefaultAsync(x => x.MaNv == id);
                if (existingNv != null && string.IsNullOrEmpty(nhanVien.AnhDaiDien))
                {
                    nhanVien.AnhDaiDien = existingNv.AnhDaiDien;
                }

                _context.Update(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaPhong"] = new SelectList(_context.PhongBans, "MaPhong", "TenPhong", nhanVien.MaPhong);
            return View("Sua", nhanVien);
        }

        // 5. Xóa Nhân viên (GET - Chỉ Admin)
        public async Task<IActionResult> Delete(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction(nameof(HoSoCaNhan));

            if (id == null) return NotFound();

            var nhanVien = await _context.NhanViens
                .Include(n => n.MaPhongNavigation)
                .FirstOrDefaultAsync(m => m.MaNv == id);

            if (nhanVien == null) return NotFound();

            return View("Xoa", nhanVien);
        }

        // POST: NhanVien/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return Unauthorized();

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}