using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    /// <summary>
    /// Sköter inloggning, registrering och lösenordsvalidering.
    /// Använder DataStorage för att läsa/spara användare.
    /// </summary>
    public class Authenticator
    {
        // Lista med alla registrerade hjältar, laddas vid start
        private static List<User> registeredUsers = DataStorage.LoadUsers();

        // Service för SMS-notiser (Twilio)
        private static NotificationService notifier = new NotificationService();

        // Testläge = inga riktiga SMS skickas
        private static bool testMode = true;

        // === REGISTRERA NY HJÄLTE ===
        public static void RegisterHero()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== HERO REGISTRATION ===\n");
            Console.ResetColor();

            // --- Användarnamn ---
            Console.Write("Enter hero name: ");
            string username = Console.ReadLine();

            // Kontrollera giltighet och att namnet inte redan finns
            if (string.IsNullOrWhiteSpace(username) ||
                registeredUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("❌ Invalid or taken username.");
                Console.ReadKey();
                return;
            }

            // --- Lösenord ---
            Console.Write("Enter password: ");
            string password = ReadPassword();

            if (!IsStrongPassword(password))
            {
                Console.WriteLine("\n❌ Weak password. Must include uppercase, number, and special char.");
                Console.ReadKey();
                return;
            }

            // --- Telefonnummer ---
            Console.Write("\nEnter phone number (+46...): ");
            string phone = Console.ReadLine();

            // Skapa användare och spara
            var newUser = new User(username, password, phone);
            registeredUsers.Add(newUser);
            DataStorage.SaveUsers(registeredUsers);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✅ Hero '{username}' registered successfully!");
            Console.ResetColor();
            Console.ReadKey();
        }

        // === INLOGGNING ===
        public static User LoginHero()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== HERO LOGIN ===\n");
            Console.ResetColor();

            Console.Write("Enter hero name: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = ReadPassword();

            // Hitta användare i listan
            var user = registeredUsers.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                Console.WriteLine("\n❌ Invalid credentials.");
                Console.ReadKey();
                return null;
            }

            // --- Generera 2FA-kod ---
            string code = new Random().Next(100000, 999999).ToString();

            if (testMode)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n[TESTMODE] 2FA code: {code}");
                Console.ResetColor();
            }
            else
            {
                notifier.SendSmsAsync(user.PhoneNumber, $"Your 2FA code is {code}").Wait();
            }

            Console.Write("\nEnter 2FA code: ");
            string input = Console.ReadLine();

            if (input == code)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Welcome, {user.Username}!");
                Console.ResetColor();
                Console.ReadKey();
                return user;
            }

            Console.WriteLine("\n❌ Wrong code.");
            Console.ReadKey();
            return null;
        }

        // === SPARA ALLA ÄNDRINGAR ===
        public static void SaveAll() => DataStorage.SaveUsers(registeredUsers);

        // === LÄS LÖSENORD UTAN ATT VISA DET ===
        private static string ReadPassword()
        {
            string password = "";
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
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
            return password;
        }

        // === LÖSENORDSVALIDERING ===
        private static bool IsStrongPassword(string pw)
        {
            return pw.Length >= 6 &&
                   pw.Any(char.IsUpper) &&
                   pw.Any(char.IsDigit) &&
                   Regex.IsMatch(pw, @"[!@#$%^&*(),.?""{}|<>]");
        }
    }
}
