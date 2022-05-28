using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (WorldMapGenerator))]
public class WorldMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldMapGenerator mapgen = (WorldMapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapgen.autoUpdate)
            {
                mapgen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapgen.GenerateMap();
        }
    }
}
