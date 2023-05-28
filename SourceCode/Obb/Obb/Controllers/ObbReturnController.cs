using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Obb.Models;
using Obb.Services;

namespace Obb.Controllers
{
    [Authorize]
    public class ObbReturnController : Controller
    {
        private readonly IObbBookService _service;
        public ObbReturnController(IObbBookService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index(string userID)
        {
            IList<ObbBook> Result;
            Result = _service.ReadBorrowBookData(userID);
            ViewBag.UserID = userID;

            return View("Index", Result);
        }

        [HttpPost]
        public IActionResult ReturnBook(ObbBook obbBook)
        {
            ObbBook Result;
            Result = _service.ReturnBook(obbBook);

            return Json(Result);
        }
    }
}
