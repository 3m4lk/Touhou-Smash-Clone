using UnityEngine;

public enum hbt
{
    Hitbox, // where Player / Genjii takes damage; used for Player hitboxes
    Attack, // what deals damage; used for attacks of all kinds
    Invincible, // can be hit by attacks, but can't take damage nor knockback; used shortly after respawning, in final smashes or after grabbing Full Power (?)
    //Intangible, // (unused) can't be hit by attacks; used for dodges
    Shield // Absorbs attacks; used for shield
}

public class Hitbox : MonoBehaviour
{
    [Tooltip("Hitbox - where Player / Genjii takes damage; used for Player hitboxes\r\nAttack - what deals damage; used for attacks of all kinds\r\nInvincible - can be hit by attacks, but can't take damage nor knockback; used shortly after respawning, in final smashes or after grabbing Full Power (?)\r\n[UNUSED] Intangible - can't be hit by attacks; used for dodges\r\nShield - Absorbs attacks; used for shield")]
    public hbt hitboxType;

    public float direction = 1f;
    public float angle;
    public float damage;
    public float knockback;

    [Tooltip("set to -1 for infinite hits")]
    public int hitAmount;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitAmount == 0) return;

        if (hitAmount != -1) hitAmount--;

        switch (hitboxType) // WOW!
        {
            case hbt.Hitbox:
                break; // self
            case hbt.Attack:
                if (collision.GetComponent<Hitbox>().hitboxType == hbt.Attack) return;

                collision.attachedRigidbody.GetComponent<PlayerStats>().dealDmg(damage, kbVector());

                break; // self
            case hbt.Invincible:
                break; // self
            case hbt.Shield:
                break; // self
            default:
                break;
        } // own hitbox type

        // print("gerfasb");

        //print(collision.attachedRigidbody.name); // other object
    }
    Vector2 kbVector()
    {
        Vector2 outVec = angleToVector(angle);
        outVec.x *= Mathf.Sign(direction);
        return outVec * knockback;
    }
    Vector2 angleToVector(float input)
    {
        float sinOut = Mathf.Sin((float)Mathf.Repeat(input / 360f * (Mathf.PI * 2f), Mathf.PI * 2f));
        float cosOut = Mathf.Cos((float)Mathf.Repeat(input / 360f * (Mathf.PI * 2f), Mathf.PI * 2f));

        //float ang = input / Mathf.PI * 4f;
        return new Vector2(sinOut, cosOut);
    }
}
