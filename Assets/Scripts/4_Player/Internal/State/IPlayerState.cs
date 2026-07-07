using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

internal interface IPlayerState
{
    ReactiveCommand<PlayerStateKey> NextState { get; }
    UniTask Enter();
    UniTask Tick();
    UniTask Exit();
}
