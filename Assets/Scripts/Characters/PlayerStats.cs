using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public PlayerMoveset moveset;

    public float damage;
    public TMP_Text damageText;

    public float maxKnockStrength;

    [Space]
    public float testDmg;
    public Vector2 testKnockVec;

    /*[Space]
    [Header("")]
    public Vector2 kbMagnitudeRange; //*/



    public void dealDmg(float amount, Vector2 knockVec) // set up kb later
    {
        damage = Mathf.Clamp(damage + amount, 0f, 999.9f);
        //damage = ((int)(damage * 10f)) / 10f;
        damageText.text = moveset.moveset.name + "\n<size=64><color=#FFFFFF><b>" + damage.ToString("0.0") + "%";

        /*if (knockVec.magnitude > distInDesDire(moveset.ownRb.linearVelocity, knockVec))
        {
            moveset.ownRb.linearVelocity = testKnockVec * 10f * Mathf.Lerp(1f, maxKnockStrength, damage / 999.9f);
        } // change velocity if hit real hard
        else
        {
            moveset.ownRb.AddForce(testKnockVec * 10f * Mathf.Lerp(1f, maxKnockStrength, damage / 999.9f), ForceMode2D.Impulse);
        } // add to velocity else//*/ // until u fix it its gon be shitted upon

        moveset.ownRb.AddForce(testKnockVec * 10f * Mathf.Lerp(1f, maxKnockStrength, damage / 999.9f), ForceMode2D.Impulse);

        print("do a hitstun if char is knocked with like, a shitton of kb");
    }

    float distInDesDire(Vector2 input, Vector2 desiredDirection)
    {
        return input.magnitude * Vector2.Dot(input.normalized, desiredDirection.normalized);
    }
}
