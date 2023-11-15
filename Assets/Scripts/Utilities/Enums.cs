namespace Enums
{

    public enum GameState { exploration, combatPreparation,combatReady,combatEnded, paused }

    public enum CombatResult { victory, defeat,escape}

    public enum CombatTurn { playerTurn,enemyTurn}

    public enum ResourceType { none,ammo, energy, health}

    public enum DamageType { physical,magical}

    public enum BuffType { offense, defense }

    public enum EntityState { inactive, idle,thinking,acting, dead }

    public enum TargetingStyle { self, single, multiple}

    public enum RegenStyle { None, Punch, Shoot }
}
