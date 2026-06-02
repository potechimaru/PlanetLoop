using System;
using UnityEngine;

[Serializable]
public class PointPlacementEntry
{
    public PointObjectType pointType;

    [Min(0)]
    public int count = 1;
}