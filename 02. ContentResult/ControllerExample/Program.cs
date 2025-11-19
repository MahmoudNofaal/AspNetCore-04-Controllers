using ControllerExample.Controllers;

namespace ControllerExample;

public class Program
{
   public static void Main(string[] args)
   {
      var builder = WebApplication.CreateBuilder(args);
      

      // instead of added each controller like this
      //builder.Services.AddTransient<HomeController>();

      // we can use addcontrollers, this adds all classes controllers as services
      builder.Services.AddControllers();

      var app = builder.Build();

      app.UseRouting();

      // enabling the routing for each action methods
      // this method detect all the controllers of entire project,
      // and it will pick up all action methods
      // the routing will be added at a time by this single statement
      app.MapControllers();

      app.Run();
   }
}
