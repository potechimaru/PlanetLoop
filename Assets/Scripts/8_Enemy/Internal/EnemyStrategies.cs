/// <summary>
/// EnemyのStrategyをまとめた構造体。Enemyの行動パターンを管理する。
/// </summary>
public struct EnemyStrategies
{
    public IDetectStrategy Detect;
    public IMoveStrategy Move;
    public IAttackStrategy Attack;

    public EnemyStrategies(
        IDetectStrategy detect,
        IMoveStrategy move,
        IAttackStrategy attack)
    {
        Detect = detect;
        Move = move;
        Attack = attack;
    }
}