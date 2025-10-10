
using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class User // En klass för att registrera och logga in användare, som innehåller email eller phone 2FA
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ContactInfo { get; set; }

        // Konstruktor
        public User(string username, string password, string contactInfo)
        {
            Username = username;
            Password = password;
            ContactInfo = contactInfo;
        }
    

            // --- Statisk lista som "databas" ---
        private static List<User> registeredUsers = new List<User>();

        // --- Registrering ---
        public static void RegisterHero()
        {
            Console.Write("Enter hero name (username): ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            // Kontrollera lösenordets styrka
            if (!IsStrongPassword(password))
            {
                Console.WriteLine("Password must be at least 6 characters long, include one number, one uppercase letter, and one special character.");
                return;
            }

            Console.Write("Enter Email or Phone for 2FA: ");
            string contact = Console.ReadLine();

            // Skapa hjälten och spara i listan
            registeredUsers.Add(new User(username, password, contact));
            Console.WriteLine($"Hero '{username}' has been registered successfully!\n");
        }

        // --- Inloggning ---
        public static User LoginHero()
        {
            Console.Write("Enter hero name: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            // Leta upp användaren
            User foundUser = registeredUsers.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (foundUser == null)
            {
                Console.WriteLine("Invalid username or password.");
                return null;
            }

            // Simulerad 2FA
            string code = Generate2FACode();
            Console.WriteLine($"[2FA CODE SENT TO {foundUser.ContactInfo}] (for test: {code})");

            Console.Write("Enter the 2FA code: ");
            string enteredCode = Console.ReadLine();

            if (enteredCode == code)
            {
                Console.WriteLine("Access granted! Welcome to the Guild!\n");
                return foundUser;
            }
            else
            {
                Console.WriteLine("Incorrect 2FA code. Access denied.");
                return null;
            }
        }

        // --- Hjälpmetoder ---

        // Kontroll av lösenordsstyrka
        private static bool IsStrongPassword(string password)
        {
            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]");
            return password.Length >= 6 && hasUpper && hasDigit && hasSpecial;
        }

        // Slumpa fram en 2FA-kod
        private static string Generate2FACode()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString(); // 6-siffrig kod
        }
    }
}

