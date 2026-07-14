using System;
using UnityEngine;

/// <summary>
/// Žg‚Á‚Ä‚È‚¢
/// </summary>
[Serializable]
public class ObstaclePlacementEntry
{
    public GameObject obstaclePrefab;

    [Min(0)]
    public int count = 1;
}