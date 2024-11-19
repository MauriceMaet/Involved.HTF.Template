using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Involved.HTF.Common;

class Alien
{
    public int Strength { get; set; }
    public int Speed { get; set; }
    public int Health { get; set; }
}

class Program
{
    public static async Task Main(string[] args)
    {
        // Create an instance of HackTheFutureClient
        var client = new HackTheFutureClient();

        // Login to get the token
        await client.Login();

        // Define the API route for team A and B
        string apiRoute = "/api/a/medium/puzzle";

        // Get the data (team A and team B) from the specified API endpoints
        string teamAJson = await client.GetData(apiRoute, "teamA");
        string teamBJson = await client.GetData(apiRoute, "teamB");

        // Deserialize the JSON data into Alien lists for team A and team B
        List<Alien> teamA = JsonConvert.DeserializeObject<List<Alien>>(teamAJson);
        List<Alien> teamB = JsonConvert.DeserializeObject<List<Alien>>(teamBJson);

        // Simulate the battle
        SimulateBattle(teamA, teamB);
    }

    static void SimulateBattle(List<Alien> teamA, List<Alien> teamB)
    {
        int aIndex = 0, bIndex = 0;

        while (aIndex < teamA.Count && bIndex < teamB.Count)
        {
            Alien alienA = teamA[aIndex];
            Alien alienB = teamB[bIndex];

            Console.WriteLine($"Alien A (Health: {alienA.Health}, Speed: {alienA.Speed}) vs Alien B (Health: {alienB.Health}, Speed: {alienB.Speed})");

            // Determine who attacks first
            if (alienA.Speed > alienB.Speed || (alienA.Speed == alienB.Speed))
            {
                Console.WriteLine("Team A attacks first!");
                Attack(alienA, alienB);
                if (alienB.Health > 0)
                {
                    Attack(alienB, alienA);
                }
            }
            else
            {
                Console.WriteLine("Team B attacks first!");
                Attack(alienB, alienA);
                if (alienA.Health > 0)
                {
                    Attack(alienA, alienB);
                }
            }

            // Check if aliens are defeated
            if (alienA.Health <= 0) aIndex++;
            if (alienB.Health <= 0) bIndex++;
        }

        // Determine the winning team
        if (aIndex < teamA.Count)
        {
            Console.WriteLine($"Team A wins with total health: {CalculateTotalHealth(teamA, aIndex)}");
        }
        else
        {
            Console.WriteLine($"Team B wins with total health: {CalculateTotalHealth(teamB, bIndex)}");
        }
    }

    static void Attack(Alien attacker, Alien defender)
    {
        defender.Health -= attacker.Strength;
        Console.WriteLine($"{(attacker.Strength > 0 ? "Hit!" : "Miss!")}: Defender's health is now {Math.Max(defender.Health, 0)}");
    }

    static int CalculateTotalHealth(List<Alien> team, int startIndex)
    {
        int totalHealth = 0;
        for (int i = startIndex; i < team.Count; i++)
        {
            totalHealth += Math.Max(0, team[i].Health);
        }
        return totalHealth;
    }
}
