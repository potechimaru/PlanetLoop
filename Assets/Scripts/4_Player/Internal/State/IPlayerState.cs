using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

/// <summary>
/// PlayerStateの状態を表すインターフェース。プレイヤーの状態遷移を管理するためのメソッドを定義する。
/// </summary>
internal interface IPlayerState
{
    ReactiveCommand<PlayerStateKey> NextState { get; }
    UniTask Enter();
    UniTask Tick();
    UniTask Exit();
}
