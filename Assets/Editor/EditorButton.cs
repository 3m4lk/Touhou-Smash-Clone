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
[CustomEditor(typeof(PlayerStats))]
public class PlayerStatsButton : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerStats stats = (PlayerStats)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("TestDealDmg"))
        {
            stats.dealDmg(stats.testDmg, stats.testKnockVec);
        }
    }
}