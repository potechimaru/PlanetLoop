using System;
using UnityEngine;

/// <summary>
/// SplineにPointを配置するための設定を表すクラス。Pointの種類と配置数を指定することができる。
/// </summary>
[Serializable]
public class PointPlacementEntry
{
    public PointObjectType pointType;

    [Min(0)]
    public int count = 1;
}