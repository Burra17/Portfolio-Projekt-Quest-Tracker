using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class NotificationService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public NotificationService()
        {
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            _fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER");

            if (!string.IsNullOrEmpty(_accountSid) && !string.IsNullOrEmpty(_authToken))
            {
                Twilio.TwilioClient.Init(_accountSid, _authToken);
            }
        }

        // --- Skicka SMS generellt ---
        public async Task<string> SendSmsAsync(string toNumber, string message)
        {
            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken))
                return "Twilio not configured.";

            try
            {
                var msg = await Twilio.Rest.Api.V2010.Account.MessageResource.CreateAsync(
                    to: new Twilio.Types.PhoneNumber(toNumber),
                    from: new Twilio.Types.PhoneNumber(_fromNumber),
                    body: message
                );

                return $"SMS sent";
            }
            catch (Exception ex)
            {
                return $"Error sending SMS: {ex.Message}";
            }
        }

        // --- Skicka deadline-notis för ett quest ---
        public async Task SendDeadlineNotificationAsync(Quest quest, string userPhone)
        {
            string message = $"⚔️ Hjälte, ditt uppdrag '{quest.Title}' måste slutföras inom 24 timmar!";

            // Här skickas SMS
            await SendSmsAsync(userPhone, message);

            // Visa panel i Spectre.Console utan SMS-ID
            var panel = new Panel($"Notification sent for '{quest.Title}'!")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("green"))
                .Header("[bold green]ALERT[/]")
                .HeaderAlignment(Justify.Center);
            AnsiConsole.Write(panel);
        }
    }
}
