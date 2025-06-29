using UnityEngine;

public class HitstunManager : MonoBehaviour
{
    public float hitstunProgress;
    public float hitstunSpeed;

    [Space]
    public float testHitstunTime;
    private void Update()
    {
        if (hitstunProgress != -1)
        {
            hitstunProgress = Mathf.Max(hitstunProgress - Time.unscaledDeltaTime, 0f);
        }

        if (hitstunProgress == 0)
        {
            hitstunProgress = -1;
            //print("get time back to normal");
            Time.timeScale = 1f;
        }
    }
    public void startHitstun(float input)
    {
        if (input < hitstunProgress || hitstunProgress == -1)
        {
            hitstunProgress = input;
            Time.timeScale = hitstunSpeed;
        }
    }
}
