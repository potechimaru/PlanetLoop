using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineObstaclePlacer))]
[CanEditMultipleObjects]
public class SplineObstaclePlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Bake", EditorStyles.boldLabel);

        if (GUILayout.Button("Bake Obstacles"))
        {
            foreach (Object obj in targets)
            {
                var placer = (SplineObstaclePlacer)obj;
                placer.Bake(deactivateAfterBake: false);

                EditorUtility.SetDirty(placer);
            }
        }

        if (GUILayout.Button("Bake & Deactivate"))
        {
            foreach (Object obj in targets)
            {
                var placer = (SplineObstaclePlacer)obj;
                placer.Bake(deactivateAfterBake: true);

                EditorUtility.SetDirty(placer);
            }
        }

        if (GUILayout.Button("Clear Baked Obstacles"))
        {
            foreach (Object obj in targets)
            {
                var placer = (SplineObstaclePlacer)obj;
                placer.ClearImmediate();

                EditorUtility.SetDirty(placer);
            }
        }
    }
}