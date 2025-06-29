using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMoveset))]
public class NameGeneratorButton : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerMoveset eTarget = (PlayerMoveset)target;
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
        PlayerStats eTarget = (PlayerStats)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("TestDealDmg"))
        {
            eTarget.dealDmg(eTarget.testDmg, eTarget.testKnockVec);
        }
    }
}
[CustomEditor(typeof(HitstunManager))]
public class HitstunManagerButton : Editor
{
    public override void OnInspectorGUI()
    {
        HitstunManager eTarget = (HitstunManager)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("TestHitstun"))
        {
            eTarget.startHitstun(eTarget.testHitstunTime);
        }
    }
}