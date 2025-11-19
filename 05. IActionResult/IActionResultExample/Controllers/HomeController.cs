using Microsoft.AspNetCore.Mvc;

namespace IActionResultExample.Controllers;

public class HomeController : Controller
{
   // IActionResult is the parent of all return Result types for action methods
   // we cannot return different Result types in same action method by using one Result Type
   // but we can return different Result type in same action method by using IActionResult

   // url: [Query String]
   // url:/book?isloggedin=true&bookid=1
   [Route("book")]
   public IActionResult Index()
   {
      //Book id should be supplied
      if (!Request.Query.ContainsKey("bookid"))
      {
         Response.StatusCode = 400;

         return Content("Book id is not supplied");
      }

      //Book id cannot be empty
      if (string.IsNullOrEmpty(Convert.ToString(Request.Query["bookid"])))
      {
         Response.StatusCode = 400;

         return Content("Book id can't be null or empty");
      }

      //Book id should be between 1 to 1000
      // the actual access of REquest object is: [ControllerContext.HttpContext.Request]
      int bookId = Convert.ToInt16(ControllerContext.HttpContext.Request.Query["bookid"]);
      if (bookId <= 0)
      {
         Response.StatusCode = 400;

         return Content("Book id can't be less than or equal to zero");
      }
      if (bookId > 1000)
      {
         Response.StatusCode = 400;

         return Content("Book id can't be greater than 1000");
      }

      //isloggedin should be true
      if (Convert.ToBoolean(Request.Query["isloggedin"]) == false)
      {
         Response.StatusCode = 401;
         return Content("User must be authenticated");
      }

      return File("/sample.pdf", "application/pdf");
   }

}
