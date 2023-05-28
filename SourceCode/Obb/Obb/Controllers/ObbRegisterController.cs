using Microsoft.AspNetCore.Mvc;
using Obb.Data;
using Obb.Models;
using Obb.Services;

namespace Obb.Controllers
{
    public class ObbRegisterController : Controller
    {
        private readonly ObbContext _Obbcontext;
        private readonly IObbLoginService _service;
        public ObbRegisterController(IObbLoginService service, ObbContext obbContext)
        {
            _service = service;
            _Obbcontext = obbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckPhoneNo(ObbUser obbUser)
        {
            ObbUser Result = _service.CheckPhoneNo(obbUser);

            return Json(Result);
        }

        [HttpPost]
        public IActionResult SaveData(ObbUser obbUser)
        {
            ObbUser Result = _service.SavaData(_Obbcontext, obbUser);

            return Json(Result);
        }
    }
}
