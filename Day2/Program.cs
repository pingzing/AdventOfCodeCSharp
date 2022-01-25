using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        enum Command
        {
            Forward,
            Down,
            Up,
        }

        static void Main(string[] args)
        {
            List<(Command command, int value)> commands = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "input.txt"))
                .Select(x =>
                {
                    string[] commandAndValue = x.Split(' ');
                    Command comm = commandAndValue[0] switch
                    {
                        "forward" => Command.Forward,
                        "down" => Command.Down,
                        "up" => Command.Up,
                        _ => throw new ArgumentOutOfRangeException($"'{commandAndValue[0]}' is not a valid command. What the hell, input?")
                    };
                    return (comm, int.Parse(commandAndValue[1]));
                }).ToList();

            PartOne(commands);
            PartTwo(commands);
        }

        static void PartOne(IEnumerable<(Command command, int value)> commands)
        {
            int horizontalPos = 0;
            int depth = 0;

            foreach ((Command comm, int val) in commands)
            {
                switch (comm)
                {
                    case Command.Forward:
                        horizontalPos += val;
                        break;
                    case Command.Down:
                        depth += val;
                        break;
                    case Command.Up:
                        depth -= val;
                        break;
                }
            }

            Console.WriteLine($"Horiz pos: {horizontalPos}, Depth: {depth}, multiplied together: {horizontalPos * depth}");
        }

        static void PartTwo(IEnumerable<(Command commmand, int value)> commands)
        {
            int aim = 0;
            int horizPos = 0;
            int depth = 0;

            foreach ((Command comm, int val) in commands)
            {
                switch (comm)
                {
                    case Command.Forward:
                        horizPos = horizPos + val;
                        depth = depth + (aim * val);
                        break;
                    case Command.Up:
                        aim = aim - val;
                        break;
                    case Command.Down:
                        aim = aim + val;
                        break;
                }
            }

            Console.WriteLine($"Part two multiplied together: {(long)horizPos * depth}");
        }
    }
}
