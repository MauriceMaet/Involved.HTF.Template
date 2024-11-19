using System;
using System.Threading.Tasks;
using Involved.HTF.Common;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        var client = new HackTheFutureClient();

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

        Console.WriteLine($"Send DateTime: {sendDateTime}");
        Console.WriteLine($"Travel Speed: {travelSpeed} light-years/minute");
        Console.WriteLine($"Distance: {distance} light-years");
        Console.WriteLine($"Day Length: {dayLengthInHours} hours");

        DateTime arrivalTime = CalculateArrivalTime(sendDateTime, travelSpeed, distance, dayLengthInHours);

        Console.WriteLine($"Arrival time: {arrivalTime.ToString("yyyy-MM-ddTHH:mm:ssZ")}");
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
