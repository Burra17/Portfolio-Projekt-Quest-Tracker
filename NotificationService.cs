using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


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

            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken))
            {
                throw new InvalidOperationException("Twilio credentials are missing in environment variables.");
            }

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<string> SendSmsAsync(string toNumber, string message)
        {
            try
            {
                var messageResource = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toNumber),
                    from: new PhoneNumber(_fromNumber ?? "+1XXXXXXXXXX"), // fallback om variabeln saknas
                    body: message
                );

                Console.WriteLine($"SMS skickat till {toNumber}. SID: {messageResource.Sid}");
                return messageResource.Sid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid SMS-utskick: {ex.Message}");
                return null;
            }
        }
    }
}
