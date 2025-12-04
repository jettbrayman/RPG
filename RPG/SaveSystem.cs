using System;
using System.Collections.Generic;
using System.IO;

namespace RPG
{
    public static class SaveSystem //save and load system
    {
        private const string FileName = "savegame.csv"; //save file name

        //save player and dungeon data
        public static void SaveGame(
            Player player,
            Dungeon dungeon,
            int playerX,
            int playerY,
            int prevX,
            int prevY
        )
        {
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                //player stats
                //PLAYER,HP,MaxHP,BaseDMG,Energy,Inventory
                sw.WriteLine(
                    "PLAYER,"
                        + player.HP
                        + ","
                        + player.MaxHP
                        + ","
                        + player.BaseDMG
                        + ","
                        + player.Energy
                        + ","
                        + player.InventoryToString()
                );

                //player position
                //POS,x,y,prevX,prevY
                sw.WriteLine("POS," + playerX + "," + playerY + "," + prevX + "," + prevY);

                //dungeon map rows
                for (int y = 0; y < 5; y++)
                {
                    List<string> row = new List<string>();
                    for (int x = 0; x < 5; x++)
                    {
                        row.Add(dungeon.MapGrid[y, x].ToString());
                    }
                    sw.WriteLine("ROW," + string.Join(",", row)); //row of map types
                }

                //opened chest coords
                sw.WriteLine("OPENED," + string.Join(";", dungeon.OpenedChests));

                //cleared room coords
                sw.WriteLine("CLEARED," + string.Join(";", dungeon.ClearedRooms));
            }

            Console.WriteLine("game saved to " + FileName);
        }

        //load player and dungeon data
        public static bool LoadGame(
            out Player player,
            out Dungeon dungeon,
            out int playerX,
            out int playerY,
            out int prevX,
            out int prevY
        )
        {
            player = new Player();
            dungeon = new Dungeon();
            playerX = 2;
            playerY = 2;
            prevX = 2;
            prevY = 2;

            if (!File.Exists(FileName))
            {
                Console.WriteLine("save file not found");
                return false;
            }

            try
            {
                string[] lines = File.ReadAllLines(FileName);
                int rowCounter = 0; //current map row index

                foreach (string raw in lines)
                {
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        continue;
                    }

                    string[] parts = raw.Split(',');
                    string tag = parts[0];

                    if (tag == "PLAYER") //player stats line
                    {
                        int hp;
                        int maxhp;
                        int baseDmg;
                        int energy;

                        int.TryParse(parts[1], out hp);
                        int.TryParse(parts[2], out maxhp);
                        int.TryParse(parts[3], out baseDmg);
                        int.TryParse(parts[4], out energy);

                        player.HP = hp;
                        player.MaxHP = maxhp;
                        player.BaseDMG = baseDmg;
                        player.Energy = energy;

                        //inventory string
                        if (parts.Length > 5)
                        {
                            string invString = parts[5];
                            player.LoadInventoryFromString(invString);
                        }
                    }
                    else if (tag == "POS") //player position line
                    {
                        int.TryParse(parts[1], out playerX);
                        int.TryParse(parts[2], out playerY);
                        int.TryParse(parts[3], out prevX);
                        int.TryParse(parts[4], out prevY);
                    }
                    else if (tag == "ROW") //dungeon row
                    {
                        //ROW,v0,v1,v2,v3,v4
                        for (int x = 0; x < 5; x++)
                        {
                            int value;
                            int.TryParse(parts[x + 1], out value);
                            dungeon.MapGrid[rowCounter, x] = value;
                        }
                        rowCounter++;
                    }
                    else if (tag == "OPENED") //opened chest coords
                    {
                        int commaIndex = raw.IndexOf(',');
                        if (commaIndex >= 0 && commaIndex < raw.Length - 1)
                        {
                            string tail = raw.Substring(commaIndex + 1);
                            if (!string.IsNullOrWhiteSpace(tail))
                            {
                                string[] coords = tail.Split(';');
                                for (int i = 0; i < coords.Length; i++)
                                {
                                    string coord = coords[i].Trim();
                                    if (coord.Length > 0)
                                    {
                                        dungeon.OpenedChests.Add(coord);
                                    }
                                }
                            }
                        }
                    }
                    else if (tag == "CLEARED") //cleared room coords
                    {
                        int commaIndex = raw.IndexOf(',');
                        if (commaIndex >= 0 && commaIndex < raw.Length - 1)
                        {
                            string tail = raw.Substring(commaIndex + 1);
                            if (!string.IsNullOrWhiteSpace(tail))
                            {
                                string[] coords = tail.Split(';');
                                for (int i = 0; i < coords.Length; i++)
                                {
                                    string coord = coords[i].Trim();
                                    if (coord.Length > 0)
                                    {
                                        dungeon.ClearedRooms.Add(coord);
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("save found");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("no save found: " + ex.Message);
                return false;
            }
        }
    }
}
