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
        private static readonly HttpClient client = new HttpClient(); // HTTP-klient för att prata med OpenAI API

        // --- Konstruktor ---
        public GuildAdvisorAI()
        {
            // Hämta API-nyckel från miljövariabler
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Missing OpenAI API key in environment variables");

            // Sätt Authorization-headern för alla requests
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        // --- Huvudmetod som hanterar all interaktion med användaren ---
        public async Task InteractWithUserAsync()
        {
            Console.WriteLine("\nChoose AI action:");
            Console.WriteLine("1. Generate quest description");
            Console.WriteLine("2. Suggest priority");
            Console.WriteLine("3. Summarize all quests");
            Console.Write("Your choice: ");
            string action = Console.ReadLine();

            if (action == "1")
            {
                // --- Generera quest description ---
                Console.Write("Enter quest title: ");
                string title = Console.ReadLine();
                string description = await GenerateQuestDescriptionAsync(title);

                Console.WriteLine("\n=== Quest Description ===");
                Console.WriteLine(description);
                Console.WriteLine("========================\n");
            }
            else if (action == "2")
            {
                // --- Föreslå prioritet ---
                string priority = await SuggestPriorityAsync();
                Console.WriteLine($"\nSuggested priority: {priority}\n");
            }
            else if (action == "3")
            {
                // --- Sammanfatta alla quests ---
                string summary = await SummarizeQuestsAsync(QuestManager.quests);

                Console.WriteLine("\n=== Quests Summary ===");
                Console.WriteLine(summary);
                Console.WriteLine("=====================\n");
            }
            else
            {
                Console.WriteLine("Invalid choice.\n");
            }

            // Vänta på Enter innan menyn visas igen
            Console.WriteLine("Press Enter to return to your menu...");
            Console.ReadLine();
        }

        // --- Generera quest description ---
        public async Task<string> GenerateQuestDescriptionAsync(string title)
        {
            // Skapa prompt för AI
            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Generate epic quest descriptions." },
                    new { role = "user", content = $"Generate a detailed, heroic quest description for the title: {title}" }
                }
            };

            // Skicka request och returnera AI-svaret
            string reply = await SendRequestAsync(requestBody);

            // Trimma whitespace och ta bort Markdown
            return reply.Trim().Replace("**", "").Replace("*", "");
        }

        // --- Föreslå prioritet baserat på quest description ---
        public async Task<string> SuggestPriorityAsync()
        {
            // Be användaren klistra in quest-beskrivningen, flera rader
            Console.WriteLine("Paste the quest description (press enter and type 'END' when done):");

            StringBuilder questBuilder = new StringBuilder();
            string line;
            while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
            {
                questBuilder.AppendLine(line);
            }

            string questDescription = questBuilder.ToString().Trim();

            if (string.IsNullOrWhiteSpace(questDescription))
            {
                Console.WriteLine("No description provided. Defaulting to Low priority.");
                return "Low";
            }

            // Rensa radbrytningar för AI
            string cleanedDescription = questDescription.Replace("\r", " ").Replace("\n", " ");

            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Suggest a priority for a quest (High, Medium, Low)." },
                    new { role = "user", content = $"Based on this quest description, suggest a priority: {cleanedDescription}" }
                }
            };

            string reply = await SendRequestAsync(requestBody);

            // Returnera endast High/Medium/Low baserat på AI-svaret
            if (reply.Contains("High", StringComparison.OrdinalIgnoreCase)) return "High";
            if (reply.Contains("Medium", StringComparison.OrdinalIgnoreCase)) return "Medium";
            return "Low";
        }

        // --- Sammanfatta alla quests ---
        public async Task<string> SummarizeQuestsAsync(List<Quest> quests)
        {
            if (quests.Count == 0) return "You have no quests at the moment.";

            // Bygg en text med alla quests och deras status
            string allQuests = string.Join("\n", quests.Select(q =>
                $"{q.Title} - {(q.IsCompleted ? "✅ Completed" : "❌ Pending")}: {q.Description}"));

            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Summarize quests in a heroic, epic briefing." },
                    new { role = "user", content = $"Summarize these quests for the hero:\n{allQuests}" }
                }
            };

            string reply = await SendRequestAsync(requestBody);

            return reply.Trim().Replace("**", "").Replace("*", "");
        }

        // --- Gemensam metod för att skicka request till OpenAI ---
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
                string reply = doc.RootElement
                                   .GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString()!;

                return reply;
            }
            catch (Exception ex)
            {
                return $"Guild Advisor is unavailable: {ex.Message}";
            }
        }
    }
}
