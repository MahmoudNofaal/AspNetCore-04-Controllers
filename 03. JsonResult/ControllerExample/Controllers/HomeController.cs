using ControllerExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControllerExample.Controllers;


[Controller]
public class HomeController : Controller
{


   //[Route("home")]
   //[Route("/")]
   //public ContentResult Index()
   //{
   //   return new ContentResult()
   //   {
   //      Content = "Hello from Index",
   //      ContentType = "text/plain",
   //      StatusCode = 301

   //   };

   //}

   [Route("home")]
   [Route("/")]
   public IActionResult Index()
   {
      //return Content("Hello from Index", "text/plain");

      return Content("<h1>Welcome</h1> <h2>Hello from Index</h2>", "text/html");
   }


   [Route("person")]
   public JsonResult Person()
   {
      var p = new Person()
      {
         Id = Guid.NewGuid(),
         FirstName = "John",
         LastName = "Doe",
         Age = 26
      };

      //return new JsonResult(p);
      return Json(p);
   }


   [Route("contact-us/{mobile:regex(^\\d{{10}}$)}")]
   public string Contact()
   {
      return "Hello from Contact";
   }

}
