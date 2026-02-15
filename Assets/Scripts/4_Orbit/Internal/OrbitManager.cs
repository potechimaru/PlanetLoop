using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbitManager
{
    private readonly IReadOnlyList<ClosedSplineLine> _lines;

    public OrbitManager(IEnumerable<ClosedSplineLine> lines)
    {
        _lines = lines.ToList();
    }

    public bool TryFindTouchedSpline(
        Vector3 pos,
        float radius,
        ClosedSplineLine exclude,
        out ClosedSplineLine result)
    {
        foreach (var line in _lines)
        {
            if (line == exclude)
                continue;

            if (IsTouchingSpline(line, pos, radius))
            {
                result = line;
                return true;
            }
        }

        result = null;
        return false;
    }

    private bool IsTouchingSpline(
        ClosedSplineLine spline,
        Vector3 worldPos,
        float radius)
    {
        float totalLen = spline.GetTotalLength();

        // サンプリング分割数（精度調整可能）
        const int sampleCount = 128;

        float step = totalLen / sampleCount;

        for (int i = 0; i < sampleCount; i++)
        {
            float d = step * i;
            Vector3 p = spline.EvaluateByDistance(d);

            if ((p - worldPos).sqrMagnitude <= radius * radius)
                return true;
        }

        return false;
    }
}
