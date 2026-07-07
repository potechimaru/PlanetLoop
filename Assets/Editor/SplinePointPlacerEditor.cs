using UnityEditor;
using UnityEngine;

/// <summary>
/// 各SplineにPointObjectを自由に配置するためのエディタ拡張クラス
/// </summary>
[CustomEditor(typeof(SplinePointPlacer))]
[CanEditMultipleObjects]
public class SplinePointPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Bake", EditorStyles.boldLabel);

        if (GUILayout.Button("Bake Points"))
        {
            foreach (Object obj in targets)
            {
                var placer = (SplinePointPlacer)obj;
                placer.Bake(deactivateAfterBake: false);

                EditorUtility.SetDirty(placer);
            }
        }

        if (GUILayout.Button("Bake & Deactivate"))
        {
            foreach (Object obj in targets)
            {
                var placer = (SplinePointPlacer)obj;
                placer.Bake(deactivateAfterBake: true);

                EditorUtility.SetDirty(placer);
            }
        }

        if (GUILayout.Button("Clear Baked Points"))
        {
            foreach (Object obj in targets)
            {
                var placer = (SplinePointPlacer)obj;
                placer.ClearImmediate();

                EditorUtility.SetDirty(placer);
            }
        }
    }
}