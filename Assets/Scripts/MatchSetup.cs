using UnityEngine;

public class MatchSetup : MonoBehaviour
{
    public mvst[] movesets;

    public PlayerMoveset[] PlayerObjects;

    public Material[] marisaSkins;
    public Material[] sakuyaSkins;
    //public Material keiyokeSkin; // just set to regular material on skin assignment
    // Keiyoke only has one skin

    private void Awake()
    {
        // do the setup herre
        for (int i = 0; i < PlayerObjects.Length; i++)
        {



            PlayerObjects[i].gameObject.SetActive(true);
            print("finished!");
        }
    }
}
