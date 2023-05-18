using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Sneaker.Models;
using Facebook;
using System.Configuration;
using BotDetect.Web.Mvc;

namespace Sneaker.Controllers
{
    public class MemberController : Controller
    {
        // GET: NguoiDung
        DataYikesDataContext data = new DataYikesDataContext();
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [CaptchaValidation("CaptchaCode", "registerCapcha", "Mã xác nhận không đúng!")]
        public ActionResult Register(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["HoTenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var matkhaunhaplai = collection["MatKhauNhapLai"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var dienthoai = collection["DienThoai"];
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["loi1"] = "Họ tên khách hàng không được để trống";

            }
            else if (string.IsNullOrEmpty(tendn))
            {
                ViewData["loi2"] = "Phải nhập tên đăng nhập";

            }
            else if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["loi3"] = "Phải nhập mật khẩu";
            }
            else if (string.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["loi4"] = "Phải nhập lại mật khẩu";
            }
            if (string.IsNullOrEmpty(diachi))
            {
                ViewData["loi5"] = "Địa chỉ không được bỏ trống";
            }
            if (string.IsNullOrEmpty(email))
            {
                ViewData["loi6"] = "Email không được bỏ trống";
            }
            if (string.IsNullOrEmpty(dienthoai))
            {
                ViewData["loi7"] = "Phải nhập điện thoại";
            }
            else
            {
                if(!ModelState.IsValid)
                {
                    ViewBag.Status = "Error";
                }
                else
                {
                    ViewBag.Status = "Success";
                    kh.HoTen = hoten;
                    kh.TaiKhoan = tendn;
                    kh.MatKhau = matkhau;
                    kh.Email = email;
                    kh.DiaChiKH = diachi;
                    kh.DienThoaiKH = dienthoai;
                    MvcCaptcha.ResetCaptcha("registerCapcha");
                    data.KhachHangs.InsertOnSubmit(kh);
                    data.SubmitChanges();
                    return RedirectToAction("Login");
                }
            }
            return this.Register();
        }
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.TaiKhoan == tendn && n.MatKhau == matkhau);
                if (kh != null)
                {
                    Session["TaiKhoan"] = kh;
                    Session["HoTen"] = kh.HoTen; // Thêm thông tin tên người dùng vào Session
                    return RedirectToAction("Index", "Home");
                }
                else
                    ViewBag.Thongbao = "TÊN ĐĂNG NHẬP HOẶC MẬT KHẨU KHÔNG ĐÚNG";
            }
            return View();
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallBack");
                return uriBuilder.Uri;
            }
        }
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallBack(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            if(!string.IsNullOrEmpty(accessToken)) 
            {
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;
            }
            else
            {

            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            Session["Cart"] = null;
            return RedirectToAction("Index", "Home");
        }

    }
}