
/// <summary>
/// Žg‚Á‚Ä‚È‚¢
/// </summary>
public class DefeatEnemyEvent
{
    private readonly IEnemyExternalFacade _enemyExternalFacade;
    public DefeatEnemyEvent(IEnemyExternalFacade enemyExternalFacade)
    {
        _enemyExternalFacade = enemyExternalFacade;
    }

    public void DefeatEnemy(int defeatEnemyCount, int allEnemyCount)
    {
        
    }
}
