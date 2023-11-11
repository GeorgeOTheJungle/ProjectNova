using System.Collections.Generic;

namespace Structs
{

    [System.Serializable]
    public struct Stats
    {
        public int health;
        public int energy;
        public int ammo;

        public int physicalDamage;
        public int physicalArmor;

        public int magicDamage;
        public int magicArmor;

        public float defenseBonus; // For guarding
        public float buffBonus; // Buffing

        public int accuracy;
        public int critRate;
        public int xpYield;
    }

    [System.Serializable]
    public struct Encounter
    {
        public List<Entity> entites;
        public List<EntityData> entitiesData;
    }
}
