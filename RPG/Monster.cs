using System;

namespace RPG
{
    public class Monster //monster and boss stats
    {
        public bool IsBoss { get; set; } = false; //true if boss room
        public int HP { get; set; } = 15; //health
        public int DMG { get; set; } = 3; //damage per hit
        public int FrozenTurns { get; set; } = 0; //how many turns the monster is frozen
    }
}
