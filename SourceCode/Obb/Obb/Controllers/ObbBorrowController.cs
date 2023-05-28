using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Obb.Models;
using Obb.Services;

namespace Obb.Controllers
{
    [Authorize]
    public class ObbBorrowController : Controller
    {
        private readonly IObbBookService _service;
        public ObbBorrowController(IObbBookService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index(string userID)
        {
            IList<ObbBook> Result;
            Result = _service.ReadBookData(userID);
            _service.UpdateLastLoginDateTime(userID);
            ViewBag.UserID = userID;

            return View("Index", Result);
        }

        [HttpPost]
        public IActionResult BorrowBook(ObbBook obbBook)
        {
            ObbBook Result;
            Result = _service.BorrowBook(obbBook);

            return Json(Result);
        }
    }
}
