using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSu.Models;

public class AccountController : Controller
{
    private readonly QlnhanSuDbContext _context;
    public AccountController(QlnhanSuDbContext context) { _context = context; }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.TaiKhoans.FirstOrDefault(x => x.Username == username && x.Password == password);
        if (user != null)
        {
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("HoTen", user.HoTen ?? user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");

            // Lưu mã nhân viên vào Session nếu tài khoản có liên kết
            if (user.MaNv != null)
            {
                HttpContext.Session.SetInt32("MaNV", user.MaNv.Value);
            }

            // Phân luồng sau khi đăng nhập
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "NhanVien");
            }
            else
            {
                return RedirectToAction("HoSoCaNhan", "NhanVien");
            }
        }
        ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
        return View();
    }

    // GET: Giao diện Đổi mật khẩu
    [HttpGet]
    public IActionResult DoiMatKhau()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
        {
            return RedirectToAction("Login");
        }
        return View();
    }

    // POST: Xử lý Đổi mật khẩu
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login");
        }

        var user = _context.TaiKhoans.FirstOrDefault(x => x.Username == username);
        if (user == null) return NotFound();

        if (user.Password != matKhauCu)
        {
            ViewBag.Error = "Mật khẩu hiện tại không chính xác!";
            return View();
        }

        if (matKhauMoi != xacNhanMatKhau)
        {
            ViewBag.Error = "Xác nhận mật khẩu mới không khớp!";
            return View();
        }

        user.Password = matKhauMoi;
        _context.Update(user);
        _context.SaveChanges();

        ViewBag.Success = "Đổi mật khẩu thành công!";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}