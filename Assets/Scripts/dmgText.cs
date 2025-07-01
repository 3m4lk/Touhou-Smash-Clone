using TMPro;
using UnityEngine;

public class dmgText : MonoBehaviour
{
    public TMP_Text ownText;

    public AnimationCurve pokeCurve;
    public float pokeTime;
    private float pokeProgress;
    void Update()
    {
        if (pokeProgress != 0f) ownText.transform.localScale = Vector3.one * pokeCurve.Evaluate(pokeProgress / pokeTime);
        pokeProgress = Mathf.Max(pokeProgress - Time.deltaTime, 0f);
    }
    public void damagePoke(float strength)
    {
        float mult = Mathf.Clamp(strength / 100f, 0f, 1f);

        pokeProgress = pokeTime * mult;
        ownText.transform.localScale = Vector3.one * pokeCurve.Evaluate(0);
    }
}
