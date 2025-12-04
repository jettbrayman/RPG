namespace RPG
{
    public static class Room
    {
        //0 = empty
        //1 = spawn
        //2 = monster
        //3 = treasure
        //4 = heal
        //5 = boss
        public static string NameFromType(int type) //returns name from room id
        {
            if (type == 1)
            {
                return "spawn";
            }
            else if (type == 2)
            {
                return "monster";
            }
            else if (type == 3)
            {
                return "treasure";
            }
            else if (type == 4)
            {
                return "heal";
            }
            else if (type == 5)
            {
                return "boss";
            }

            return "empty";
        }
    }
}
