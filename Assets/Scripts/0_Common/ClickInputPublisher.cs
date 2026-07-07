using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// クリック入力を購読可能なイベントとして提供するクラス
/// </summary>
public class ClickInputPublisher : MonoBehaviour, IPointerClickHandler
{
    private readonly Subject<Unit> _onClicked = new();
    public IObservable<Unit> OnClicked => _onClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 左クリックのみ反応させたいという意思（笑）
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _onClicked.OnNext(Unit.Default);
    }

    private void OnDestroy()
    {
        _onClicked.Dispose();
    }
}