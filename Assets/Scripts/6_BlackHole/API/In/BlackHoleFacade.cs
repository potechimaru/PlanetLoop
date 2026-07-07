using System;
using UniRx;
using UnityEngine;

public interface IBlackHoleFacade
{
    Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt);

    IObservable<Unit> OnPlayerEnteredBlackHole { get; }
    IObservable<Unit> OnPlayerExitedOuterLimit { get; }
}

/// <summary>
/// BlackHoleコンポーネント群の内部メソッドを外部に公開するFacade。
/// 内部構造を隠蔽し、外部からのアクセスを簡素化する役割を持つ。
/// </summary>
public class BlackHoleFacade : IBlackHoleFacade
{
    private readonly BlackHoleGravity _blackHoleGravity;
    private readonly BlackHoleDetector _blackHoleDetector;

    public BlackHoleFacade(
        BlackHoleGravity blackHoleGravity,
        BlackHoleDetector blackHoleDetector)
    {
        _blackHoleGravity = blackHoleGravity;
        _blackHoleDetector = blackHoleDetector;
    }

    public IObservable<Unit> OnPlayerEnteredBlackHole
        => _blackHoleDetector.OnPlayerEnteredBlackHole;

    public IObservable<Unit> OnPlayerExitedOuterLimit
        => _blackHoleDetector.OnPlayerExitedOuterLimit;

    public Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt)
    {
        return _blackHoleGravity.BendDirection(worldPos, dir, dt);
    }
}