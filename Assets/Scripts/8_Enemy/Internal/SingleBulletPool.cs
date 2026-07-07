using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
public class SingleBulletPool : MonoBehaviour, IBulletReturner
{
    [Header("Prefab / Parent")]
    [SerializeField] private RectTransform prefab;
    [SerializeField] private RectTransform defaultParent;

    [Header("Pooling")]
    [SerializeField, Min(0)] private int prewarmCount = 10;
    [SerializeField] private bool setInactiveOnReturn = true;

    [Inject] private EnemyBulletManager _enemyBulletManager;

    [Inject] private Transform _playerTransform; 

    private readonly Stack<PooledBulletObject> _pool = new();
    private readonly HashSet<PooledBulletObject> _rented = new();

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

    private PooledBulletObject CreateNew(Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError($"{nameof(SingleBulletPool)}: prefab ‚ª–¢Ý’è‚Å‚·B", this);
            return null;
        }

        var rt = Instantiate(prefab, parent);
        var bullet = rt.GetComponent<PooledBulletObject>();
        if (bullet == null) bullet = rt.gameObject.AddComponent<PooledBulletObject>();
        bullet.Bind(this);

        if (setInactiveOnReturn) rt.gameObject.SetActive(false);
        return bullet;
    }

    public async UniTask Rent(Vector2 anchoredPos, Vector3 dirNormalized, float activeRadius)
    {
        PooledBulletObject item = _pool.Count > 0 ? _pool.Pop() : CreateNew(defaultParent);
        if (item == null) return;

        _rented.Add(item);

        _enemyBulletManager.RegisterBullet(item.GetComponent<EnemyBullet>());

        var rt = (RectTransform)item.transform;

        if (rt.parent != defaultParent) rt.SetParent(defaultParent, false);

        rt.anchoredPosition = anchoredPos;
        item.gameObject.SetActive(true);
        item.GetComponent<EnemyBullet>().Launch(dirNormalized, _playerTransform, activeRadius);

        await UniTask.CompletedTask;

    }

    public void Return(PooledBulletObject item)
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