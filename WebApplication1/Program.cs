using System.Linq;
using System.Net.WebSockets;
using System.Text;

namespace WebApplication1
{
    public class Program
    {
        static List<User> users = new List<User>()
        { 
            new User{Name = "Makar", Age = 18, Login = "Nedoprofi", Password = "16050620"}
        };
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();


            app.Run(MiddleWare);

            app.Run();
        }
        #region MiddleWare
        public static async Task MiddleWare(HttpContext context)
        {
            var path = context.Request.Path;
            Console.WriteLine("path: " + path + " method: " + context.Request.Method);
            switch (path)
            {
                case "/": await SendPage(context, "Index.html"); break;

                case "/login": await SendPage(context, "Login.html"); break;

                case "/singup": await SendPage(context, "SingUp.html"); break;

                case "/profile": await SendPage(context, "Profile.html"); break;

                case "/users/data/login" when context.Request.Method == "POST" :await  CheckLoginUser(context); break;

                case "/users/data" when context.Request.Method == "POST": await AddUser(context); break;

                default: await SendPage(context, "404.html"); break;
            }
        }

        #endregion

        #region funcs
        public static async Task SendPage(HttpContext context, string fileName)
        {
            var responce = context.Response;
            responce.ContentType = "text/html";
            await responce.SendFileAsync("html/" + fileName);
        }

        public static async Task CheckLoginUser(HttpContext contex)
        {
            var responce = contex.Response;
            var request = contex.Request;
            try
            {
                User? user = await request.ReadFromJsonAsync<User>();
                if(user != null)
                    if (users.FirstOrDefault(x => x.Login == user.Login)?.Password == user.Password)
                        responce.StatusCode = 201;
                else
                    responce.StatusCode = 0;
                Console.WriteLine("end try, user is null?: " + user==null);
                Console.WriteLine("status code: " + responce.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responce.StatusCode = 404;
                await responce.WriteAsJsonAsync(new { message = ex.Message });
            }
        }

        public static async Task AddUser(HttpContext contex)
        {
            var responce = contex.Response;
            var request = contex.Request;

            try
            {
                User? user = await request.ReadFromJsonAsync<User>();
                if(user != null)
                {
                    Console.WriteLine($"name {user.Name}, age {user.Age}, login {user.Login}, password {user.Password}");
                    users.Add(user);
                    await responce.WriteAsJsonAsync(new {message = "user is added"});
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("error with add user");
                responce.StatusCode = 0;
                await responce.WriteAsJsonAsync(new { message = "error with convert json", exeption = ex.Message });
            }
        }
        #endregion

        class User
        {
            public string? Login { get; set; }
            public string? Name { get; set; }
            public int? Age { get; set; }
            public string? Password { get; set; }
        }
    }
}
