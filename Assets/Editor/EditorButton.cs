using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMoveset))]
public class NameGeneratorButton : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerMoveset moveset = (PlayerMoveset)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Test"))
        {
        }
    }
}