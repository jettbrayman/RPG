using System;

namespace RPG
{
    public class TreasureChest //treasure chest and loot
    {
        public string Item { get; set; } //what is inside the chest

        public TreasureChest() //rolls loot when chest is created
        {
            Random rnd = new Random(); //random number for loot
            int roll = rnd.Next(100); //0-99

            if (roll < 30) //30% chance of nothing
                Item = "Nothing";
            else if (roll < 60) //30% chance of sword
                Item = "Sword";
            else if (roll < 80) //20% chance of potion
                Item = "Potion";
            else //20% chance of scroll
                Item = "Scroll";
        }

        public void Open(Player player) //gives item to player
        {
            Console.WriteLine();

            switch (Item)
            {
                case "Nothing": //no loot
                    Console.WriteLine("the chest is empty");
                    break;

                case "Sword": //gives sword
                    player.Swords++;
                    Console.WriteLine("you found a sword (damage doubled and stackable)");
                    break;

                case "Potion": //gives potion
                    player.Potions++;
                    Console.WriteLine("you found a healing potion");
                    break;

                case "Scroll": //gives scroll
                    player.Scrolls++;
                    Console.WriteLine("you found a freeze scroll");
                    break;
            }
        }
    }
}
