
namespace Portfolio_Projekt_Quest_Tracker
{
    public static class MenuHelper //Statisk klass för att skriva ut promgrammets meny
    {
        // Huvud meny som skrivs ut när programmet startar
        public static void MainMenu()
        {
            Console.WriteLine("1. Register hero.");
            Console.WriteLine("2. Login hero.");
            Console.WriteLine("3. Exit program");
        }


        // Meny för inloggad hjälte
        public static void HeroMenu(string heroName)
        {
            Console.WriteLine($"\n=== Welcome, {heroName}! ===");
            Console.WriteLine("1. Add new quest");
            Console.WriteLine("2. View all quests");
            Console.WriteLine("3. Update/Complete quest");
            Console.WriteLine("4. Request Guild Advisor help (AI)");
            Console.WriteLine("5. Show guild report");
            Console.WriteLine("6. Logout");
            Console.WriteLine("============================");
            Console.Write("Choose an option: ");
        }
    }
}
