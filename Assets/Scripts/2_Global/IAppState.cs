using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// Applicationの状態を表すインターフェース。各状態はこのインターフェースを実装する必要がある。
/// </summary>
public interface IAppState
{
    ReactiveCommand<AppStateKey> NextState { get; }
    UniTask Enter();
    UniTask Exit();

}
