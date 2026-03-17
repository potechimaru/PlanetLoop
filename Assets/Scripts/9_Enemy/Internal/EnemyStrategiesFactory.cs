public static class EnemyStrategyFactory
{
    public static EnemyStrategies Create(
        EnemyType type,
        EnemyController enemyController)
    {
        return type switch
        {
            EnemyType.Enemy1 => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new FixedMoveStrategy(),
                new SingleShotAttackStrategy(enemyController)
            ),

            _ => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new WanderInCircleMoveStrategy(enemyController),
                new SingleShotAttackStrategy(enemyController)
            )
        };
    }
}