using System;
using System.Threading.Tasks;
using Involved.HTF.Common;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        var client = new HackTheFutureClient();

        try
        {
            await client.Login();

            string apiRoute = "/api/b/medium/puzzle";

            string jsonSendDateTime = await client.GetData(apiRoute, "sendDateTime");
            string jsonTravelSpeed = await client.GetData(apiRoute, "travelSpeed");
            string jsonDistance = await client.GetData(apiRoute, "distance");
            string jsonDayLength = await client.GetData(apiRoute, "dayLength");

            DateTime sendDateTime = ParseDateTime(jsonSendDateTime);
            double travelSpeed = JsonConvert.DeserializeObject<double>(jsonTravelSpeed);
            double distance = JsonConvert.DeserializeObject<double>(jsonDistance);
            int dayLengthInHours = JsonConvert.DeserializeObject<int>(jsonDayLength);

            Console.WriteLine($"Send DateTime: {sendDateTime} \n");
            Console.WriteLine($"Travel Speed: {travelSpeed} light-years/minute \n");
            Console.WriteLine($"Distance: {distance} light-years \n");
            Console.WriteLine($"Day Length: {dayLengthInHours} hours \n");

            DateTime arrivalTime = CalculateArrivalTime(sendDateTime, travelSpeed, distance, dayLengthInHours);

            Console.WriteLine($"Arrival time: {arrivalTime.ToString("yyyy-MM-ddTHH:mm:ssZ")}\n");

            try
            {
                string postRoute = "/api/b/medium/puzzle";
                string payload = arrivalTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var response = await client.PostData(postRoute, payload);
                Console.WriteLine("Post response: " + await response.Content.ReadAsStringAsync());
            }
            catch (Exception postEx)
            {
                Console.WriteLine($"Failed to post arrival time: {postEx.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static DateTime ParseDateTime(string jsonSendDateTime)
    {
        if (DateTime.TryParse(jsonSendDateTime, out DateTime parsedDate))
        {
            return parsedDate;
        }
        else
        {
            if (long.TryParse(jsonSendDateTime, out long unixTimestamp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
            }
            else
            {
                throw new Exception($"Invalid sendDateTime format: {jsonSendDateTime}");
            }
        }
    }

    static DateTime CalculateArrivalTime(DateTime sendDateTime, double travelSpeed, double distance, int dayLengthInHours)
    {
        double travelTimeInMinutes = (distance / travelSpeed) * 60;
        double minutesInADay = dayLengthInHours * 60;

        DateTime arrivalTime = sendDateTime.AddMinutes(travelTimeInMinutes);

        if (travelTimeInMinutes > minutesInADay)
        {
            double timeInCurrentDay = (arrivalTime - sendDateTime).TotalMinutes % minutesInADay;
            arrivalTime = sendDateTime.AddMinutes(timeInCurrentDay);
        }

        return arrivalTime;
    }
}
