using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class ControllerManager : MonoBehaviour
{
    public bool devMode;
    public GameObject devUI;
    public TMP_Text d_inputs;

    private float tillDevModeOff = 13.37f;
    private int devModeCount = 0;

    private void Awake()
    {
        if (false)
        {
            tillDevModeOff = 13.37f;
            devStart();
        }
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
        if (false)
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
            }

            tillDevModeOff = Mathf.Max(tillDevModeOff - Time.deltaTime, 0f);
        }
    }
    void devKeyCheck()
    {
        if (devMode)
        {
            d_inputs.text = null;
            for (int i = 0; i < 0; i++)
            {
                // pop texts in devmode based on Player inputs (like, if jump is held etc.)
                /*string inputColor = "<color=red>";
                if (Players[0].inputs[i].mode)
                {
                    inputColor = "<color=green>";
                }
                d_inputs.text += Players[0].inputs[i].name + ": " + inputColor + Players[0].inputs[i].mode + "</color>\n";

                if (Players[0].inputs[i].internalName == "mD" || Players[0].inputs[i].internalName == "mJ" || Players[0].inputs[i].internalName == "shield")
                {
                    d_inputs.text += "\n";
                }//*/
            }
        }
        //*/
    }
    /*private void OnApplicationFocus(bool focus) // not needed / not feasible
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
    }//*/
}
