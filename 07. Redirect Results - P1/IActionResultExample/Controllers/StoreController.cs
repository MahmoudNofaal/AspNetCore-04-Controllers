using Microsoft.AspNetCore.Mvc;

namespace IActionResultExample.Controllers;

public class StoreController : Controller
{
   [Route("store/books")]
   public IActionResult Books()
   {
      return File("/sample.pdf", "application/pdf");
   }

}
