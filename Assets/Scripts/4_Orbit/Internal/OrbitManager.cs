using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbitManager
{
    private readonly IEnumerable<ClosedSplineLine> _lines;

    public OrbitManager(IEnumerable<ClosedSplineLine> lines)
    {
        _lines = lines;

        // デバッグ
        Debug.Log($"[LineManager] Injected ClosedSplineLine count = {_lines.Count()}");
    }

}
