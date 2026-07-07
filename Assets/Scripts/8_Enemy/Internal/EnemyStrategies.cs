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