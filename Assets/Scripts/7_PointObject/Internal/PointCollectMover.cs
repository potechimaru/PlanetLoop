using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PointCollectMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float arriveThreshold = 0.05f;

    private CancellationTokenSource _cts;

    public void BeginMove(Transform target)
    {
        // ëΩèdñhé~
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        MoveAsync(target, _cts.Token).Forget();
    }

    private async UniTask MoveAsync(
        Transform target,
        CancellationToken token)
    {
        try
        {
            while (target != null)
            {
                token.ThrowIfCancellationRequested();

                Vector3 targetPos = target.position;

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime);

                float sqrDist =
                    (transform.position - targetPos).sqrMagnitude;

                if (sqrDist <= arriveThreshold * arriveThreshold)
                {
                    gameObject.SetActive(false);
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (System.OperationCanceledException)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}