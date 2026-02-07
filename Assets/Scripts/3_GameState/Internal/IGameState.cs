using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

internal interface IGameState
{
    ReactiveCommand<GameStateKey> NextState { get; }
    UniTask Enter();
    UniTask Tick();
    UniTask Exit();
}
