using Involved.HTF.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var client = new HackTheFutureClient();

        await client.Login();

        string apiRoute = "/api/a/hard/puzzle";
        string fieldname = "quatralianNumbers";

        string quatralianJson = await client.GetData(apiRoute, fieldname);

        var quatralianNumbers = DeserializeQuatralianNumbers(quatralianJson);

        int totalDecimal = 0;

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
