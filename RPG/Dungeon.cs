using System;
using System.Collections.Generic;

namespace RPG
{
    public class Dungeon
    {
        public int[,] MapGrid { get; set; } = new int[5, 5]; //5x5 dungeon map
        private Random ran = new Random();                   //random for room layout

        public List<string> OpenedChests { get; set; } = new List<string>(); //opened chest cords
        public List<string> ClearedRooms { get; set; } = new List<string>(); //cleared monster rooms

        public void GenerateRooms() //generates a random dungeon layout
        {
            //clear map
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    MapGrid[y, x] = 0;
                }
            }

            OpenedChests.Clear();
            ClearedRooms.Clear();

            //room layout list
            List<int> roomTypes = new List<int>
            {
                1,      //spawn
                2, 2, 2,//monsters
                3, 3,   //treasures
                4,      //heal
                5       //boss
            };

            int xPos = 2;
            int yPos = 2; //start in center
            MapGrid[yPos, xPos] = roomTypes[0];

            //place each room next to previous room if possible
            for (int i = 1; i < roomTypes.Count; i++)
            {
                bool placed = false;
                int tries = 0;

                while (!placed && tries < 500)
                {
                    tries++;

                    int dir = ran.Next(4);
                    int nextX = xPos;
                    int nextY = yPos;

                    if (dir == 0)
                    {
                        nextY--; //up
                    }
                    else if (dir == 1)
                    {
                        nextY++; //down
                    }
                    else if (dir == 2)
                    {
                        nextX--; //left
                    }
                    else if (dir == 3)
                    {
                        nextX++; //right
                    }

                    if (nextX < 0 || nextX > 4 || nextY < 0 || nextY > 4)
                    {
                        continue; //out of bounds
                    }

                    if (MapGrid[nextY, nextX] != 0)
                    {
                        continue; //already a room here
                    }

                    MapGrid[nextY, nextX] = roomTypes[i];
                    xPos = nextX;
                    yPos = nextY;
                    placed = true;
                }

                //backup placement if random walk fails
                if (!placed)
                {
                    for (int y = 0; y < 5 && !placed; y++)
                    {
                        for (int x = 0; x < 5 && !placed; x++)
                        {
                            if (MapGrid[y, x] == 0)
                            {
                                MapGrid[y, x] = roomTypes[i];
                                placed = true;
                            }
                        }
                    }
                }
            }
        }

        public static string Cord(int x, int y) //converts cords to "x,y"
        {
            return $"{x},{y}";
        }
    }
}