using Cysharp.Threading.Tasks;
using UniRx;

public enum EnemyStateKey
{
    Idle,       // 発見待ち
    Telegraph,  // 予告
    Cooldown    // クールダウン
}