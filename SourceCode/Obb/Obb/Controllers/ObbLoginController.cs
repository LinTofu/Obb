using Microsoft.AspNetCore.Mvc;
using Obb.Models;
using System.Diagnostics;
using Obb.Services;
using XAct.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using XAct.Users;

namespace Obb.Controllers
{
    
    public class ObbLoginController : Controller
    {
        private readonly IObbLoginService _service;
        public ObbLoginController(IObbLoginService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            ObbUser Result = new ObbUser();

            return View("Index", Result);
        }

        // 登入 身分驗證
        [HttpPost]
        public async Task<IActionResult> Index(string phoneNo, string password, string returnUrl)
        {
            ObbUser Result = new ObbUser();
            Result.PhoneNo = phoneNo;
            Result.Password = password;
            Result = _service.LoginCheck(Result);
            if(Result.ErrorMsg == null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, phoneNo)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Login");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return Redirect(returnUrl == null ? "/ObbBorrow/Index?userID=" + Result.UserID : returnUrl);
            }
            else
            {
                return View();
            }
        }

        // 登出 註銷身分驗證
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "ObbLogin");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}