using Cysharp.Threading.Tasks;

/// <summary>
/// AttackStrategyで実装するインターフェース
/// </summary>
public interface IAttackStrategy
{
    UniTask OnEnterTelegraph();  // 予告開始時に必要なら初期化
    void TickTelegraph();        // 予告中の見た目更新など
    UniTask Fire();              // 発射（ここで弾生成/レーザー開始など）
}