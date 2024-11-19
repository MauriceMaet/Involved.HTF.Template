using Involved.HTF.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Create an instance of HackTheFutureClient
        var client = new HackTheFutureClient();

        // Log in to authenticate and get the token
        await client.Login();

        // Define the API route and field name for the data (you can modify these as necessary)
        string apiRoute = "/api/a/hard/puzzle";
        string fieldname = "quatralianNumbers"; // assuming the field that holds the list of quatralian numbers in the response

        // Retrieve the quatralian numbers using GetData
        string quatralianJson = await client.GetData(apiRoute, fieldname);

        // Deserialize the JSON array into a list of quatralian numbers
        var quatralianNumbers = DeserializeQuatralianNumbers(quatralianJson);

        int totalDecimal = 0;

        // Process each quatralian number
        foreach (var quatralian in quatralianNumbers)
        {
            totalDecimal += ConvertToDecimal(quatralian);
        }

        Console.WriteLine($"De som van de decimale waarden is: {totalDecimal}");

        string quatralianSum = ConvertToQuatralian(totalDecimal);
        Console.WriteLine($"De som in Quatralianse notatie is: {quatralianSum}");
    }

    static List<string> DeserializeQuatralianNumbers(string json)
    {
        // Deserialize the JSON string to a List of strings (Quatralian numbers)
        // Ensure you have the necessary deserialization logic depending on your actual JSON format
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
    }

    static int ConvertToDecimal(string quatralianNumber)
    {
        int decimalValue = 0;
        string[] parts = quatralianNumber.Split(' ');

        foreach (var part in parts)
        {
            int value = 0;
            int length = part.Length;

            foreach (char ch in part)
            {
                if (ch == '·')
                {
                    value++;
                }
                else if (ch == '|')
                {
                    value += 5;
                }
            }

            decimalValue = decimalValue * 20 + value;
        }

        return decimalValue;
    }

    static string ConvertToQuatralian(int decimalNumber)
    {
        if (decimalNumber == 0)
            return "Ⱄ";

        string quatralianNumber = "";
        while (decimalNumber > 0)
        {
            int remainder = decimalNumber % 20;
            quatralianNumber = ConvertSingleDigitToQuatralian(remainder) + " " + quatralianNumber;
            decimalNumber /= 20;
        }

        return quatralianNumber.Trim();
    }

    static string ConvertSingleDigitToQuatralian(int digit)
    {
        int ones = digit % 5;
        int fives = digit / 5;

        string quatralian = new string('·', ones);
        if (fives > 0)
        {
            quatralian = new string('|', fives) + quatralian;
        }

        return quatralian;
    }
}
