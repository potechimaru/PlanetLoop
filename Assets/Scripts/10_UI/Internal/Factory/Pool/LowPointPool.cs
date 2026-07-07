using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;

public class LowPointPool : MonoBehaviour, IPlayUIReturner
{
    [Header("Prefab / Parent")]
    [SerializeField] private RectTransform prefab;
    [SerializeField] private RectTransform defaultParent;

    [Header("Pooling")]
    [SerializeField, Min(0)] private int prewarmCount = 10;
    [SerializeField] private bool setInactiveOnReturn = true;

    private readonly Stack<PooledPlayUIItem> _pool = new();
    private readonly HashSet<PooledPlayUIItem> _rented = new();

    private void Awake()
    {
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var item = CreateNew(defaultParent);
            Return(item);
        }
    }

    private PooledPlayUIItem CreateNew(Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError($"{nameof(NewOrbitPointPool)}: prefab Ç™ñ¢ê›íËÇ≈Ç∑ÅB", this);
            return null;
        }

        var rt = Instantiate(prefab, parent);
        var item = rt.GetComponent<PooledPlayUIItem>();
        if (item == null) item = rt.gameObject.AddComponent<PooledPlayUIItem>();
        item.Bind(this);

        if (setInactiveOnReturn) rt.gameObject.SetActive(false);
        return item;
    }

    public async UniTask Rent(Vector2 anchoredPos, RectTransform parent = null)
    {
        var p = parent != null ? parent : defaultParent;

        PooledPlayUIItem item = _pool.Count > 0 ? _pool.Pop() : CreateNew(p);
        if (item == null) return;

        _rented.Add(item);

        var rt = (RectTransform)item.transform;
        if (rt.parent != p) rt.SetParent(p, false);

        rt.anchoredPosition = anchoredPos;
        item.gameObject.SetActive(true);

        try
        {
            var anim = item.GetComponent<PopUpAnimation>();
            if (anim != null)
            {
                await anim.PlayAsync(item.GetCancellationTokenOnDestroy());
            }

            if (item != null && item.gameObject.activeInHierarchy)
            {
                item.ReturnToPool();
            }
        }
        catch (OperationCanceledException)
        {
            // í èÌèIóπàµÇ¢
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void Return(PooledPlayUIItem item)
    {
        if (item == null) return;

        if (_rented.Contains(item))
        {
            _rented.Remove(item);
        }

        if (setInactiveOnReturn)
        {
            item.gameObject.SetActive(false);
        }

        if (defaultParent != null)
        {
            item.transform.SetParent(defaultParent, false);
        }

        _pool.Push(item);
    }
}