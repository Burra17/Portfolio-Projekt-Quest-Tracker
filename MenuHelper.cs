using Spectre.Console;
using System;
using System.Collections.Generic;

namespace Portfolio_Projekt_Quest_Tracker
{
    public static class MenuHelper
    {
        // === Huvudmeny ===
        public static string ShowMainMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Quest Tracker")
                .Centered()
                .Color(Color.Green));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Main Menu[/]")
                    .PageSize(5)
                    .AddChoices(new[]
                    {
                        "Register Hero",
                        "Login Hero",
                        "Exit Program"
                    }));

            return choice;
        }

        // === Hjältemeny ===
        public static string ShowHeroMenu(string heroName)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText($"Welcome {heroName}")
                .Centered()
                .Color(Color.Cyan1));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Hero Menu[/]")
                    .PageSize(7)
                    .AddChoices(new[]
                    {
                        "Add New Quest",
                        "View All Quests",
                        "Update/Complete Quest",
                        "Guild Advisor (AI)",
                        "Logout"
                    }));

            return choice;
        }
    }
}
