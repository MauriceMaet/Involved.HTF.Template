using Involved.HTF.Common;

public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new HackTheFutureClient();

        await client.Login();

        string apiRoute = "/api/a/easy/puzzle";
        var commands = await client.GetData(apiRoute, "commands");

        int result = ProcessCommands(commands);

    }

    private static int ProcessCommands(string commands)
    {
        string[] commandList = commands.Split(',');

        int depthPerMeter = 0;
        int totalDistance = 0;
        int totalDepth = 0;

        foreach (string command in commandList)
        {
            string[] parts = command.Trim().Split(' ');
            string action = parts[0];
            int value = int.Parse(parts[1]);

            switch (action)
            {
                case "Up":
                    depthPerMeter -= value;
                    break;

                case "Down":
                    depthPerMeter += value;
                    break;

                case "Forward":
                    totalDistance += value;
                    totalDepth += depthPerMeter * value;
                    break;
            }
        }

        int result = totalDistance * totalDepth;

        Console.WriteLine($"Total Distance: {totalDistance}");
        Console.WriteLine($"Total Depth: {totalDepth}");
        Console.WriteLine($"Result (Distance * Depth): {result}");

        return result;
    }
}