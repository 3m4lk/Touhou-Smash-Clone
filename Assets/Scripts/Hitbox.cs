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

    private float attackCooldown;
    private void Awake()
    {
        if (GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().enabled = GameObject.Find("ControllerManager").GetComponent<ControllerManager>().devMode;
    }
    private void FixedUpdate()
    {
        attackCooldown = Mathf.Max(attackCooldown - Time.fixedDeltaTime, 0f);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hitAmount == 0) return;

        if (hitAmount != -1) hitAmount--;

        switch (hitboxType) // WOW!
        {
            case hbt.Hitbox:
                break; // self
            case hbt.Attack:
                if (attackCooldown != 0f) return;

                if (collision.GetComponent<Hitbox>().hitboxType == hbt.Attack) return; // if other is also an attack; add more ignored hitboxes perhaps?

                collision.attachedRigidbody.GetComponent<PlayerStats>().dealDmg(damage, kbVector());
                //attackCooldown = 0.01f * 4f; // 3x fixed time step
                attackCooldown = 0.1f;

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

    public void switchHitboxType(hbt input)
    {
        hitboxType = input;

        if (!GetComponent<SpriteRenderer>().enabled) return; // dev mode hitboxes

        string desMaterial = default;
        switch (input)
        {
            case hbt.Hitbox:
                desMaterial = "hitbox";
                break;
            case hbt.Attack:
                desMaterial = "attack";
                break;
            case hbt.Invincible:
                desMaterial = "invincible";
                break;
            case hbt.Shield:
                desMaterial = "shield";
                break;
            default:
                print("bruh");
                break;
        }
        GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/DEV/" + desMaterial);
        // change color (if dev)
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
