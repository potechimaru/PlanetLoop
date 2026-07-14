using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;


/// <summary>
/// GameModeSelectの時、ゲームモードのスロットを回転させるためのコントローラークラス。スロットの表示やアニメーションを管理する。
/// </summary>
public class GameModeCarouselController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private List<GameModeSlotView> _slotViews = new();

    private IGameModeManager _gameModeManager;

    private bool _isAnimating;
    private int _slotOffset = 0;

    [Inject]
    public void Construct(IGameModeManager gameModeManager)
    {
        _gameModeManager = gameModeManager;
    }

    /// <summary>
    /// TitleからGameModeSelectに遷移したときに、スロットの配置を初期化するためのメソッド。スロットの位置を即座に更新する。
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask PlayFormationAsync(CancellationToken cancellationToken = default)
    {
        if (_gameModeManager == null)
            return;

        try
        {
            await PlayAssignmentsAsync(
                DummyPlayerSplineStopAnimator.MoveDirection.Forward,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(GameModeCarouselController)} PlayFormationAsync Error: {ex}", this);
        }
    }

    /// <summary>
    /// スロットを右に回転させるためのメソッド。スロットの位置をアニメーションで更新する。
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask RotateRightAsync(CancellationToken cancellationToken = default)
    {
        if (_gameModeManager == null || _isAnimating)
            return;

        _isAnimating = true;

        try
        {
            _gameModeManager.RotateRight();

            int count = GetValidSlotCount();
            if (count > 0)
            {
                _slotOffset = (_slotOffset - 1 + count) % count;
            }

            await PlayAssignmentsAsync(
                DummyPlayerSplineStopAnimator.MoveDirection.Backward,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(GameModeCarouselController)} RotateRightAsync Error: {ex}", this);
        }
        finally
        {
            _isAnimating = false;
        }
    }

    /// <summary>
    /// スロットを左に回転させるためのメソッド。スロットの位置をアニメーションで更新する。
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask RotateLeftAsync(CancellationToken cancellationToken = default)
    {
        if (_gameModeManager == null || _isAnimating)
            return;

        _isAnimating = true;

        try
        {
            _gameModeManager.RotateLeft();

            int count = GetValidSlotCount();
            if (count > 0)
            {
                _slotOffset = (_slotOffset + 1) % count;
            }

            await PlayAssignmentsAsync(
                DummyPlayerSplineStopAnimator.MoveDirection.Forward,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(GameModeCarouselController)} RotateLeftAsync Error: {ex}", this);
        }
        finally
        {
            _isAnimating = false;
        }
    }

    public void RefreshImmediate()
    {
        if (_gameModeManager == null)
            return;

        try
        {
            var assignments = BuildAssignments();
            int count = assignments.Count;

            for (int i = 0; i < count; i++)
            {
                var assignment = assignments[i];

                if (assignment.DummyPlayerAnimator == null || assignment.Spline == null)
                    continue;

                float totalLength = assignment.Spline.GetTotalLength();
                if (totalLength <= 1e-6f)
                    continue;

                float spacing = totalLength / count;
                int rotatedIndex = (i + _slotOffset) % count;
                float distance = spacing * rotatedIndex;

                assignment.DummyPlayerAnimator.Initialize(assignment.Spline);
                assignment.DummyPlayerAnimator.SetPositionImmediate(distance);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(GameModeCarouselController)} RefreshImmediate Error: {ex}", this);
        }
    }

    public GameModeType GetCurrentSelectedMode()
    {
        if (_gameModeManager == null)
            return default;

        return _gameModeManager.CurrentSelectedMode;
    }

    private async UniTask PlayAssignmentsAsync(
        DummyPlayerSplineStopAnimator.MoveDirection moveDirection,
        CancellationToken cancellationToken)
    {
        var assignments = BuildAssignments();
        int count = assignments.Count;

        var tasks = new List<UniTask>(count);

        for (int i = 0; i < count; i++)
        {
            var assignment = assignments[i];

            if (assignment.DummyPlayerAnimator == null || assignment.Spline == null)
                continue;

            float totalLength = assignment.Spline.GetTotalLength();
            if (totalLength <= 1e-6f)
                continue;

            float spacing = totalLength / count;
            int rotatedIndex = (i + _slotOffset) % count;
            float distance = spacing * rotatedIndex;

            assignment.DummyPlayerAnimator.Initialize(assignment.Spline);
            tasks.Add(
                assignment.DummyPlayerAnimator.PlayToDistanceAsync(
                    distance,
                    moveDirection,
                    cancellationToken));
        }

        await UniTask.WhenAll(tasks);
    }

    private int GetValidSlotCount()
    {
        return _slotViews.Count(x => x != null);
    }

    private List<GameModeAssignment> BuildAssignments()
    {
        if (_gameModeManager == null)
            return new List<GameModeAssignment>();

        var orderedEntries = _gameModeManager.Entries
            .Where(x => x != null)
            .OrderBy(x => x.Order)
            .ToList();

        var orderedSlots = _slotViews
            .Where(x => x != null)
            .OrderBy(x => x.Order)
            .ToList();

        if (orderedEntries.Count != orderedSlots.Count)
        {
            throw new InvalidOperationException(
                $"{nameof(GameModeCarouselController)}: GameModeDummyEntry??({orderedEntries.Count}) ?? GameModeSlotView??({orderedSlots.Count}) ????v??????????B");
        }

        var slotSplines = orderedSlots.Select(x => x.Spline).ToList();
        var assignedSplines = _gameModeManager.GetCurrentSplineAssignments(slotSplines);

        var result = new List<GameModeAssignment>(orderedEntries.Count);

        for (int i = 0; i < orderedEntries.Count; i++)
        {
            result.Add(new GameModeAssignment(
                orderedEntries[i].DummyPlayerAnimator,
                assignedSplines[i]));
        }

        return result;
    }

    public void ActiveSlot()
    {
        if (_gameModeManager == null)
            return;

        foreach (var slot in _slotViews)
        {
            if (slot == null)
                continue;

            slot.gameObject.SetActive(true);
        }
    }

    private readonly struct GameModeAssignment
    {
        public DummyPlayerSplineStopAnimator DummyPlayerAnimator { get; }
        public ClosedSplineLine Spline { get; }

        public GameModeAssignment(
            DummyPlayerSplineStopAnimator dummyPlayerAnimator,
            ClosedSplineLine spline)
        {
            DummyPlayerAnimator = dummyPlayerAnimator;
            Spline = spline;
        }
    }
}