using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

/// <summary>
/// GameStateのインターフェース。各GameStateはこのインターフェースを実装する必要がある。
/// </summary>
internal interface IGameState
{
    ReactiveCommand<GameStateKey> NextState { get; }
    UniTask Enter();
    UniTask Tick();
    UniTask Exit();
}
