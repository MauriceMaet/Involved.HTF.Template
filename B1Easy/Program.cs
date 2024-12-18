﻿using Involved.HTF.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        try
        {
            var client = new HackTheFutureClient();

            await client.Login();

            string encodedMessage = await client.GetData("/api/b/easy/puzzle", "alienMessage");

            if (string.IsNullOrEmpty(encodedMessage))
            {
                Console.WriteLine("Error: Received empty or null encoded message from API.");
                return;
            }

            Dictionary<string, string> alphabet = new Dictionary<string, string>
            {
                { "∆", "A" }, { "⍟", "B" }, { "◊", "C" }, { "Ψ", "D" },
                { "Σ", "E" }, { "ϕ", "F" }, { "Ω", "G" }, { "λ", "H" },
                { "ζ", "I" }, { "Ϭ", "J" }, { "ↄ", "K" }, { "◯", "L" },
                { "⧖", "M" }, { "⊗", "N" }, { "⊕", "O" }, { "∇", "P" },
                { "⟁", "Q" }, { "⎍", "R" }, { "φ", "S" }, { "✦", "T" },
                { "⨅", "U" }, { "ᚦ", "V" }, { "ϡ", "W" }, { "⍾", "X" },
                { "⍝", "Y" }, { "≈", "Z" }
            };

            string decodedMessage = DecodeMessage(encodedMessage, alphabet);

            Console.WriteLine("Encoded Message: " + encodedMessage + "\n");
            Console.WriteLine("Decoded Message: " + decodedMessage + "\n");

            try
            {
                string postRoute = "/api/b/easy/puzzle";
                var response = await client.PostData(postRoute, decodedMessage);
                Console.WriteLine("Post response: " + await response.Content.ReadAsStringAsync());
            }
            catch (Exception postEx)
            {
                Console.WriteLine($"Failed to post decoded message: {postEx.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static string DecodeMessage(string encoded, Dictionary<string, string> alphabet)
    {
        if (string.IsNullOrEmpty(encoded))
        {
            return string.Empty;
        }

        string decoded = "";
        foreach (char ch in encoded)
        {
            string symbol = ch.ToString();

            if (alphabet.ContainsKey(symbol))
            {
                decoded += alphabet[symbol];
            }
            else
            {
                decoded += symbol;
            }
        }
        return decoded;
    }
}
