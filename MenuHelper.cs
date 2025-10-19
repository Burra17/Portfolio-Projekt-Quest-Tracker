using Spectre.Console;
using System;
using System.Collections.Generic;

namespace Portfolio_Projekt_Quest_Tracker
{
    public static class MenuHelper
    {
        // === Huvudmeny för icke inloggade användare ===
        public static string ShowMainMenu()
        {
            // Rensa konsolen och visa stor titel
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Quest Tracker")
                .Centered()
                .Color(Color.Green));

            // Visa val och låt användaren välja
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Main Menu[/]")
                    .PageSize(5)
                    .AddChoices(new[]
                    {
                        "Register Hero",  // Registrera ny hjälte
                        "Login Hero",     // Logga in befintlig hjälte
                        "Exit Program"    // Avsluta programmet
                    }));

            return choice;
        }

        // === Hjältemeny för inloggad användare ===
        public static string ShowHeroMenu(string heroName)
        {
            // Rensa konsolen och visa välkomsttext med hjältenamn
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText($"Welcome {heroName}")
                .Centered()
                .Color(Color.Cyan1));

            // Visa val och låt användaren välja
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Hero Menu[/]")
                    .PageSize(8) // Ökat för att passa fler val
                    .AddChoices(new[]
                    {
                        "Add New Quest",          // Skapa nytt quest
                        "View All Quests",        // Visa alla quests
                        "Update/Complete Quest",  // Uppdatera eller markera quest som slutfört
                        "Guild Advisor (AI)",     // AI-assistent för quest
                        "Check Quest Deadlines",  // Nytt val för att kolla quests nära deadline
                        "Logout"                  // Logga ut
                    }));

            return choice;
        }
    }
}
