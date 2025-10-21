using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class Authenticator
    {
        // Lista över registrerade användare (lagras i minnet)
        private static List<User> registeredUsers = new List<User>();

        // NotificationService används för att kunna skicka SMS via t.ex. Twilio
        private static NotificationService notifier = new NotificationService();

        // 🧪 Testläge — true = inga riktiga SMS skickas (kod visas direkt i terminalen)
        private static bool testMode = true;

        // =============================
        //        REGISTRERING
        // =============================
        public static void RegisterHero()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("HERO REGISTRATION");
            Console.WriteLine("======================\n");
            Console.ResetColor();

            try
            {
                // --- Användarnamn ---
                Console.Write("Enter hero name (username): ");
                string username = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Username cannot be empty.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }

                // Kontrollera om namnet redan finns
                if (registeredUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Username already exists. Choose another one.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }

                // --- Lösenord ---
                Console.Write("Enter password: ");
                string password = "";
                while (true)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Enter)
                        break;
                    else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password[..^1];
                        Console.Write("\b \b");
                    }
                    else
                    {
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                }

                // Validera lösenord
                if (!IsStrongPassword(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPassword must be at least 6 characters long, include one number, one uppercase letter, and one special character.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }

                // --- Telefonnummer ---
                Console.Write("\nEnter your phone number (+46..): ");
                string phone = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(phone) || !phone.StartsWith("+"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid phone number format. Must start with + and include country code.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }

                // Skapa användare
                registeredUsers.Add(new User(username, password, phone));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nHero '{username}' registered successfully!");
                Console.ResetColor();

                Console.WriteLine("\nPress any key to return to main menu...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
            }
        }

        // =============================
        //          INLOGGNING
        // =============================
        public static User LoginHero()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("HERO LOGIN");
            Console.WriteLine("======================\n");
            Console.ResetColor();

            try
            {
                // --- Användarnamn ---
                Console.Write("Enter hero name: ");
                string username = Console.ReadLine();

                // --- Lösenord ---
                Console.Write("Enter password: ");
                string password = "";
                while (true)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Enter)
                        break;
                    else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password[..^1];
                        Console.Write("\b \b");
                    }
                    else
                    {
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                }

                // Kolla om användaren finns
                User foundUser = registeredUsers.FirstOrDefault(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password);

                if (foundUser == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid username or password.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return null;
                }

                // --- Skicka/verifiera 2FA-kod ---
                string code = new Random().Next(100000, 999999).ToString();

                if (testMode)
                {
                    // 🧪 Testläge — visa koden direkt
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"\n[TESTMODE] 2FA code for {foundUser.Username}: {code}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("\nSending verification code via SMS...");
                    notifier.SendSmsAsync(foundUser.PhoneNumber, $"Your 2FA code is: {code}").Wait();
                }

                Console.Write("\nEnter the 2FA code: ");
                string inputCode = Console.ReadLine();

                if (inputCode == code)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nAccess granted! Welcome back, {foundUser.Username}!");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return foundUser;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n❌ Incorrect verification code.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return null;
            }
        }

        // =============================
        //   LÖSENORDSKRAV / VALIDERING
        // =============================
        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            bool hasUpper = password.Any(char.IsUpper); // minst en versal
            bool hasDigit = password.Any(char.IsDigit); // minst en siffra
            bool hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]"); // minst ett specialtecken

            return password.Length >= 6 && hasUpper && hasDigit && hasSpecial;
        }
    }
}
