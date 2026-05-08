using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickInputPublisher : MonoBehaviour, IPointerClickHandler
{
    private readonly Subject<Unit> _onClicked = new();
    public IObservable<Unit> OnClicked => _onClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        // ¶ƒNƒŠƒbƒN‚Ì‚İ”½‰‚³‚¹‚½‚¢ê‡
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _onClicked.OnNext(Unit.Default);
    }

    private void OnDestroy()
    {
        _onClicked.Dispose();
    }
}