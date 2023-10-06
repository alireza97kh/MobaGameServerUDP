namespace Dobeil
{
    public enum CreepState
    {
        //Idle,
        Moving,
        Attacking,
        Dead
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