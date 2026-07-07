using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

public class EnemySimulationRangeController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float activeRadius = 12f;
    [SerializeField, Min(0.05f)] private float checkInterval = 0.2f;

    private EnemyManager _enemyManager;
    private CancellationToken _destroyToken;

    [Inject]
    public void Construct(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
    }

    private void Awake()
    {
        _destroyToken = this.GetCancellationTokenOnDestroy();
    }

    private void Start()
    {
        CheckLoopAsync(_destroyToken).Forget();
    }

    private async UniTaskVoid CheckLoopAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                if (player != null && _enemyManager != null)
                {
                    _enemyManager.UpdateEnemySimulationByDistance(
                        player.position,
                        activeRadius
                    );
                }

                await UniTask.Delay(
                    TimeSpan.FromSeconds(checkInterval),
                    cancellationToken: ct
                );
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public float GetActiveRadius() => activeRadius;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, activeRadius);
    }
#endif
}