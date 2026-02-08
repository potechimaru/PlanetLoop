using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スプライン（点列）と位置との距離判定を行う純粋クラス
/// </summary>
internal static class OrbitDistanceEvaluator
{
    /// <summary>
    /// 点列で表されたスプラインに対して、位置が一定距離以内かを判定
    /// </summary>
    public static bool IsTouching(
        IReadOnlyList<Vector3> samples,
        Vector3 position,
        float threshold)
    {
        if (samples == null || samples.Count < 2)
            return false;

        for (int i = 0; i < samples.Count - 1; i++)
        {
            if (DistancePointToSegment(position, samples[i], samples[i + 1]) <= threshold)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 点と線分の最短距離
    /// </summary>
    private static float DistancePointToSegment(
        Vector3 p,
        Vector3 a,
        Vector3 b)
    {
        Vector3 ab = b - a;
        float abSqr = ab.sqrMagnitude;

        if (abSqr < 1e-6f)
            return Vector3.Distance(p, a);

        float t = Vector3.Dot(p - a, ab) / abSqr;
        t = Mathf.Clamp01(t);

        Vector3 closest = a + ab * t;
        return Vector3.Distance(p, closest);
    }
}
