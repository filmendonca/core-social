using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    //This controller redirects to the specific error view
    [Route("error/")]
    public class ErrorController : Controller
    {
        [Route("404")]
        public IActionResult ResourceNotFound() => View("NotFound");
        [Route("405")]
        public IActionResult MethodNotSupported() => View("Error", new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        [Route("500")]
        public IActionResult InternalServerError() => View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
