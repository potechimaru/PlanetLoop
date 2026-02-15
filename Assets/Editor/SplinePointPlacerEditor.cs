using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplinePointPlacer))]
public class SplinePointPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var placer = (SplinePointPlacer)target;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Bake", EditorStyles.boldLabel);

        if (GUILayout.Button("Bake Points"))
        {
            placer.Bake(deactivateAfterBake: false);
        }

        if (GUILayout.Button("Bake & Deactivate"))
        {
            placer.Bake(deactivateAfterBake: true);
        }

        if (GUILayout.Button("Clear Baked Points"))
        {
            placer.ClearImmediate();
        }
    }
}
