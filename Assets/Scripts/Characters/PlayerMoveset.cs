using UnityEngine;
[System.Serializable]
public class move
{
    //public string name; // for int reference only

    [Tooltip("x - regular;\ny - stale (after 4 or more of the same attack)")]
    public Vector2 damageRange;

    public float knockback;

    [Space]
    [Header("if true, unleash attack after charging ends (or gets interrupted by Player) with a multiplier")]
    public bool isChargeable;
    public float chargeTime;
    [HideInInspector] public float chargeProgress;
    [Tooltip("")]
    public Vector2 chargeMultRange;
    [Tooltip("should attack be unleashed when charge progress finishes?")]
    public bool autoAttack;

    [Space]
    [Header("if true, apply velocity ")]
    public bool changeVelocity;
    public Vector2 appliedVelocity;

    //[Space]
    //[Header("how much knockback cancels the attack")]
    //public float knockbackAmountToCancel;
}
[System.Serializable]
public class mvst
{
    public string name;

    [Space]
    public move m_jab;
    public move m_upSmash;
    public move m_horSmash;
    public move m_downSmash;

    [Space]
    [Header("SCRAP / JUST DUPE REGULAR ATTACKS IF NOT ENOUGH TIME")]
    public move m_air;
    public move m_upAir;
    public move m_horAir;
    public move m_downAir;

    [Space]
    public move m_shot;
    public move m_recovery;

    [Space]
    public float speed;
    public float jumpHeight;
}

public class PlayerMoveset : MonoBehaviour
{
    public mvst moveset;

    private string lastMove; // for stale
    private int staleCount;

    // float damageDeal = Mathf.Lerp(m_jab.damageRange.x, m_jab.damageRange.y, Mathf.Min(staleCount, 4) / 4f);
    // staleCount++;
}
