using Microsoft.IdentityModel.Tokens;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class GuildAdvisorAI
    {
        // --- HTTP-klient för att prata med OpenAI API ---
        private static readonly HttpClient client = new HttpClient();

        // --- Konstruktor ---
        public GuildAdvisorAI()
        {
            // Hämta API-nyckel från miljövariabler
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            // Felhantering: saknas API-nyckel
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("⚠️ Missing OpenAI API key in environment variables");

            // Sätt Authorization-headern för alla requests
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        // === Huvudmeny för AI-interaktion ===
        public async Task InteractWithUserAsync()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== 🤖 GUILD ADVISOR AI ===\n");
                Console.ResetColor();

                // Visa val för användaren
                Console.WriteLine("1. Generate quest description");
                Console.WriteLine("2. Suggest quest priority");
                Console.WriteLine("3. Summarize all quests");
                Console.WriteLine("4. Return to main menu\n");
                Console.Write("Your choice: ");
                string action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                        await HandleGenerateDescriptionAsync();
                        break;

                    case "2":
                        await HandleSuggestPriorityAsync();
                        break;

                    case "3":
                        await HandleSummarizeAsync();
                        break;

                    case "4":
                        Console.WriteLine("\nReturning to main menu...");
                        await Task.Delay(500);
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nInvalid choice. Please try again.\n");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                // Fångar eventuella fel i hela menyn
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        // === Alternativ 1: Generera quest description ===
        private async Task HandleGenerateDescriptionAsync()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== 📝 GENERATE QUEST DESCRIPTION ===\n");
            Console.ResetColor();

            Console.Write("Enter quest title: ");
            string title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Title cannot be empty.");
                Console.ResetColor();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nGenerating description, please wait...\n");

            string description = await GenerateQuestDescriptionAsync(title);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== Quest Description ===\n");
            Console.ResetColor();

            Console.WriteLine(description);
            Console.WriteLine("\n==========================");

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // === Alternativ 2: Föreslå prioritet ===
        private async Task HandleSuggestPriorityAsync()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== ⚔️ SUGGEST QUEST PRIORITY ===\n");
            Console.ResetColor();

            Console.WriteLine("Paste the quest description below (type 'END' when finished):");

            // Bygger upp flera rader tills användaren skriver END
            StringBuilder questBuilder = new StringBuilder();
            string line;
            while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
            {
                questBuilder.AppendLine(line);
            }

            string questDescription = questBuilder.ToString().Trim();

            if (string.IsNullOrWhiteSpace(questDescription))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nNo description provided. Defaulting to Low priority.");
                Console.ResetColor();
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nAnalyzing quest details...");
            string priority = await SuggestPriorityAsync(questDescription);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSuggested priority: {priority}\n");
            Console.ResetColor();

            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        // === Alternativ 3: Sammanfatta alla quests ===
        private async Task HandleSummarizeAsync()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== 📜 SUMMARIZE ALL QUESTS ===\n");
            Console.ResetColor();

            string summary = await SummarizeQuestsAsync(QuestManager.quests);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(summary);
            Console.ResetColor();

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // === Skicka prompt till AI för att generera quest-beskrivning ===
        public async Task<string> GenerateQuestDescriptionAsync(string title)
        {
            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Generate epic quest descriptions." },
                    new { role = "user", content = $"Generate a detailed, heroic quest description for the title: {title}" }
                }
            };

            string reply = await SendRequestAsync(requestBody);

            return reply.Trim().Replace("**", "").Replace("*", "");
        }

        // === Skicka prompt till AI för att föreslå prioritet ===
        public async Task<string> SuggestPriorityAsync(string questDescription)
        {
            string cleanedDescription = questDescription.Replace("\r", " ").Replace("\n", " ");

            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Suggest a quest priority (High, Medium, Low)." },
                    new { role = "user", content = $"Based on this quest description, suggest a priority: {cleanedDescription}" }
                }
            };

            string reply = await SendRequestAsync(requestBody);

            // Fångar bara relevant ord från AI-svaret
            if (reply.Contains("High", StringComparison.OrdinalIgnoreCase)) return "High";
            if (reply.Contains("Medium", StringComparison.OrdinalIgnoreCase)) return "Medium";
            return "Low";
        }

        // === Skicka prompt till AI för att sammanfatta alla quests ===
        public async Task<string> SummarizeQuestsAsync(List<Quest> quests)
        {
            if (quests.Count == 0)
                return "You have no quests at the moment.";

            // Bygger upp en lista med alla quests för AI:n
            string allQuests = string.Join("\n", quests.Select(q =>
                $"{q.Title} - {(q.IsCompleted ? "✅ Completed" : "❌ Pending")}: {q.Description}"));

            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Summarize quests in a heroic, epic tone." },
                    new { role = "user", content = $"Summarize these quests for the hero:\n{allQuests}" }
                }
            };

            string reply = await SendRequestAsync(requestBody);

            return reply.Trim().Replace("**", "").Replace("*", "");
        }

        // === Gemensam metod för att skicka request till OpenAI ===
        private async Task<string> SendRequestAsync(object requestBody)
        {
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Skicka POST-request till OpenAI API
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseString);

                // Extrahera AI-svaret ur JSON
                string reply = doc.RootElement
                                   .GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString()!;

                return reply;
            }
            catch (Exception ex)
            {
                // Felhantering vid nätverksproblem eller API-fel
                return $"⚠️ Guild Advisor is currently unavailable: {ex.Message}";
            }
        }
    }
}
