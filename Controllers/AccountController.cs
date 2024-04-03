using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Demo.Models;



namespace RegistrationAndLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly string connectionString = "Data Source=Registration.db";

        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Register(UserModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // Check if the email already exists
                    var commandCheck = connection.CreateCommand();
                    commandCheck.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    commandCheck.Parameters.AddWithValue("@Email", model.Email);
                    int count = Convert.ToInt32(commandCheck.ExecuteScalar());


                    if (count > 0)
                    {
                        TempData["RegistrationMessage"] = "Email already exists. Please login or use a different email or password.";
                        return RedirectToAction("Login"); // Redirect to Login action
                    }

                    // Email doesn't exist, proceed with registration
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Users (UserName, Email, Password) VALUES (@UserName, @Email, @Password)";
                    command.Parameters.AddWithValue("@UserName", model.UserName);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Password", model.Password);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel model)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                command.Parameters.AddWithValue("@Email", model.Email);
                command.Parameters.AddWithValue("@Password", model.Password);

                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    return Redirect("http://localhost:5093/");
                }
                else
                {
                    TempData["LoginMessage"] = "You are login with Wrong Email Or Password !... Please login with different email or password.";
                    return View("Login", model);
                }
            }
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Users()
        {
            List<UserModel> users = new List<UserModel>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            UserName = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3)
                        };
                        users.Add(user);
                    }
                }
            }

            return View(users);
        }
    }
}
