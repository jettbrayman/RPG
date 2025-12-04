using System;

namespace RPG
{
    public static class DebugMenu
    {
        public static void ShowMap(int[,] grid) //debug map display
        {
            Console.WriteLine("debug map:");

            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    int value = grid[y, x];

                    if (value == 0)
                    {
                        Console.Write(" "); //no room
                    }
                    else
                    {
                        Console.Write(value); //room type number
                    }

                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
