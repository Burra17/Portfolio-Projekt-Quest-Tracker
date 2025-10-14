using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace Portfolio_Projekt_Quest_Tracker
{
    public class NotificationService
    {
        //Twilio
        private readonly string _accountSid; //Kontots SID
        private readonly string _authToken; //Kontots Auth Token
        private readonly string _fromNumber; //Nummer som sms skickas från

        //Konstruktor
        public NotificationService()
        {
            // Hämtar Twilio miljövariabler
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            _fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER");

            //Kollar så de finns
            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken))
            {
                throw new InvalidOperationException("Twilio credentials are missing in environment variables.");
            }

            //Initiera Twilio med SID och auth token, (faketelefon som nemo kalla det)
            TwilioClient.Init(_accountSid, _authToken);
        }

        //metod för att skicka sms
        public async Task<string> SendSmsAsync(string toNumber, string message)
        {
            try
            {
                //Skapa och skicka meddelandet via twilio
                var messageResource = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toNumber), // Mottagarens telefonnummer
                    from: new PhoneNumber(_fromNumber ?? "+1XXXXXXXXXX"), // avsändarens nummer, fallback om variabeln saknas
                    body: message //sms innehållet
                );

                //Bekräftelse i konsolen
                Console.WriteLine($"SMS skickat till {toNumber}.");
                return messageResource.Sid; //Returnera för meddelandet
            }
            catch (Exception ex)
            {
                //Fånga fel vid sms-utskick
                Console.WriteLine($"Fel vid SMS-utskick: {ex.Message}");
                return null;
            }
        }
    }
}
