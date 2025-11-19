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


   [Route("file-download")]
   public VirtualFileResult FileDownload()
   {
      //return new VirtualFileResult("/sample.pdf", "application/pdf");

      return File("/sample.pdf", "application/pdf");
   }

   [Route("file-download2")]
   public PhysicalFileResult FileDownload2()
   {
      //return new PhysicalFileResult("D:\\WORK\\AI-Native Software Development\\GitHub Repo\\DotNET(Deep-Dive)\\AspNetCore Mastery\\04. Controllers & IActionResult\\04. File Results\\ControllerExample\\wwwroot\\sample.pdf", "application/pdf");
      return PhysicalFile("D:\\WORK\\AI-Native Software Development\\GitHub Repo\\DotNET(Deep-Dive)\\AspNetCore Mastery\\04. Controllers & IActionResult\\04. File Results\\ControllerExample\\wwwroot\\sample.pdf", "application/pdf");
   }

   [Route("file-download3")]
   public FileContentResult FileDownload3()
   {
      byte[] bytes = System.IO.File.ReadAllBytes("D:\\WORK\\AI-Native Software Development\\GitHub Repo\\DotNET(Deep-Dive)\\AspNetCore Mastery\\04. Controllers & IActionResult\\04. File Results\\ControllerExample\\wwwroot\\sample.pdf");

      //return new FileContentResult(bytes, "application/pdf");
      return File(bytes, "application/pdf");
   }

   [Route("contact-us/{mobile:regex(^\\d{{10}}$)}")]
   public string Contact()
   {
      return "Hello from Contact";
   }

}
