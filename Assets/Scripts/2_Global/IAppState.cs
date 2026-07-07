using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public interface IAppState
{
    ReactiveCommand<AppStateKey> NextState { get; }
    UniTask Enter();
    UniTask Exit();

}
