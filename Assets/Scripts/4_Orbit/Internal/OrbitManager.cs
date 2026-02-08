using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbitManager
{
    private readonly IReadOnlyList<ClosedSplineLine> _lines;

    public OrbitManager(IEnumerable<ClosedSplineLine> lines)
    {
        _lines = lines.ToList();

        // デバッグ
        Debug.Log($"[LineManager] Injected ClosedSplineLine count = {_lines.Count()}");
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

            if (OrbitDistanceEvaluator.IsTouching(
                    line.CollisionSamples,
                    pos,
                    radius))
            {
                result = line;
                //Debug.Log("TryFindTouchedSpline : true");
                return true;
            }
        }

        result = null;
        return false;
    }

}
