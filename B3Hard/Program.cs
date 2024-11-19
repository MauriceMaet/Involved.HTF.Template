using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Involved.HTF.Common;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        var client = new HackTheFutureClient();

        await client.Login();

        string apiRoute = "/api/b/hard/puzzle";
        string fieldname = "maze"; 

        string mazeJson = await client.GetData(apiRoute, fieldname);

        var mazeData = JsonConvert.DeserializeObject<List<List<string>>>(mazeJson);

        char[][] maze = ConvertToCharArray(mazeData);

        int result = SolveMaze(maze);

        Console.WriteLine(result);
    }

    static char[][] ConvertToCharArray(List<List<string>> mazeData)
    {
        int rows = mazeData.Count;
        int cols = mazeData[0].Count;
        char[][] maze = new char[rows][];

        for (int i = 0; i < rows; i++)
        {
            maze[i] = new char[cols];
            for (int j = 0; j < cols; j++)
            {
                maze[i][j] = mazeData[i][j][0];
            }
        }

        return maze;
    }

    static int SolveMaze(char[][] maze)
    {
        int rows = maze.Length;
        int cols = maze[0].Length;

        var start = (-1, -1);
        var end = (-1, -1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (maze[i][j] == 'S') start = (i, j);
                if (maze[i][j] == 'E') end = (i, j);
            }
        }

        var directions = new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

        var queue = new Queue<(int, int, int)>();
        var visited = new bool[rows, cols];
        queue.Enqueue((start.Item1, start.Item2, 0));
        visited[start.Item1, start.Item2] = true;

        while (queue.Count > 0)
        {
            var (x, y, steps) = queue.Dequeue();

            if (x == end.Item1 && y == end.Item2)
            {
                return steps;
            }

            foreach (var (dx, dy) in directions)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newX < rows && newY >= 0 && newY < cols && !visited[newX, newY])
                {
                    if (maze[newX][newY] != '#' && maze[newX][newY] != 'B')
                    {
                        visited[newX, newY] = true;
                        queue.Enqueue((newX, newY, steps + 1));
                    }
                }
            }
        }

        return -1;
    }
}
