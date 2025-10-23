using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portfolio_Projekt_Quest_Tracker
{
    /// <summary>
    /// GuildAdvisorAI – AI-rådgivare som pratar med OpenAI via HTTP.
    /// Meny och output via Spectre.Console. Anpassad för per-user quests.
    /// </summary>
    public class GuildAdvisorAI
    {
        // --- HTTP-klient återanvänds för alla anrop (prestanda & sockets) ---
        private static readonly HttpClient client = new HttpClient();

        // --- Välj OpenAI-modell här (kan bytas vid behov) ---
        private const string DefaultModel = "gpt-4.1-mini";

        /// <summary>
        /// Init: lägg till Authorization-header från miljövariabeln OPENAI_API_KEY.
        /// </summary>
        public GuildAdvisorAI()
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Missing OPENAI_API_KEY environment variable.");

            // Sätt bara headern en gång
            if (client.DefaultRequestHeaders.Authorization == null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
        }

        /// <summary>
        /// Huvudmeny för AI:n. Måste få den inloggade hjälten för att läsa hens quests.
        /// </summary>
        public async Task InteractWithUserAsync(User loggedInUser)
        {
            bool active = true;

            while (active)
            {
                Console.Clear();
                AnsiConsole.Write(new FigletText("Guild Advisor").Centered().Color(Color.MediumPurple));
                AnsiConsole.MarkupLine($"[grey]Advisor linked to hero: [bold]{loggedInUser.Username}[/][/]\n");

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow]Choose AI action[/]")
                        .PageSize(6)
                        .AddChoices(
                            "Generate quest description",
                            "Suggest quest priority",
                            "Summarize all quests",
                            "Return to main menu"
                        ));

                switch (action)
                {
                    case "Generate quest description":
                        await HandleGenerateDescriptionAsync();
                        break;

                    case "Suggest quest priority":
                        await HandleSuggestPriorityAsync();
                        break;

                    case "Summarize all quests":
                        await HandleSummarizeAsync(loggedInUser);
                        break;

                    case "Return to main menu":
                        // liten paus för att ge feedback
                        AnsiConsole.MarkupLine("\nReturning to main menu...");
                        await Task.Delay(350);
                        active = false;
                        break;
                }
            }
        }

        // ============================================================
        // ===============   MENYALTERNATIV / HANDLERS   ==============
        // ============================================================

        /// <summary>
        /// Alternativ 1: Generera en episk men kort quest-beskrivning.
        /// </summary>
        private async Task HandleGenerateDescriptionAsync()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow]=== GENERATE QUEST DESCRIPTION ===[/]");

            string title = AnsiConsole.Ask<string>("Enter quest title:");
            if (string.IsNullOrWhiteSpace(title))
            {
                AnsiConsole.MarkupLine("[red]Title cannot be empty.[/]");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            AnsiConsole.MarkupLine("\n[grey]Generating description...[/]\n");
            string description = await GenerateQuestDescriptionAsync(title);

            AnsiConsole.MarkupLine("[green]=== Quest Description ===[/]");
            Console.WriteLine(description);

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        /// <summary>
        /// Alternativ 2: Låt AI föreslå prioritet (High/Medium/Low) utifrån beskrivning.
        /// </summary>
        private async Task HandleSuggestPriorityAsync()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow]=== SUGGEST QUEST PRIORITY ===[/]");
            AnsiConsole.MarkupLine("Paste the quest description (type 'END' when finished):");

            var sb = new StringBuilder();
            while (true)
            {
                var line = Console.ReadLine();
                if (line != null && line.Trim().Equals("END", StringComparison.OrdinalIgnoreCase)) break;
                sb.AppendLine(line);
            }

            string questDescription = sb.ToString().Trim();
            if (string.IsNullOrWhiteSpace(questDescription))
            {
                AnsiConsole.MarkupLine("[red]No description provided. Defaulting to Low priority.[/]");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            AnsiConsole.MarkupLine("\n[grey]Analyzing quest...[/]\n");
            string priority = await SuggestPriorityAsync(questDescription);

            AnsiConsole.MarkupLine($"[green]Suggested priority: [bold]{priority}[/][/]");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        /// <summary>
        /// Alternativ 3: Sammanfatta ALLA quests för inloggad hjälte (inte global lista).
        /// </summary>
        private async Task HandleSummarizeAsync(User loggedInUser)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow]=== SUMMARIZE ALL QUESTS ===[/]\n");

            var quests = loggedInUser.Quests;
            if (quests.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]You have no quests at the moment.[/]");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            string summary = await SummarizeQuestsAsync(quests);

            AnsiConsole.MarkupLine("[green]=== All Quests Summary ===[/]");
            Console.WriteLine(summary);

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // ============================================================
        // ======================   OPENAI-LOGIK   =====================
        // ============================================================

        /// <summary>
        /// Skapar en kort och “terminalvänlig” quest-beskrivning för en given titel.
        /// </summary>
        public async Task<string> GenerateQuestDescriptionAsync(string title)
        {
            var requestBody = new
            {
                model = DefaultModel,
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Generate epic yet concise quest descriptions suitable for a terminal UI. Avoid markdown formatting." },
                    new { role = "user", content = $"Write a short, vivid quest description for the title: \"{title}\". Keep it 5-7 lines max." }
                }
            };

            string reply = await SendRequestAsync(requestBody);
            return Sanitize(reply);
        }

        /// <summary>
        /// Låt AI föreslå High/Medium/Low utifrån en given text.
        /// </summary>
        public async Task<string> SuggestPriorityAsync(string questDescription)
        {
            string cleaned = questDescription.Replace("\r", " ").Replace("\n", " ");

            var requestBody = new
            {
                model = DefaultModel,
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Reply ONLY with one word: High, Medium, or Low." },
                    new { role = "user", content = $"Given this quest description, what priority should it have (High/Medium/Low)? {cleaned}" }
                }
            };

            string reply = Sanitize(await SendRequestAsync(requestBody));

            // Hårdsäkerställ bara H/M/L
            if (reply.IndexOf("High", StringComparison.OrdinalIgnoreCase) >= 0) return "High";
            if (reply.IndexOf("Medium", StringComparison.OrdinalIgnoreCase) >= 0) return "Medium";
            return "Low";
        }

        /// <summary>
        /// Sammanfatta en lista med quests i hjälte-ton.
        /// </summary>
        public async Task<string> SummarizeQuestsAsync(List<Quest> quests)
        {
            // Bygg upp en enkel, tydlig lista som prompt
            string allQuests = string.Join("\n", quests.Select(q =>
                $"- {q.Title} | {(q.IsCompleted ? "Completed" : "Pending")} | Due {q.DueDate:yyyy-MM-dd} | Priority {q.Priority}: {Truncate(q.Description, 160)}"));

            var requestBody = new
            {
                model = DefaultModel,
                messages = new[]
                {
                    new { role = "system", content = "You are a Guild Advisor. Summarize the hero's quests with a concise heroic tone suitable for a terminal. No markdown." },
                    new { role = "user", content = $"Summarize these quests and recommend next 1-3 actions:\n{allQuests}" }
                }
            };

            string reply = await SendRequestAsync(requestBody);
            return Sanitize(reply);
        }

        /// <summary>
        /// Skickar POST till OpenAI Chat Completions och extraherar svaret.
        /// </summary>
        private async Task<string> SendRequestAsync(object requestBody)
        {
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string resp = await response.Content.ReadAsStringAsync();

                // Enkel felhantering vid 4xx/5xx för att ytliggöra problem i konsolen
                if (!response.IsSuccessStatusCode)
                    return $"[OpenAI error {((int)response.StatusCode)}] {resp}";

                using var doc = JsonDocument.Parse(resp);
                string reply = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";

                return reply;
            }
            catch (Exception ex)
            {
                return $"Guild Advisor is currently unavailable: {ex.Message}";
            }
        }

        // ============================================================
        // ======================   HJÄLP-METODER   ====================
        // ============================================================

        private static string Sanitize(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            // Ta bort markdown-stjärnor så texten blir ren i terminalen
            return s.Replace("**", "").Replace("*", "").Trim();
        }

        private static string Truncate(string s, int max) // Trunkera text för sammanfattning
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max - 1) + "…";
        }
    }
}
