using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class NotificationService
    {
        private readonly string _accountSid;     // Twilio Account SID
        private readonly string _authToken;      // Twilio Auth Token
        private readonly string _fromNumber;     // Telefonnummer som SMS skickas från

        // Konstruktor
        public NotificationService()
        {
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            _fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER");

            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken))
                throw new InvalidOperationException("Twilio credentials are missing in environment variables.");

            // Initiera Twilio-klienten
            TwilioClient.Init(_accountSid, _authToken);
        }

        // Skickar ett SMS
        public async Task<string> SendSmsAsync(string toNumber, string message)
        {
            try
            {
                var messageResource = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toNumber),
                    from: new PhoneNumber(_fromNumber ?? "+1XXXX"),
                    body: message
                );

                Console.WriteLine($"SMS skickat till {toNumber}: {message}");
                return messageResource.Sid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid SMS-utskick: {ex.Message}");
                return null;
            }
        }

        // Skickar påminnelser för quests som närmar sig deadline (< 24 timmar)
        public async Task SendDeadlineAlertsAsync(User user)
        {
            if (user == null)
            {
                Console.WriteLine("Ingen hjälte inloggad. Kan inte skicka notiser.");
                return;
            }

            bool anyAlert = false;

            foreach (var quest in QuestManager.quests)
            {
                if (quest.IsCompleted) continue; // Hoppa över slutförda quests

                double hoursLeft = (quest.DueDate - DateTime.Now).TotalHours;

                // Visa deadline
                Console.WriteLine($"Quest: {quest.Title}");
                Console.WriteLine($"Due: {quest.DueDate:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Time left: {Math.Round(hoursLeft, 1)} timmar");

                if (hoursLeft <= 24 && hoursLeft > 0)
                {
                    string message = $"Hjälte {user.Username}, ditt uppdrag \"{quest.Title}\" måste vara klart imorgon!";
                    await SendSmsAsync(user.PhoneNumber, message);
                    Console.WriteLine("Notis skickad!");
                    anyAlert = true;
                }

                Console.WriteLine(); // tom rad mellan quests
            }

            if (!anyAlert)
                Console.WriteLine("Inga quests nära deadline just nu.");

            Console.WriteLine("\nTryck på Enter för att återgå till menyn...");
            Console.ReadLine(); // Vänta på användaren innan menyn visas
        }
    }
}
