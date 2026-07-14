
/// <summary>
/// Enemy‚Ìí—Ş‚É‰‚¶‚½í—ª‚ğ¶¬‚·‚éFactoryƒNƒ‰ƒX
/// </summary>
public static class EnemyStrategyFactory
{

    public static EnemyStrategies Create(
        EnemyType type,
        EnemyController enemyController)
    {
        return type switch
        {
            EnemyType.Enemy5 => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new FixedMoveStrategy(),
                new LaserAttackStrategy(enemyController)
            ),

            EnemyType.Enemy1 => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new FixedMoveStrategy(),
                new SingleShotAttackStrategy(enemyController)
            ),

            EnemyType.Enemy2 => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new FixedMoveStrategy(),
                new ThreeWayAttackStrategy(enemyController, 25f)
            ),

            EnemyType.Enemy3 => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new FixedMoveStrategy(),
                new SingleShotAttackStrategy(enemyController)
            ),

            EnemyType.Enemy4 => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new FixedMoveStrategy(),
                new ThreeWayAttackStrategy(enemyController, 15f)
            ),

            _ => new EnemyStrategies(
                new CircleDetectStrategy(enemyController),
                new FixedMoveStrategy(),
                new SingleShotAttackStrategy(enemyController)
            )
        };
    }
}