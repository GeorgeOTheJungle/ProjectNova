namespace Enums
{

    public enum GameState { exploration,explorationTransition,messagePrompt, combatPreparation,combatReady,combatEnded, paused }

    public enum CombatResult {none, victory, defeat,escape}

    public enum CombatTurn { playerTurn,enemyTurn}

    public enum ResourceType { none,ammo, energy, health}

    public enum DamageType { physical,magical}

    public enum StatusEffectType { none, fire, ice, weaken}
    public enum BuffType { offense, defense }

    public enum EntityType { player,enemy,boss}
    public enum EntityState { inactive, idle,thinking,acting, dead }

    public enum TargetingStyle { self, single, multiple}

    public enum RegenStyle { None, Punch, Shoot }

    public enum PlayerStat { health,energy,ammo,physicalDmg,physicalArmor,magicalDmg,magicalArmor,critRate,shoot,punch}

    public enum FrameEvent { playParticles,damagePlayer,stopParticles}
}
