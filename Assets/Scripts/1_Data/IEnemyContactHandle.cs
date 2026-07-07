/// <summary>
/// PlayerがEnemyに接触した際の処理を定義するインターフェース
/// Assembly Diffinitionの循環参照問題を解決するために定義。今はあまり意味は無くなった。
/// Enemyに当たったらチャージに関係なく倒せるようにした。
/// </summary>
public interface IEnemyContactHandle
{
    void Defeat();
}