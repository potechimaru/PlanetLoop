using Cysharp.Threading.Tasks;
using UniRx;

/// <summary>
/// EnemyStateの状態を表す列挙型。敵の状態遷移を管理するためのキーを定義する。
/// </summary>
public enum EnemyStateKey
{
    Idle,       // 発見待ち
    Telegraph,  // 予告
    Cooldown    // クールダウン
}