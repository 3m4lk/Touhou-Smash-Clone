using UnityEngine;

[System.Serializable]
public class cntrls
{
    public string left;
    public string right;
    public string up;
    public string down;

    [Space]
    public string attack;
    public string shot;

    [Space]
    public string jump;
    public string shield;
    public string taunt;

    //[Space]
    //public Vector2 inputVector;
}
public class ControllerManager : MonoBehaviour
{
    public cntrls PlayerA;
    public cntrls PlayerB;
    public cntrls PlayerC;
    public cntrls PlayerD;
}
