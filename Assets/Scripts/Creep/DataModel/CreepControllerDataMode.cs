namespace Dobeil
{
    public enum CreepState : ushort
    {
        Idle = 0,
        Moving = 1,
        MoveToTarget = 2,
        Attacking = 3,
        Dead = 4
    }

    public enum CreepStateAction
    {
        DoNothing,
        KeepMoving,
        MoveToEnemyCreep,
        MoveToEnemyHero,
        MoveToEnemyTower,
        AttackToEnemyCreep,
        AttackToEnemyHero,
        AttackToEnemyTower,
    }

    [System.Serializable]
    public class CreepScore
    {
        public float DoNothing;
        public float KeepMoving;
        public float MoveToEnemyCreep;
        public float MoveToEnemyHero;
        public float MoveToEnemyTower;
        public float AttackToEnemyCreep;
        public float AttackToEnemyHero;
        public float AttackToEnemyTower;
    }
}