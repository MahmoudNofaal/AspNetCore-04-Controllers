using Microsoft.AspNetCore.Mvc;

namespace ControllerExample.Controllers;

// CONTROLLER
// OPTIONALLLY :
// - IS A PUBLIC CLASS.
// - INHERITED FROM [Microsoft.AspNetCore.Mvc.Controller]

// it should be suffix with word Controller
// or we can use [Controller] attribute 
[Controller]
public class HomeController : Controller
{
   // we can add multiple routes to same action method and routing to it with different routes
   [Route("sayhello")]
   [Route("sayhello2")]
   [Route("/")]
   public string method1()
   {
      return "Hello from method1";
   }


   [Route("home")]
   public string Index()
   {
      return "Hello from Index";
   }


   [Route("about")]
   public string About()
   {
      return "Hello from About";
   }


   [Route("contact-us/{mobile:regex(^\\d{{10}}$)}")]
   public string Contact()
   {
      return "Hello from Contact";
   }

}
