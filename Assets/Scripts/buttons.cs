using UnityEngine;
using UnityEngine.SceneManagement;

public class buttons : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            moveToScene(0);
        }
    }
    public void moveToScene(int input)
    {
        SceneManager.LoadScene(input);
    }
    public void kil()
    {
        Application.Quit();
    }
}
