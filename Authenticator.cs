
using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class Authenticator
    {
        //Lista med registrerarade användare (i minnet)
        private static List<User> registeredUsers = new List<User>();

        //NotificationService för att skicka SMS
        private static NotificationService notifier = new NotificationService();

        // --- Registrera ny hjälte ---
        public static void RegisterHero()
        {
            //Be om användarnamn
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

            //Be om lösenord
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (!IsStrongPassword(password)) // Om Lösenordet inte är tillräckligt starkt, använder metoden som finns längre ner
            {
                Console.WriteLine("Password must be at least 6 characters long, include one number, one uppercase letter, and one special character.");
                return;
            }

            //Be om telefonnummer
            Console.Write("Enter you phone number (+46..): ");
            string phone = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(phone) || !phone.StartsWith("+")) //Kollar om det är tomt och startar med +
            {
                Console.WriteLine("Invalid phone number format. Must start with + and include country code.");
                return;
            }

            registeredUsers.Add(new User(username, password, phone)); // Skapa en ny användare och lägg till i listan
            Console.WriteLine($"Hero '{username}' registered successfully!\n"); // Skriva ut till användaren
        }

        // --- Logga in hjälte med 2FA---
        public static User LoginHero()
        {
            //Be om namn och lösenord
            Console.Write("Enter hero name: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            //Kontrollera om användare finns och om lösenordet stämmer
            User foundUser = registeredUsers.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (foundUser == null) //Om användare inte hittas
            {
                Console.WriteLine("Invalid username or password.");
                return null;
            }

            // Generera 6-siffrig kod
            string code = new Random().Next(100000, 999999).ToString();

            // Skicka SMS
            Console.WriteLine("Sending verification code via SMS...");
            notifier.SendSmsAsync(foundUser.PhoneNumber, $"🛡️ Your 2FA code is: {code}").Wait();

            // Be användaren ange koden direkt
            Console.Write("Enter the 2FA code you received: ");
            string inputCode = Console.ReadLine();

            //Kollar om den genererade koden stämmer med vad användare skriver in
            if (inputCode == code)
            {
                Console.WriteLine($"Access granted! Welcome back, {foundUser.Username}.");
                return foundUser;
            }

            //Felaktig kod
            Console.WriteLine("Incorrect verification code.");
            return null;
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
