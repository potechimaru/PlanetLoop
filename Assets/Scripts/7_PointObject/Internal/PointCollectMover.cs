using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 今は使っていない。PlayerがPointObjectを獲得する時に、PointObjectをPlayerの位置まで移動させるためのクラス。
/// 全く見えないので、今は使っていない。将来的に使うかもしれないので残しておく。
/// </summary>
public class PointCollectMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float arriveThreshold = 0.05f;

    private CancellationTokenSource _cts;

    public void BeginMove(Transform target)
    {
        // 多重防止
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        MoveAsync(target, _cts.Token).Forget();
    }

    /// <summary>
    /// 移動開始
    /// </summary>
    /// <param name="target">Player</param>
    /// <param name="token"></param>
    /// <returns></returns>
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