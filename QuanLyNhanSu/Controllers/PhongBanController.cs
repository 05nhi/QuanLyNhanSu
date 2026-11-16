using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Controllers
{
    public class PhongBanController : Controller
    {
        private readonly QlnhanSuDbContext _context;

        public PhongBanController(QlnhanSuDbContext context)
        {
            _context = context;
        }

        // Hàm kiểm tra quyền Admin dùng chung
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // 1. Danh sách phòng ban
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            if (!IsAdmin())
                return RedirectToAction("HoSoCaNhan", "NhanVien");

            var danhSach = await _context.PhongBans
                .Include(p => p.NhanViens)
                .ToListAsync();

            return View(danhSach);
        }

        // 2. Thêm mới phòng ban (GET)
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("HoSoCaNhan", "NhanVien");
            return View();
        }

        // POST: Thêm mới phòng ban
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenPhong,MoTa")] PhongBan phongBan)
        {
            if (!IsAdmin()) return Unauthorized();

            if (ModelState.IsValid)
            {
                _context.Add(phongBan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm phòng ban mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(phongBan);
        }

        // 3. Chỉnh sửa phòng ban (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("HoSoCaNhan", "NhanVien");
            if (id == null) return NotFound();

            var phongBan = await _context.PhongBans.FindAsync(id);
            if (phongBan == null) return NotFound();

            return View(phongBan);
        }

        // POST: Chỉnh sửa phòng ban
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPhong,TenPhong,MoTa")] PhongBan phongBan)
        {
            if (!IsAdmin()) return Unauthorized();
            if (id != phongBan.MaPhong) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(phongBan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật phòng ban thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(phongBan);
        }

        // 4. Xóa phòng ban (POST trực tiếp từ nút Xóa để thao tác nhanh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var phongBan = await _context.PhongBans
                .Include(p => p.NhanViens)
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phongBan == null) return NotFound();

            // Chặn xóa nếu phòng ban đang có nhân viên để tránh lỗi khóa ngoại SQL
            if (phongBan.NhanViens.Any())
            {
                TempData["Error"] = $"Không thể xóa phòng '{phongBan.TenPhong}' vì đang có {phongBan.NhanViens.Count} nhân viên trực thuộc!";
                return RedirectToAction(nameof(Index));
            }

            _context.PhongBans.Remove(phongBan);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa phòng ban thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}