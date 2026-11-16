using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly QlnhanSuDbContext _context;

        public TaiKhoanController(QlnhanSuDbContext context)
        {
            _context = context;
        }

        // Hàm kiểm tra quyền Admin để chặn User truy cập trực tiếp qua URL
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // 1. Danh sách tài khoản hệ thống
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            // Chặn User gõ thẳng link /TaiKhoan
            if (!IsAdmin())
                return RedirectToAction("HoSoCaNhan", "NhanVien");

            var danhSach = await _context.TaiKhoans
                .Include(t => t.MaNvNavigation)
                .ToListAsync();

            return View(danhSach);
        }

        // 2. Cấp tài khoản mới (GET)
        [HttpGet]
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            // Chặn User gõ thẳng link /TaiKhoan/Create
            if (!IsAdmin())
                return RedirectToAction("HoSoCaNhan", "NhanVien");

            // Chỉ lấy danh sách nhân viên CHƯA có tài khoản để tránh cấp trùng
            var nvDaCoTk = _context.TaiKhoans.Where(t => t.MaNv != null).Select(t => t.MaNv).ToList();
            var nvChuaCoTk = _context.NhanViens.Where(n => !nvDaCoTk.Contains(n.MaNv)).ToList();

            ViewData["MaNv"] = new SelectList(nvChuaCoTk, "MaNv", "HoTen");
            return View();
        }

        // POST: Cấp tài khoản mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,HoTen,Role,MaNv")] TaiKhoan taiKhoan)
        {
            if (!IsAdmin()) return Unauthorized();

            if (_context.TaiKhoans.Any(t => t.Username == taiKhoan.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
            }

            if (ModelState.IsValid)
            {
                // Nếu chọn liên kết nhân viên mà bỏ trống Họ tên -> tự lấy tên nhân viên điền vào
                if (taiKhoan.MaNv != null && string.IsNullOrEmpty(taiKhoan.HoTen))
                {
                    var nv = await _context.NhanViens.FindAsync(taiKhoan.MaNv);
                    taiKhoan.HoTen = nv?.HoTen;
                }

                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã cấp tài khoản '{taiKhoan.Username}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            var nvDaCoTk = _context.TaiKhoans.Where(t => t.MaNv != null).Select(t => t.MaNv).ToList();
            var nvChuaCoTk = _context.NhanViens.Where(n => !nvDaCoTk.Contains(n.MaNv)).ToList();
            ViewData["MaNv"] = new SelectList(nvChuaCoTk, "MaNv", "HoTen", taiKhoan.MaNv);
            return View(taiKhoan);
        }

        // 3. Sửa / Đặt lại mật khẩu tài khoản (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            // Chặn User gõ thẳng link /TaiKhoan/Edit/...
            if (!IsAdmin())
                return RedirectToAction("HoSoCaNhan", "NhanVien");

            if (string.IsNullOrEmpty(id)) return NotFound();

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "HoTen", taiKhoan.MaNv);
            return View(taiKhoan);
        }

        // POST: Sửa / Đặt lại mật khẩu tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,Password,HoTen,Role,MaNv")] TaiKhoan taiKhoan)
        {
            if (!IsAdmin()) return Unauthorized();
            if (id != taiKhoan.Username) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã cập nhật tài khoản '{taiKhoan.Username}'!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "HoTen", taiKhoan.MaNv);
            return View(taiKhoan);
        }

        // 4. Xóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsAdmin()) return Unauthorized();

            // Chặn Admin tự xóa chính tài khoản đang đăng nhập
            if (id == HttpContext.Session.GetString("Username"))
            {
                TempData["Error"] = "Không thể xóa tài khoản Admin đang đăng nhập!";
                return RedirectToAction(nameof(Index));
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa tài khoản '{id}'!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}