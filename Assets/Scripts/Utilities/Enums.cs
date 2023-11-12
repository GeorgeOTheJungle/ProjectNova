namespace Enums
{

    public enum GameState { exploration, combatPreparation,combatReady, paused }

    public enum CombatResult { victory, defeat,escape}

    public enum CombatTurn { playerTurn,enemyTurn}

    public enum ResourceType { none,ammo, energy, health}

    public enum DamageType { physical,magical}

    public enum BuffType { offense, defense }
}
