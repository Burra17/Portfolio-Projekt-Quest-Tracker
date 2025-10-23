using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Portfolio_Projekt_Quest_Tracker
{
    /// <summary>
    /// Hanterar läsning och skrivning av data till en JSON-fil.
    /// Lagrar alla användare och deras quests så att allt sparas mellan sessioner.
    /// </summary>
    public static class DataStorage
    {
        private static readonly string FilePath = "heroes.json"; // Filnamn för sparad data

        /// <summary>
        /// Läser in alla registrerade användare från JSON-filen.
        /// Om filen saknas eller är korrupt returneras en tom lista.
        /// </summary>
        public static List<User> LoadUsers()
        {
            if (!File.Exists(FilePath))
                return new List<User>();

            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch
            {
                Console.WriteLine("Failed to load heroes. Starting with empty list...");
                return new List<User>();
            }
        }

        /// <summary>
        /// Sparar alla användare och deras quests till JSON-filen.
        /// </summary>
        public static void SaveUsers(List<User> users)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(users, options);
            File.WriteAllText(FilePath, json);
        }
    }
}
