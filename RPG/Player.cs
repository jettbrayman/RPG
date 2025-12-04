using System;

namespace RPG
{
    public class Player //player stats and items
    {
        public int HP { get; set; } = 30;
        public int MaxHP { get; set; } = 30;
        public int BaseDMG { get; set; } = 5;
        public int Energy { get; set; } = 10; //unused right now

        public int Swords { get; set; } = 0; //each sword doubles damage
        public int Potions { get; set; } = 0; //each potion heals to full
        public int Scrolls { get; set; } = 0; //each scroll freezes enemy for 2 turns

        public int EffectiveDamage() //returns final damage
        {
            //damage = baseDMG * $2^{swords}$
            int multiplier = 1 << Swords; //bit shift is same as $2^{swords}$
            return BaseDMG * multiplier;
        }

        public string InventoryToString() //converts inventory to string for save
        {
            return $"{Swords}|{Potions}|{Scrolls}";
        }

        public void LoadInventoryFromString(string data) //loads inventory from save string
        {
            string[] parts = data.Split('|');

            if (parts.Length >= 3)
            {
                int swords;
                int potions;
                int scrolls;

                if (int.TryParse(parts[0], out swords))
                {
                    Swords = swords;
                }

                if (int.TryParse(parts[1], out potions))
                {
                    Potions = potions;
                }

                if (int.TryParse(parts[2], out scrolls))
                {
                    Scrolls = scrolls;
                }
            }
        }
    }
}
