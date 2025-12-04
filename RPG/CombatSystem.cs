using System;

namespace RPG
{
    public enum FightOutcome //combat result
    {
        PlayerDied,
        PlayerWon,
        PlayerRanAway
    }

    public static class CombatSystem
    {
        public static Monster CreateMonster(bool isBoss) //creates monster or boss
        {
            if (isBoss) //boss room
            {
                Monster boss = new Monster();
                boss.IsBoss = true;
                boss.HP = 40; //boss hp
                boss.DMG = 6; //boss damage
                return boss;
            }
            else //normal monster room
            {
                Monster monster = new Monster();
                monster.IsBoss = false;
                monster.HP = 20; //monster hp
                monster.DMG = 4; //monster damage
                return monster;
            }
        }

        public static FightOutcome Fight(Player player, Monster enemy, Func<string, int> getChoice) //main combat loop
        {
            Console.WriteLine(
                $"a {(enemy.IsBoss ? "boss" : "monster")} appears enemy HP: {enemy.HP}"
            );

            while (player.HP > 0 && enemy.HP > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"player HP: {player.HP}/{player.MaxHP} | enemy HP: {enemy.HP}");
                Console.WriteLine("1: attack");
                Console.WriteLine("2: use potion");
                Console.WriteLine("3: use freeze scroll");
                Console.WriteLine("4: run away");

                int choice = getChoice("combat"); //gets combat choice from program

                if (choice == 1) //attack
                {
                    int dmg = player.EffectiveDamage();
                    enemy.HP -= dmg;
                    Console.WriteLine($"you hit for {dmg} damage");
                }
                else if (choice == 2) //potion
                {
                    if (player.Potions > 0)
                    {
                        player.Potions--;
                        player.HP = player.MaxHP;
                        Console.WriteLine("you use a potion and fully heal");
                    }
                    else
                    {
                        Console.WriteLine("you do not have any potions");
                        continue;
                    }
                }
                else if (choice == 3) //scroll
                {
                    if (player.Scrolls > 0)
                    {
                        player.Scrolls--;
                        enemy.FrozenTurns += 2;
                        Console.WriteLine(
                            "you use a freeze scroll the enemy is frozen for 2 turns"
                        );
                    }
                    else
                    {
                        Console.WriteLine("you do not have any scrolls");
                        continue;
                    }
                }
                else if (choice == 4) //run away
                {
                    Console.WriteLine("you run away");
                    return FightOutcome.PlayerRanAway;
                }
                else
                {
                    Console.WriteLine("invalid combat choice");
                    continue;
                }

                //enemy turn
                if (enemy.HP > 0)
                {
                    if (enemy.FrozenTurns > 0)
                    {
                        enemy.FrozenTurns--;
                        Console.WriteLine("enemy is frozen and skips its turn");
                    }
                    else
                    {
                        player.HP -= enemy.DMG;
                        Console.WriteLine($"enemy hits for {enemy.DMG} damage");
                    }
                }
            }

            if (player.HP <= 0) //player dead
            {
                Console.WriteLine("you have been defeated");
                return FightOutcome.PlayerDied;
            }

            Console.WriteLine("enemy defeated");
            return FightOutcome.PlayerWon;
        }
    }
}
