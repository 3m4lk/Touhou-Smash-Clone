using UnityEngine;
using TMPro;

[System.Serializable]
public class axs
{
    [Header("For internal reference only")]
    public string name;
    public int buttonPositive;
    public int buttonNegative;
    public int value;
}
[System.Serializable]
public class inpt
{
    public string name;
    public string internalName;
    public string[] keys;
    public bool mode;
}
[System.Serializable]
public class cntrls
{
    [Header("For internal reference only")]
    public string name;
    public PlayerMoveset Character;
    public PlayerAnimationManager animation;

    [Space]
    public inpt[] inputs;

    [Space]
    public axs[] inputAxes;
}
public class ControllerManager : MonoBehaviour
{
    public cntrls[] Players;

    [Space]
    public bool devMode;
    public GameObject devUI;
    public TMP_Text d_inputs;

    private float tillDevModeOff = 13.37f;
    private int devModeCount = 0;

    private void Awake()
    {
        tillDevModeOff = 13.37f;
        devStart();
    }
    void devStart()
    {
        devUI.gameObject.SetActive(devMode);
        toggleVisuals(devMode);
        devKeyCheck();
    }
    void toggleVisuals(bool input)
    {
        GameObject[] allVisuals = GameObject.FindGameObjectsWithTag("hitboxVis");
        for (int i = 0; i < allVisuals.Length; i++)
        {
            allVisuals[i].GetComponent<SpriteRenderer>().enabled = input;
        }
    }
    private void Update()
    {
        if (!devMode)
        {
            if (tillDevModeOff > 0f)
            {
                if (Input.GetKeyDown(devModeCount + ""))
                {
                    if (devModeCount == 9)
                    {
                        devMode = true;
                        devStart();
                        tillDevModeOff = 0;
                        toggleVisuals(true);
                        print("<color=magenta>WELCOME TO DEV MODE!");
                    }
                    devModeCount++;
                }
            }
            else
            {
                if (Input.GetKeyDown(devModeCount + ""))
                {
                    if (devModeCount == 9)
                    {
                        devMode = true;
                        devStart();
                        tillDevModeOff = 0;
                        toggleVisuals(true);
                        print("<color=magenta>WELCOME TO DEV MODE!");

                    }
                    devModeCount++;
                }
            } // remove in final
        }

        tillDevModeOff = Mathf.Max(tillDevModeOff - Time.deltaTime, 0f);

        for (int i = 0; i < Players.Length; i++)
        {
            for (int a = 0; a < Players[i].inputs.Length; a++)
            {
                inpt currInput = Players[i].inputs[a];
                for (int b = 0; b < currInput.keys.Length; b++)
                {
                    if (Input.GetKeyDown(currInput.keys[b]) && !Players[i].inputs[a].mode)
                    {
                        Players[i].inputs[a].mode = true;
                        Players[i].Character.processInput(currInput.internalName, true);
                        devKeyCheck();
                    }
                    else if (Input.GetKeyUp(currInput.keys[b]) && Players[i].inputs[a].mode)
                    {
                        Players[i].inputs[a].mode = false;
                        Players[i].Character.processInput(currInput.internalName, false);
                        devKeyCheck();
                    }
                }
            }
            for (int a = 0; a < Players[0].inputAxes.Length; a++)
            {
                Players[i].Character.processAxis(Players[0].inputAxes[a].name, Players[0].inputAxes[a].value);
            }
        }
    }
    void devKeyCheck()
    {
        axisValueUpdate();
        if (devMode)
        {
            d_inputs.text = null;
            for (int i = 0; i < Players[0].inputs.Length; i++)
            {
                string inputColor = "<color=red>";
                if (Players[0].inputs[i].mode)
                {
                    inputColor = "<color=green>";
                }
                d_inputs.text += Players[0].inputs[i].name + ": " + inputColor + Players[0].inputs[i].mode + "</color>\n";

                if (Players[0].inputs[i].internalName == "mD" || Players[0].inputs[i].internalName == "mJ" || Players[0].inputs[i].internalName == "shield")
                {
                    d_inputs.text += "\n";
                }
            }
        }
    }
    public void axisValueUpdate()
    {
        for (int i = 0; i < Players[0].inputAxes.Length; i++)
        {
            float oldAxVal = Players[0].inputAxes[i].value;
            bool posInp = false;
            bool negInp = false;

            for (int a = 0; a < Players[0].inputs[Players[0].inputAxes[i].buttonPositive].keys.Length; a++)
            {
                if (Input.GetKey(Players[0].inputs[Players[0].inputAxes[i].buttonPositive].keys[a]))
                {
                    posInp = true;
                    break;
                }
            }

            for (int a = 0; a < Players[0].inputs[Players[0].inputAxes[i].buttonNegative].keys.Length; a++)
            {
                if (Input.GetKey(Players[0].inputs[Players[0].inputAxes[i].buttonNegative].keys[a]))
                {
                    negInp = true;
                    break;
                }
            }

            if (posInp == negInp)
            {
                Players[0].inputAxes[i].value = 0;
            }
            else
            {
                if (posInp) Players[0].inputAxes[i].value = 1;
                else Players[0].inputAxes[i].value = -1;

                if (Players[0].inputAxes[i].name == "horizontal" && Players[0].inputAxes[i].value != oldAxVal)
                {
                    //print("direChange");
                    Players[0].animation.switchDirection(Players[0].inputAxes[i].value);
                } // on movement direction change
            }
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        //print("LOCK TF IN: " + focus);
        if (!focus)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                for (int a = 0; a < Players[i].inputs.Length; a++)
                {
                    Players[i].inputs[a].mode = false;
                    Players[i].Character.processInput(Players[i].inputs[a].internalName, false);
                }
            }
            devKeyCheck();
            //if (devMode) { print("Lost Focus!"); }
        } // kick off all inputs if lost focus
    }
}
