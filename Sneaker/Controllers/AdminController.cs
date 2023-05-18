using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sneaker.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace Sneaker.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        DataYikesDataContext db = new DataYikesDataContext();
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Admin");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["TaiKhoanAdmin"] = ad;
                    return RedirectToAction("DonDatHang", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult SanPham(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.SanPhams.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemSanPham()
        {
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "MaThuongHieu", "TenThuongHieu");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSanPham(SanPham SanPham, HttpPostedFileBase fileupload)
        {
            //đưa dữ liệu vào dropdownload
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "MaThuongHieu", "TenThuongHieu");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh đại diện";
                return View(SanPham);
            }
            // thêm vào csdl
            else
            {
                if (ModelState.IsValid)
                {
                    //lưu tên file , lưu ý bổ sung thư việng using System.IO
                    var filename = Path.GetFileName(fileupload.FileName);
                    // lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Assets/Images/Products/"), filename);
                    // kiểm tra hình tồn tại chưa?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // lưu hình ảnh vào  đường dẫn
                        fileupload.SaveAs(path);
                    }
                    SanPham.AnhDD = filename;
                    db.SanPhams.InsertOnSubmit(SanPham);
                    db.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }
        public ActionResult ChiTietSanPham(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = SanPham.MaSP;
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(SanPham);
        }
        public ActionResult XoaSanPham(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = SanPham.MaSP;
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(SanPham);
        }
        [HttpPost, ActionName("XoaSanPham")]
        public ActionResult XacNhanXoa(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = SanPham.MaSP;
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SanPhams.DeleteOnSubmit(SanPham);
            db.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        public ActionResult SuaSanPham(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "MaThuongHieu", "TenThuongHieu", SanPham.MaThuongHieu);
            return View(SanPham);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSanPham(SanPham SanPham)
        {
            SanPham itemm = db.SanPhams.SingleOrDefault(n => n.MaSP == SanPham.MaSP);
            itemm.TenSP = SanPham.TenSP;
            itemm.GiaBan = SanPham.GiaBan;
            itemm.MoTa = SanPham.MoTa;
            db.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        public ActionResult ThuongHieu(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.ThuongHieus.ToList().OrderBy(n => n.MaThuongHieu).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemThuongHieu()
        {
            return View();
        }
        public ActionResult SuaThuongHieu(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaThuongHieu(ThuongHieu ThuongHieu)
        {
            ThuongHieu itemm = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == ThuongHieu.MaThuongHieu);
            itemm.TenThuongHieu = ThuongHieu.TenThuongHieu;
            db.SubmitChanges();
            return RedirectToAction("ThuongHieu");
        }
        public ActionResult ChiTietThuongHieu(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);
            ViewBag.MaThuongHieu = item.MaThuongHieu;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult XoaThuongHieu(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);
            ViewBag.MaThuongHieu = item.MaThuongHieu;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaThuongHieu")]
        public ActionResult XacNhanXoa1(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);
            ViewBag.MaThuongHieu = item.MaThuongHieu;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.ThuongHieus.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("ThuongHieu");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemThuongHieu(ThuongHieu item)
        {
            db.ThuongHieus.InsertOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("ThuongHieu");
        }
        public ActionResult KhachHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.KhachHangs.ToList().OrderBy(n => n.MaKH).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult XoaKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaKH")]
        public ActionResult XacNhanXoa2(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.KhachHangs.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("KhachHang");
        }
        public ActionResult ChiTietKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult SuaKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaKH(KhachHang kh)
        {
            KhachHang itemm = db.KhachHangs.SingleOrDefault(n => n.MaKH == kh.MaKH);
            itemm.HoTen = kh.HoTen;
            itemm.TaiKhoan = kh.TaiKhoan;
            itemm.MatKhau = kh.MatKhau;
            itemm.Email = kh.Email;
            itemm.DiaChiKH = kh.DiaChiKH;
            itemm.DienThoaiKH = kh.DienThoaiKH;
            db.SubmitChanges();
            return RedirectToAction("KhachHang");
        }
        public ActionResult DonDatHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.DonDatHangs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SuaDDH(int id)
        {
            DonDatHang item = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaDDH(DonDatHang ddh)
        {
            DonDatHang itemm = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == ddh.MaDonHang);
            db.SubmitChanges();
            return RedirectToAction("DonDatHang");
        }
        public ActionResult ChiTietDH(int id)
        {
            ChiTietDatHang item = db.ChiTietDatHangs.FirstOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = item.MaDonHang;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult XoaDDH(int id)
        {
            DonDatHang item = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = item.MaDonHang;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaDDH")]
        public ActionResult XacNhanXoa3(int id)
        {
            DonDatHang item = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = item.MaDonHang;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.DonDatHangs.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("DonDatHang");
        }

        //Các bước nhúng ckediter
        //1.Tải bộ plugin và cho vào project
        //2.kéo file Js vào layout
        //3.Thay đổi input nội dung thành textarea, có đặt id cho input
        //4.Viết lệnh Js cho textarea
        //5.Luư dữ liệu: - Tắt kiểm tra HTML cho Action lưu dữ liệu

        //Sử dụng CKFinder
        //Bước1: Tải bộ plugin và cho vào project
        //Bước 2: Kéo Js vào Layout
        //Bước 3: Thêm DLL thư viện vào projey
        //Bước 4:Cấu hình
        //Bước 5: Sử dụng
    }
}
