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

    public int jumpAmount;

    /*
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

    //*/

    [Space]
    public float speed;
    public float jumpHeight;
    public float gravityStrength = 1f;

    public Vector2 recoveryForce;
}

public class PlayerMoveset : MonoBehaviour
{
    public mvst moveset;

    private string lastMove; // for stale
    private int staleCount;

    public Rigidbody2D ownRb;
    public PlayerAnimationManager animManager;

    public BoxCollider2D collision;
    public BoxCollider2D crouchCollision;

    [Space(20)]
    // Desired
    public Vector2 desMovementVector;
    private bool desJump;

    [Space]
    public bool isGrounded;
    public LayerMask groundMask;
    public Vector2 gcScale;
    public Transform gcPosition;
    private float coyoteTime;

    public int jumpsRemaining = 1;

    // float damageDeal = Mathf.Lerp(m_jab.damageRange.x, m_jab.damageRange.y, Mathf.Min(staleCount, 4) / 4f);
    // staleCount++;

    private void Awake()
    {
        ownRb.gravityScale = moveset.gravityStrength;
        jumpsRemaining = moveset.jumpAmount;
    }
    public void processInput(string input, bool mode)
    {
        switch (input)
        {
            default:
                break;
            case "mJ":
                desJump = mode;
                break; // Jump

            case "attack":
                if (mode)
                {
                    if (isGrounded)
                    {
                        switch (desMovementVector.y)
                        {
                            case -1:
                                print("dSmash");
                                animManager.playAnimation("x_downSmash");
                                break;
                            case 1:
                                print("uSmash");
                                animManager.playAnimation("x_upSmash");
                                break;
                            case 0:
                                switch (desMovementVector.x)
                                {
                                    default:
                                        print("sSmash");
                                        animManager.playAnimation("x_sideSmash");
                                        break;
                                    case 0:
                                        print("jab");
                                        animManager.playAnimation("x_jab");
                                        break;
                                }
                                break;
                        }
                        return;
                    } // grounded
                    //print("air");

                    switch (desMovementVector.y)
                    {
                        case -1:
                            print("dAir");
                            animManager.playAnimation("x_dair");
                            break;
                        case 1:
                            print("uAir");
                            animManager.playAnimation("x_uair");
                            break;
                        case 0:
                            switch (desMovementVector.x)
                            {
                                default:
                                    print("sAir");
                                    animManager.playAnimation("x_sair");
                                    break;
                                case 0:
                                    print("nAir");
                                    animManager.playAnimation("x_nair");
                                    break;
                            }
                            break;
                    }
                }

                // looks abominable but idgaf just slide that shi in brah 🥀
                break; // Attack
            case "shot":
                if (mode)
                {
                    if (desMovementVector.y == 1)
                    {
                        if (!animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack"))
                        {
                            print("recovery");
                            animManager.playAnimation("x_recovery");
                            forceJump(default, moveset.recoveryForce);
                        } // idfc, this is a check if u can even do recovery
                        return;
                    }
                    animManager.playAnimation("x_shot");
                    print("shot");
                }
                break;   // Shot
            case "shield":
                if (mode)
                {
                    print("shield");
                    animManager.playAnimation("x_shield");
                }
                break; // Shield
            case "taunt":
                if (mode)
                {
                    print("taunt");
                    animManager.playAnimation("taunt");
                }
                break;  // Taunt
        }
    }
    public void processAxis(string input, float value)
    {
        switch (input)
        {
            default:
                break;
            case "horizontal":
                desMovementVector.x = value;
                break;
            case "vertical":
                desMovementVector.y = value;
                break;
        }
    }

    private void FixedUpdate()
    {
        groundCheck();
        //ownRb.AddForce(desMovementVector * moveset.speed, ForceMode2D.)
        Vector2 ogPos = ownRb.position;
        ogPos.x += desMovementVector.x * moveset.speed * Time.fixedDeltaTime;
        ownRb.position = ogPos;

        if (isGrounded)
        {
            if (desMovementVector.x != 0)
            {
                animManager.playAnimation("walk");
            }
            else
            {
                //animManager.playAnimation("idle");
            }
        }

        if (isGrounded && desJump || jumpsRemaining > 0 && desJump)
        {
            if (!isGrounded)
            {
                jumpsRemaining = Mathf.Max(jumpsRemaining - 1, 0); // just in case stupid shit happens
                // play aerial jump animation
                animManager.playAnimation("jumpAir");
            }
            else
            {
                animManager.playAnimation("jump");
            }
            forceJump(moveset.jumpHeight);
        }
        desJump = false;
    }
    void groundCheck()
    {
        bool oldGrounded = isGrounded;

        coyoteTime = Mathf.Max(coyoteTime - Time.fixedDeltaTime, 0f);

        /*if (Physics2D.BoxCast(gcPosition.position, gcScale, 0, Vector2.down, 0, groundMask))
        {
            //coyoteTime = 0.1f; // deathbomb reference ?????????????????????
            coyoteTime = 0;
        }//*/

        isGrounded = Physics2D.BoxCast(gcPosition.position, gcScale, 0, Vector2.down, 0, groundMask);

        //isGrounded = (coyoteTime != 0f);

        if (isGrounded != oldGrounded)
        {
            if (isGrounded)
            {
                // land on ground
                print("Landed!");
                jumpsRemaining = moveset.jumpAmount;
                animManager.playAnimation("idle", true);
                return;
            }
            // leave ground
            jumpsRemaining = Mathf.Max(jumpsRemaining - 1, 0);
            print("left the ground");
            if (animManager.currentAnimation != "jump")
            {
                animManager.playAnimation("idleAir");
            }
        }
    }

    void forceJump(float input = default, Vector2 inputVec = default)
    {
        if (inputVec != default)
        {
            ownRb.linearVelocityX += inputVec.x * 10f * animManager.lookDire;
            ownRb.linearVelocityY = inputVec.y * 10f;
            return;
        }
        ownRb.linearVelocityY = input * 10f;
    }
}
