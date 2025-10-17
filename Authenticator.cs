using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class Authenticator
    {
        // Lista över registrerade användare (lagras i minnet, ej i databas)
        private static List<User> registeredUsers = new List<User>();

        // NotificationService används för att kunna skicka SMS via t.ex. Twilio
        private static NotificationService notifier = new NotificationService();

        // 🧪 Testläge — sätt till true för att INTE skicka riktiga SMS vid inloggning
        private static bool testMode = true;

        // =============================
        //        REGISTRERING
        // =============================
        public static void RegisterHero()
        {
            // --- Be användaren skriva in ett användarnamn ---
            Console.Write("Enter hero name (username): ");
            string username = Console.ReadLine();

            // Kontrollera att fältet inte är tomt
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            // Kolla om användarnamnet redan finns (case-insensitive)
            if (registeredUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Username already exists. Choose another one.");
                return;
            }

            // --- Be användaren skriva in ett lösenord ---
            Console.Write("Enter password: ");
            string password = "";

            // Läs ett tecken i taget, och visa '*' i stället för själva tecknet
            while (true)
            {
                var key = Console.ReadKey(true); // true = visa inte tangent i konsolen

                if (key.Key == ConsoleKey.Enter) // Användaren tryckte Enter -> klart
                    break;
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    // Ta bort sista tecknet om användaren trycker Backspace
                    password = password[..^1];
                    Console.Write("\b \b"); // Ta bort en stjärna visuellt
                }
                else
                {
                    // Lägg till tecknet i lösenordet och visa en stjärna
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            // Kontrollera att lösenordet uppfyller kraven (metoden finns längre ner)
            if (!IsStrongPassword(password))
            {
                Console.WriteLine("\nPassword must be at least 6 characters long, include one number, one uppercase letter, and one special character.");
                return;
            }

            // --- Be användaren skriva in sitt telefonnummer ---
            Console.Write("\nEnter your phone number (+46..): ");
            string phone = Console.ReadLine();

            // Kontrollera att numret inte är tomt och börjar med landskod (+)
            if (string.IsNullOrWhiteSpace(phone) || !phone.StartsWith("+"))
            {
                Console.WriteLine("Invalid phone number format. Must start with + and include country code.");
                return;
            }

            // Skapa ny användare och lägg till i listan
            registeredUsers.Add(new User(username, password, phone));
            Console.WriteLine($"\nHero '{username}' registered successfully!\n");
        }

        // =============================
        //          INLOGGNING
        // =============================
        public static User LoginHero()
        {
            // --- Be användaren skriva in sina inloggningsuppgifter ---
            Console.Write("Enter hero name: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = "";

            // Läs ett tecken i taget, och visa '*' i stället för själva tecknet
            while (true)
            {
                var key = Console.ReadKey(true); // true = visa inte tangent i konsolen

                if (key.Key == ConsoleKey.Enter) // Användaren tryckte Enter -> klart
                    break;
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    // Ta bort sista tecknet om användaren trycker Backspace
                    password = password[..^1];
                    Console.Write("\b \b"); // Ta bort en stjärna visuellt
                }
                else
                {
                    // Lägg till tecknet i lösenordet och visa en stjärna
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            // Hitta användaren i listan baserat på användarnamn och lösenord
            User foundUser = registeredUsers.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            // Om ingen användare hittas -> felmeddelande
            if (foundUser == null)
            {
                Console.WriteLine("Invalid username or password.");
                return null;
            }

            // --- Skapa en 6-siffrig kod för tvåfaktorsautentisering ---
            string code = new Random().Next(100000, 999999).ToString();

            if (testMode)
            {
                // 🧪 TESTLÄGE: Hoppa över SMS och skriv ut koden direkt i konsolen
                Console.WriteLine($"[TESTMODE] 2FA code for {foundUser.Username}: {code}");
            }
            else
            {
                // PRODUKTION: Skicka koden via SMS med NotificationService
                Console.WriteLine("Sending verification code via SMS...");
                notifier.SendSmsAsync(foundUser.PhoneNumber, $"🛡️ Your 2FA code is: {code}").Wait();
            }

            // --- Be användaren mata in koden ---
            Console.Write("Enter the 2FA code you received: ");
            string inputCode = Console.ReadLine();

            // Kontrollera att koden stämmer
            if (inputCode == code)
            {
                Console.WriteLine($"Access granted! Welcome back, {foundUser.Username}.");
                return foundUser;
            }

            // Om koden är felaktig
            Console.WriteLine("Incorrect verification code.");
            return null;
        }

        // =============================
        //   LÖSENORDSKRAV / VALIDERING
        // =============================
        private static bool IsStrongPassword(string password)
        {
            // Returnera false om lösenordet är tomt
            if (string.IsNullOrEmpty(password)) return false;

            // Kontrollera att lösenordet innehåller minst:
            bool hasUpper = password.Any(char.IsUpper); // en versal
            bool hasDigit = password.Any(char.IsDigit); // en siffra
            bool hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]"); // ett specialtecken

            // Lösenordet måste vara minst 6 tecken långt och uppfylla alla krav
            return password.Length >= 6 && hasUpper && hasDigit && hasSpecial;
        }
    }
}
