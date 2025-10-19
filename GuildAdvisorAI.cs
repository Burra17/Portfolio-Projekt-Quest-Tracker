using Microsoft.IdentityModel.Tokens;
using OpenAI;
using OpenAI.Chat;
using Spectre.Console;
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
        // --- HTTP-klient ---
        // Vi använder en statisk HttpClient för alla OpenAI-anrop. 
        // Detta gör att vi inte skapar nya klienter varje gång, vilket är mer effektivt.
        private static readonly HttpClient client = new HttpClient();

        // --- Konstruktor ---
        public GuildAdvisorAI()
        {
            // Hämta OpenAI API-nyckel från miljövariabler
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            // Kontrollera att API-nyckeln finns
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Missing OpenAI API key in environment variables");

            // Lägg till Authorization-headern i alla requests
            // Detta behövs för att OpenAI ska acceptera våra anrop
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        // === Huvudmeny med Spectre.Console ===
        // Användaren kan navigera med piltangenter istället för att skriva siffror
        public async Task InteractWithUserAsync()
        {
            try
            {
                // Rensa konsolen för en ren start
                Console.Clear();

                // Skriv ut en titel med färg
                AnsiConsole.MarkupLine("[bold cyan]\n=== GUILD ADVISOR AI ===[/]\n");

                // Skapa en meny med SelectionPrompt från Spectre.Console
                // Användaren kan navigera med piltangenter och välja ett alternativ
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose AI action:")
                        .PageSize(5) // Max antal synliga alternativ
                        .AddChoices(new[]
                        {
                            "Generate quest description",
                            "Suggest quest priority",
                            "Summarize all quests",
                            "Return to main menu"
                        }));

                // Hantera användarens val
                switch (action)
                {
                    case "Generate quest description":
                        // Anropa metod som genererar quest-beskrivning
                        await HandleGenerateDescriptionAsync();
                        break;

                    case "Suggest quest priority":
                        // Anropa metod som föreslår prioritet baserat på beskrivning
                        await HandleSuggestPriorityAsync();
                        break;

                    case "Summarize all quests":
                        // Anropa metod som sammanfattar alla quests
                        await HandleSummarizeAsync();
                        break;

                    case "Return to main menu":
                        // Tillbaka till huvudmenyn
                        AnsiConsole.MarkupLine("\nReturning to main menu...");
                        await Task.Delay(500); // liten paus för bättre UX
                        break;
                }
            }
            catch (Exception ex)
            {
                // Fångar eventuella oväntade fel under menyn
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        // === Alternativ 1: Generera quest description ===
        private async Task HandleGenerateDescriptionAsync()
        {
            // Rensa konsolen
            Console.Clear();

            // Skriv ut rubrik i gult
            AnsiConsole.MarkupLine("[yellow]=== GENERATE QUEST DESCRIPTION ===[/]");

            // Fråga användaren om quest-titel
            string title = AnsiConsole.Ask<string>("Enter quest title:");

            // Om användaren inte skriver något, avbryt
            if (string.IsNullOrWhiteSpace(title))
            {
                AnsiConsole.MarkupLine("[red]Title cannot be empty.[/]");
                Console.ReadKey();
                return;
            }

            // Informera användaren att AI:n analyserar texten
            AnsiConsole.MarkupLine("\nGenerating description, please wait...\n");

            // Anropa AI för att generera beskrivning
            string description = await GenerateQuestDescriptionAsync(title);

            // Skriv ut resultatet i grönt
            AnsiConsole.MarkupLine("[green]=== Quest Description ===[/]");
            Console.WriteLine(description);

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // === Alternativ 2: Föreslå prioritet ===
        private async Task HandleSuggestPriorityAsync()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow]=== SUGGEST QUEST PRIORITY ===[/]");

            // Instruktion till användaren för att klistra in text
            AnsiConsole.MarkupLine("Paste the quest description (type 'END' when finished):");

            // StringBuilder används för att samla flera rader från användaren
            StringBuilder questBuilder = new StringBuilder();
            string line;
            while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
                questBuilder.AppendLine(line);

            // Konvertera StringBuilder till string och trimma whitespace
            string questDescription = questBuilder.ToString().Trim();

            // Om användaren inte skriver något, sätt default-prioritet
            if (string.IsNullOrWhiteSpace(questDescription))
            {
                AnsiConsole.MarkupLine("[red]No description provided. Defaulting to Low priority.[/]");
                Console.ReadKey();
                return;
            }

            // Informera användaren att AI:n analyserar questen
            AnsiConsole.MarkupLine("\nAnalyzing quest details...");

            // Anropa AI för att få prioritet
            string priority = await SuggestPriorityAsync(questDescription);

            // Skriv ut resultatet i grönt
            AnsiConsole.MarkupLine($"[green]Suggested priority: {priority}[/]");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        // === Alternativ 3: Sammanfatta alla quests ===
        private async Task HandleSummarizeAsync()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow]=== SUMMARIZE ALL QUESTS ===[/]");

            // Anropa AI för att skapa sammanfattning
            string summary = await SummarizeQuestsAsync(QuestManager.quests);

            // Skriv ut resultatet i grönt
            AnsiConsole.MarkupLine("[green]All Quests Summary:[/]");
            Console.WriteLine(summary);

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // === AI: Generera quest-beskrivning ---
        public async Task<string> GenerateQuestDescriptionAsync(string title)
        {
            // Skapa JSON-payload som skickas till OpenAI
            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Generate epic quest descriptions." },
                    new { role = "user", content = $"Generate a detailed, heroic quest description for the title: {title}" }
                }
            };

            // Skicka request och få svar
            string reply = await SendRequestAsync(requestBody);

            // Rensa bort markdown-symboler
            return reply.Trim().Replace("**", "").Replace("*", "");
        }

        // === AI: Föreslå prioritet ---
        public async Task<string> SuggestPriorityAsync(string questDescription)
        {
            // Ta bort radbrytningar så texten blir en lång rad
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

            // Extrahera bara High/Medium/Low
            if (reply.Contains("High", StringComparison.OrdinalIgnoreCase)) return "High";
            if (reply.Contains("Medium", StringComparison.OrdinalIgnoreCase)) return "Medium";
            return "Low";
        }

        // === AI: Sammanfatta alla quests ---
        public async Task<string> SummarizeQuestsAsync(List<Quest> quests)
        {
            if (quests.Count == 0) return "You have no quests at the moment.";

            // Skapa en sträng med alla quests och status
            string allQuests = string.Join("\n", quests.Select(q =>
                $"{q.Title} - {(q.IsCompleted ? "Completed" : "Pending")}: {q.Description}"));

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

        // === Skicka request till OpenAI API ---
        private async Task<string> SendRequestAsync(object requestBody)
        {
            // Serialisera objektet till JSON
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Skicka POST-request till OpenAI
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                // Parsar JSON-responsen
                using JsonDocument doc = JsonDocument.Parse(responseString);

                // Extrahera innehållet från AI-svaret
                string reply = doc.RootElement.GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString()!;

                return reply;
            }
            catch (Exception ex)
            {
                // Om något går fel, returnera meddelande
                return $"Guild Advisor is currently unavailable: {ex.Message}";
            }
        }
    }
}
