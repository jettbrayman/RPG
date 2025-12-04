using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace RPG
{
    internal class Program
    {
        static Dungeon dungeon;
        static Player player;
        static int playerX,
            playerY;
        static int prevX,
            prevY; // previous room (for run away)
        static Random ran = new Random();

        static void Main(string[] args)
        {
            MainMenu();
        }

        static void MainMenu() //main menu
        {
            while (true) //main menu ui
            {
                Console.Clear();
                Console.WriteLine("rpg dungeon game");
                Console.WriteLine("1: start new game");
                Console.WriteLine("2: load game from csv");
                Console.WriteLine("3: debug dungeon map");
                Console.WriteLine("4: close game");
                Console.Write("choose: ");

                string choice = Console.ReadLine();

                if (choice == "1") //sends user into main gameplay and starts a new game loop
                {
                    StartNewGame();
                    GameLoop();
                }
                else if (choice == "2") //loads sace data
                {
                    if (
                        SaveSystem.LoadGame(
                            out player,
                            out dungeon,
                            out playerX,
                            out playerY,
                            out prevX,
                            out prevY
                        )
                    ) //checks if user has saved data then loads data from csv file
                    {
                        GameLoop();
                    }
                    else
                    {
                        Console.WriteLine("press enter to return to start menu");

                        string input; //checks for blank imput

                        do
                        {
                            input = Console.ReadLine();

                            if (!string.IsNullOrEmpty(input))
                            {
                                Console.WriteLine("invalid input press enter");
                            }
                        } while (!string.IsNullOrEmpty(input));
                    }
                }
                else if (choice == "3") //shows debug map
                {
                    if (
                        SaveSystem.LoadGame(
                            out player,
                            out dungeon,
                            out playerX,
                            out playerY,
                            out prevX,
                            out prevY
                        )
                    ) //checks if user has a save and loads dungeon map from the save if user does not have a save load a random dungeon
                    {
                        ShowMap(dungeon);
                    }
                    else
                    {
                        Console.WriteLine("no save availible showing a random map:");
                        Dungeon ranDun = new Dungeon(); //generates a random dungeon using dungeon class
                        ranDun.GenerateRooms();
                        ShowMap(ranDun);
                    }

                    Console.WriteLine("press enter to remove debug menu");

                    string input;

                    do
                    {
                        input = Console.ReadLine();

                        if (!string.IsNullOrEmpty(input))
                        {
                            Console.WriteLine("invalid input press enter to return");
                        }
                    } while (!string.IsNullOrEmpty(input));
                }
                else //exits game
                {
                    return;
                }
            }
        }

        static void StartNewGame() //starts a new game
        {
            dungeon = new Dungeon(); //generates new dungeon object
            dungeon.GenerateRooms();
            player = new Player(); //generates new player object
            playerX = 2; //sets player data to defaults
            playerY = 2;
            prevX = playerX; //previous x and y coordinates are for deciding where to send player when player runs from a fight
            prevY = playerY;
        }

        static void GameLoop() //main game loop
        {
            bool playing = true;

            while (playing)
            {
                Console.Clear();
                Console.WriteLine(
                    $"player HP: {player.HP}/{player.MaxHP}  |  baseDMG: {player.BaseDMG}  |  swords: {player.Swords} potions: {player.Potions}  scrolls: {player.Scrolls}"
                ); //displays player object info
                Console.WriteLine(
                    $"location: ({playerX},{playerY}) | room type: {Room.NameFromType(dungeon.MapGrid[playerY, playerX])}"
                ); //displays room coordinates and the type of room the player is currrently in
                Console.WriteLine();

                int roomType = dungeon.MapGrid[playerY, playerX];
                string cord = Dungeon.Cord(playerX, playerY);

                if ((roomType == 2 || roomType == 5) && !dungeon.ClearedRooms.Contains(cord)) //checks if a room is a monster or boss room and if its a cleared room
                {
                    Monster enemy = CombatSystem.CreateMonster(roomType == 5);

                    FightOutcome result = CombatSystem.Fight(player, enemy, GetCombatChoice);

                    if (result == FightOutcome.PlayerDied)
                    {
                        Console.WriteLine("press enter to return to menu");
                        Console.ReadLine();
                        return; // back to main menu
                    }
                    else if (result == FightOutcome.PlayerWon)
                    {
                        dungeon.ClearedRooms.Add(cord); // only mark cleared if enemy actually died
                        Console.WriteLine("room cleared press enter to continue");
                        Console.ReadLine();
                        continue;
                    }
                    else if (result == FightOutcome.PlayerRanAway)
                    {
                        // move the player back to the previous room
                        playerX = prevX;
                        playerY = prevY;

                        Console.WriteLine(
                            "You ran away to the previous room. Press enter to continue"
                        );
                        Console.ReadLine();
                        continue;
                    }
                }

                if (roomType == 3 && !dungeon.OpenedChests.Contains(cord)) //in chest in this room is unopened then runs open chest information
                {
                    Console.WriteLine("treasure room you get 1 random treasure");
                    OpenChestAt(player, dungeon, playerX, playerY);
                    Console.WriteLine("press enter to continue");
                    Console.ReadLine();
                    continue;
                }

                if (roomType == 4) //healing room
                {
                    Console.WriteLine("healing room HP is now fully healed");
                    player.HP = player.MaxHP;
                }

                List<(string label, int newX, int newY, string action)> options =
                    new List<(string, int, int, string)>();

                //shows current movement options on the options selector
                //checks if the players surrounding options (y-1 y+1 x-1 x+1) are valid movements andd shows only the valid ones
                if (IsValidMove(playerX, playerY - 1)) //up
                    options.Add(("move up", playerX, playerY - 1, "move"));
                if (IsValidMove(playerX + 1, playerY)) //right
                    options.Add(("move right", playerX + 1, playerY, "move"));
                if (IsValidMove(playerX, playerY + 1)) //down
                    options.Add(("move down", playerX, playerY + 1, "move"));
                if (IsValidMove(playerX - 1, playerY)) //left
                    options.Add(("move left", playerX - 1, playerY, "move"));

                options.Add(("Save game", -1, -1, "save")); //option to save game or quit and gives an invalid movement option
                options.Add(("Quit to menu", -1, -1, "quit"));

                Console.WriteLine($"You have {options.Count} options:"); //display
                for (int i = 0; i < options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {options[i].label}");
                }

                int choiceIndex = -1;
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("please type a number: ");
                    string line = Console.ReadLine();
                    if (int.TryParse(line, out int num) && num >= 1 && num <= options.Count) //check if users selected option is valid
                    {
                        choiceIndex = num - 1;
                        break;
                    }
                    Console.WriteLine("invalid choice");
                }

                (string label, int newX, int newY, string action) selected = options[choiceIndex]; //calls selected option and the current player positions

                if (selected.action == "move")
                {
                    prevX = playerX; //save previous cords
                    prevY = playerY;
                    playerX = selected.newX;
                    playerY = selected.newY; //moves player cords to new movement cords
                    continue;
                }
                else if (selected.action == "save") //saves game
                {
                    SaveSystem.SaveGame(player, dungeon, playerX, playerY, prevX, prevY);
                    Console.WriteLine("press enter to continue");
                    Console.ReadLine();
                    continue;
                }
                else if (selected.action == "quit")
                {
                    return;
                }
            }
        }

        static bool IsValidMove(int x, int y) //checks if a move is in bounds of the dungeon map so not 0 1 2 3 4 5
        {
            if (x < 0 || x > 4 || y < 0 || y > 4)
            {
                return false;
            }
            return dungeon.MapGrid[y, x] != 0;
        }

        static void ShowMap(Dungeon d) //map function
        {
            Console.WriteLine("dungeon map:");
            for (int y = 0; y < 5; y++) //writes 5x5 dungeon map
            {
                for (int x = 0; x < 5; x++)
                {
                    int mapWriter = d.MapGrid[y, x];
                    string currentChar;

                    if (mapWriter == 0)
                    {
                        currentChar = ".";
                    }
                    else
                    {
                        currentChar = mapWriter.ToString();
                    }

                    Console.Write(currentChar + " ");
                }
                Console.WriteLine();
            }
        }
        static void OpenChestAt(Player p, Dungeon d, int x, int y) //open chest
        {
            string cord = Dungeon.Cord(x, y);

            int roll = ran.Next(100); //30% chance of nothing 30% a sword 20% a potion and 20% a scroll
            if (roll < 30)
            {
                Console.WriteLine("inside the chest was nothing");
            }
            else if (roll < 60) //sword
            {
                p.Swords++;
                Console.WriteLine("inside the chest was a sword (doubles damage and stackable).");
            }
            else if (roll < 80) //potion
            {
                p.Potions++;
                Console.WriteLine("inside the chest was a potion (restores health fully)");
            }
            else //scroll
            {
                p.Scrolls++;
                Console.WriteLine(
                    "inside the chest was a freeze scroll (freezes enemy for 2 turns)"
                );
            }

            d.OpenedChests.Add(cord); //adds the current coordinates of the player to the opened chests
        }

        static int GetCombatChoice(string context) // gets combat choice
        {
            while (true)
            {
                Console.Write("choice: ");
                string userSelection = Console.ReadLine();

                try
                {
                    int choice = int.Parse(userSelection);
                    return choice;
                }
                catch
                {
                    Console.WriteLine("invalid choice numbers only");
                }
            }
        }
    }
}
