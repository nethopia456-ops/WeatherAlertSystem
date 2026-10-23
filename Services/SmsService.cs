
using System;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAlertSystem
{
    public static class SmsService
    {
        public static Task<string> SendAsync(
            string recipient,
            string message)
        {
            if (string.IsNullOrWhiteSpace(recipient))
            {
                throw new ArgumentException(
                    "Recipient number is required.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Message cannot be empty.");
            }

            // SIMULATION ONLY: No SMS is sent.
            string result =
                "SIMULATED SMS - NOT SENT\n\n" +
                "Recipient: " + recipient + "\n\n" +
                "Message:\n" + message + "\n\n" +
                "Status: Demo completed. No real SMS was sent.";

            return Task.FromResult(result);
        }
    }
}

