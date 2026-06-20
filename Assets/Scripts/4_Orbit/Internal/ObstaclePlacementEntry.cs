using System;
using UnityEngine;

[Serializable]
public class ObstaclePlacementEntry
{
    public GameObject obstaclePrefab;

    [Min(0)]
    public int count = 1;
}