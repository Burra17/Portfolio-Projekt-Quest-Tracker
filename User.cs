
using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class User // En klass för att registrera och logga in användare
    {
        // --- Egenskaper ---
        public string Username { get; set; }
        public string Password { get; set; }

        // --- Konstruktor ---
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        // --- "Databas" i minnet ---
        private static List<User> registeredUsers = new List<User>();

        // --- Registrera ny hjälte ---
        public static void RegisterHero()
        {
            Console.Write("Enter hero name (username): ");
            string username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username)) // Om användaren lämnar blankt
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            if (registeredUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))) // Kolla så att det inte redan finns
            {
                Console.WriteLine("Username already exists. Choose another one.");
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (!IsStrongPassword(password)) // Om Lösenordet inte är tillräckligt starkt, använder metoden som finns längre ner
            {
                Console.WriteLine("Password must be at least 6 characters long, include one number, one uppercase letter, and one special character.");
                return;
            }

            registeredUsers.Add(new User(username, password)); // Skapa en ny användare och lägg till i listan
            Console.WriteLine($"Hero '{username}' registered successfully!\n"); // Skriva ut till användaren
        }

        // --- Logga in hjälte ---
        public static User LoginHero()
        {
            Console.Write("Enter hero name: "); 
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            User foundUser = registeredUsers.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password); // Check för att se om det är lika som registrerad användare

            if (foundUser == null) // Om det inte hittas
            {
                Console.WriteLine("Invalid username or password."); // Skriv ut
                return null;
            }

            Console.WriteLine($"Access granted! Welcome back, {foundUser.Username}!\n"); // Användaren lyckades med inloggningen
            return foundUser; //Returnera founduser
        }

        // --- Lösenordskontroll ---
        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false; // Kollar så att det inte är tomt

            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]");

            return password.Length >= 6 && hasUpper && hasDigit && hasSpecial; //returnerar lösen om det innehåller alla krav
        }
    }
}