using Microsoft.AspNetCore.Mvc;

namespace IActionResultExample.Controllers;

public class StoreController : Controller
{
   [Route("store/books/{id}")]
   public IActionResult Books(int id)
   {
      return Content($"<h1>The id of book: {id}</h1>");
   }

}
