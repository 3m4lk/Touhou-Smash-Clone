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
    [Tooltip("idk what for but SURELY this could be useful for sum?")]
    public string fullName;
    public string name;
    public string ownColor = "FFFFFF";

    [Space]
    public string title;

    [Space]
    public int skin;

    public int jumpAmount;

    [Space]
    public float speed;
    public float jumpHeight;
    public float gravityStrength = 1f;

    public float sprintMult = 1f;
    public float startStepMult = 1f;
    public float sprintStepMult = 1f;

    public Vector2 recoveryForce;
}

public class PlayerMoveset : MonoBehaviour
{
    public mvst moveset;
    public ControllerManager ownController;
    public PlayerStats ownStats;

    public int PlayerIndex;

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

    public bool isWalking; // this one too
    public bool isRunning; // do it

    public LayerMask groundMask;
    public Vector2 gcScale;
    public Transform gcPosition;
    private float coyoteTime;

    public int jumpsRemaining = 1;

    public bool duckState;

    public float sprintCooldown;

    public float sprintMultiplier = 1f;

    public float horVelo;

    private bool sprintStep = false;

    public float fastFallVelo;

    [Header("0 - Regular;\n1 - Duck")]
    public GameObject[] collisions;
    public float vertVelo;

    // float damageDeal = Mathf.Lerp(m_jab.damageRange.x, m_jab.damageRange.y, Mathf.Min(staleCount, 4) / 4f);
    // staleCount++;

    private float sprintPushCooldown;

    private string lastVanityAnim;

    private bool culpritCheck;

    private void Awake()
    {
        ownRb.gravityScale = moveset.gravityStrength;
        jumpsRemaining = moveset.jumpAmount;
        vanityAnim("idle");
    }
    public void processInput(string input, bool mode)
    {
        if (ownStats.respawnOnInput) ownStats.respawn();

        if (!ownStats.isAlive || ownStats.isStunned) return;

        if (ownStats.isKnocked)
        {
            if (ownStats.knockProgress == 0f)
            {
                /*switch (input)
                {
                    case "mL":
                        return;
                    case "mR":
                        return;
                    case "mU":
                        return;
                    case "mD":
                        return;
                } // side directionals don't disable knock //*/ // yes they do, you've played brawl

                if (input == "mD") return; // except that one, this 1 stays

                ownStats.bootOutOfKnock();
            }
            return;
        }

        switch (input)
        {
            default:
                break;
            case "mL":
                pInputSides(mode);
                break;
            case "mR":
                pInputSides(mode);
                break;

            case "mJ":
                if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")) return;
                duckCollisions(false);
                desJump = mode;
                break; // Jump

            case "mD":
                if (isGrounded)
                {
                    if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")) return;

                    duckCollisions(mode);

                    duckState = mode;
                    if (mode)
                    {
                        //print("duck");
                        vanityAnim("duck");
                        return;
                    }
                    vanityAnim("idle");
                    return;
                }
                if (mode)
                {
                    if (ownRb.linearVelocityY <= fastFallVelo * (1f / 3f) && ownRb.linearVelocityY > fastFallVelo)
                    {
                        //print("if above, set down velo to some amount; that fastfall thingy from smash");
                        ownRb.linearVelocityY = fastFallVelo;
                    }
                }
                break;

            case "attack":
                if (mode)
                {
                    duckState = false;
                    duckCollisions(false);
                    if (isGrounded)
                    {
                        switch (desMovementVector.y)
                        {
                            case -1:
                                //print("dSmash");
                                animManager.playAnimation("x_downSmash");
                                break;
                            case 1:
                                //print("uSmash");
                                animManager.playAnimation("x_upSmash");
                                break;
                            case 0:
                                switch (desMovementVector.x)
                                {
                                    default:
                                        //print("sSmash");
                                        animManager.playAnimation("x_sideSmash");
                                        break;
                                    case 0:
                                        //print("jab");
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
                            //print("dAir");
                            animManager.playAnimation("x_dair");
                            break;
                        case 1:
                            //print("uAir");
                            animManager.playAnimation("x_uair");
                            break;
                        case 0:
                            switch (desMovementVector.x)
                            {
                                default:
                                    //print("sAir");
                                    animManager.switchDirection(desMovementVector.x, true);
                                    animManager.playAnimation("x_sair");
                                    break;
                                case 0:
                                    //print("nAir");
                                    animManager.playAnimation("x_nair");
                                    break;
                            }
                            break;
                    }
                }

                // looks abominable but idgaf just slide that shi in brah 🥀
                break; // Attack
            case "shot":
                duckState = false;
                if (mode)
                {
                    duckCollisions(false);
                    if (desMovementVector.y == 1)
                    {
                        if (!animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack"))
                        {
                            //print("recovery");
                            animManager.switchDirection(desMovementVector.x, true);
                            animManager.playAnimation("x_recovery");
                            forceJump(default, moveset.recoveryForce);
                            jumpsRemaining = 0;
                        } // idfc, this is a check if u can even do recovery
                        return;
                    }
                    animManager.playAnimation("x_shot");
                    //print("shot");
                }
                break;   // Shot
            case "shield":
                if (isGrounded)
                {
                    duckCollisions(false);
                    if (mode)
                    {
                        if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")) return;

                        //print("shield");
                        vanityAnim("x_shield");
                        return;
                    }
                    //print("shieldRelease");
                    //shieldTime = 0.35f;
                    vanityAnim("x_shieldRelease");
                }
                break; // Shield
            case "taunt":
                if (mode && isGrounded)
                {
                    duckCollisions(false);
                    //print("taunt");
                    //vanityAnim("taunt");
                    animManager.playAnimation("taunt");
                    return;
                }
                break;  // Taunt
        }
    }
    public void duckCollisions(bool mode)
    {
        collisions[0].SetActive(!mode);
        collisions[1].SetActive(mode);
    }
    public void processAxis(string input, float value)
    {
        switch (input)
        {
            default:
                break;
            case "horizontal":
                desMovementVector.x = 0;
                if (!duckState)
                {
                    desMovementVector.x = value;
                }
                break;
            case "vertical":
                desMovementVector.y = value;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!ownStats.isAlive) return;

        groundCheck();

        sprintCooldown = Mathf.Max(sprintCooldown - Time.fixedDeltaTime, 0f);
        sprintPushCooldown = Mathf.Max(sprintPushCooldown - Time.fixedDeltaTime, 0f);

        movement();

        if (isGrounded && desJump || jumpsRemaining > 0 && desJump)
        {
            if (!isGrounded)
            {
                jumpsRemaining = Mathf.Max(jumpsRemaining - 1, 0); // just in case stupid shit happens
                vanityAnim("jumpAir");
            }
            else
            {
                vanityAnim("jump");
            }
            forceJump(moveset.jumpHeight);
        }
        desJump = false;
    }

    void movement()
    {
        if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack") && isGrounded || ownStats.shieldMode || ownStats.isKnocked && isGrounded || ownStats.isStunned) return;
        vertVelo = ownRb.linearVelocityY;

        //ownRb.AddForce(desMovementVector * moveset.speed, ForceMode2D.)
        //Vector2 ogPos = ownRb.position;
        //ogPos.x += desMovementVector.x * moveset.speed * sprintMultiplier * Time.fixedDeltaTime;
        //ownRb.position = ogPos;

        //float horVelocity = ownRb.linearVelocityX;

        //float direToDesiredVelo = Mathf.Sign(horVelocity - (desMovementVector.x * moveset.speed));

        //print("movement");
        Vector2 horMovVec = desMovementVector;

        animManager.ownAnimator.SetBool("isWalking", desMovementVector.x > 0.01f);

        horVelo = ownRb.linearVelocityX;

        horMovVec.y = 0;

        float knockMult = 1f;
        if (ownStats.isKnocked) knockMult = 0.5f;

        if (sprintStep && sprintPushCooldown == 0f)
        {
            sprintPushCooldown = 0.4f; // to prevent sprintspamming
            sprintStep = false;
            ownRb.linearVelocityX = desMovementVector.x * moveset.speed * moveset.sprintStepMult;
            //print("<color=cyan>SPRINT INITIAL PUSH");
        }
        else if (Mathf.Abs(horVelo) < 0.1f && desMovementVector.x != 0)
        {
            ownRb.linearVelocityX = desMovementVector.x * moveset.speed * moveset.startStepMult;
            //print("<color=red>INITIAL PUSH");
        }
        else if (Mathf.Abs(horVelo) < moveset.speed * sprintMultiplier)
        {
            ownRb.AddForce(horMovVec * moveset.speed * knockMult, ForceMode2D.Impulse);
        }
    }
    void groundCheck()
    {
        bool oldGrounded = isGrounded;

        coyoteTime = Mathf.Max(coyoteTime - Time.fixedDeltaTime, 0f);

        isGrounded = Physics2D.BoxCast(gcPosition.position, gcScale, 0, Vector2.down, 0, groundMask);

        if (isGrounded != oldGrounded)
        {
            animManager.ownAnimator.SetBool("isGrounded", isGrounded);

            if (isGrounded)
            {
                // land on ground
                //print("Landed!");
                changeSprintMult();
                duckState = false;
                animManager.switchDirection(desMovementVector.x);
                jumpsRemaining = moveset.jumpAmount;

                // knock check
                if (desMovementVector.x > 0.01f)
                {
                    vanityAnim("walk");
                }
                else
                {
                    vanityAnim("idle");
                }

                float switchDireVal = -1;
                if (GetComponent<AIController>() && GetComponent<AIController>().enabled) switchDireVal = GetComponent<AIController>().inputAxis.x;
                else switchDireVal = ownController.Players[0].inputAxes[0].value; // change l8r to ownController.Players[PlayerIndex].inputAxes[0].value

                animManager.switchDirection(switchDireVal, false, true);
                //print("DIRE: " + ownController.Players[0].inputAxes[0].value);

                if (ownStats.isKnocked) ownStats.knockProgress = 1.5f; // knockState for 2 secs when knocked touches floor

                return;
            }
            // leave ground
            //print("left the ground");

            vanityAnim("x_shieldRelease");

            //animManager.playAnimation("idleAir");
            duckState = false;
            if (desJump)
            {
                vanityAnim("idle"); // it works for some reason... WHY AND HOW DOES IT WORK ????????
            }
            jumpsRemaining = Mathf.Max(jumpsRemaining - 1, 0);
        }
    }

    void forceJump(float input = default, Vector2 inputVec = default)
    {
        if (!isGrounded)
        {
            changeSprintMult();
        }

        if (inputVec != default)
        {
            ownRb.linearVelocityX += inputVec.x * 10f * animManager.lookDire;
            ownRb.linearVelocityY = inputVec.y * 10f;
            return;
        }
        ownRb.linearVelocityY = input * 10f;
    }

    void pInputSides(bool mode)
    {
        if (!isGrounded) return;

        /*if (ownController.Players[0].inputAxes[0].value != 0)
        {
            vanityAnim("idle");
            return;
        }//*/

        changeSprintMult();
        if (mode)
        {
            //animManager.playAnimation("walk");
            if (sprintCooldown > 0f)
            {
                //print("sprint");
                changeSprintMult(moveset.sprintMult);
                sprintStep = true;

                // apply sprint speed
                vanityAnim("run");
            }
            else
            {
                //print("walk");
                vanityAnim("walk");
            }

            sprintCooldown = 0.24f;
            // successful direction switch resets cooldown
            return;
        }
        vanityAnim("idle");
    }
    void changeSprintMult(float input = default)
    {
        if (input == default)
        {
            animManager.ownAnimator.SetBool("isRunning", false);
            sprintMultiplier = 1f;
            return;
        } // unsprint
        animManager.ownAnimator.SetBool("isRunning", true);
        sprintMultiplier = input;
    }

    public void vanityAnim(string input)
    {
        if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack") && input != "x_shield" && input != "x_shieldRelease") return;
        string outputAnim = default;

        duckCollisions(false);

        ownStats.shieldMode = !true; // :)
        ownStats.ownHitbox.switchHitboxType(hbt.Hitbox);

        // idle
        // walk
        // run
        // jump
        // jumpAir
        //// taunt
        // shield

        //print("vanity: " + input);

        if (!isGrounded && input != "jump" && input != "jumpAir")
        {
            //print("did the thing: " + input);
            outputAnim = "idleAir";
            //if (ownStats.isKnocked) outputAnim = "airKnocked";
        }
        else if (!ownStats.isKnocked)
        {
            outputAnim = input; // replaced the default thing in switch

            switch (input)
            {
                case "duck":
                    outputAnim = input;
                    duckCollisions(true);
                    break;
                case "x_shield":
                    if (!duckState)// && ownStats.unshieldCooldown == 0f)
                    {
                        outputAnim = input;
                        ownStats.toggleShield(true);
                    }
                    break;
                case "x_shieldRelease":
                    /*if (!culpritCheck)
                    {
                        /*if (ownStats.unshieldCooldown > 0f)
                        {
                            outputAnim = "x_shield";
                            break;
                        }//* /

                        outputAnim = "idle";
                    }//*/
                    outputAnim = "idle";
                    ownStats.toggleShield(false);
                    break;
            }

            if (duckState)
            {
                outputAnim = "duck";
                duckCollisions(true);
            }
        }
        else
        {
            outputAnim = "airKnocked";
            if (isGrounded)
            {
                outputAnim = "knockedIdle";
            }
        }

        //if (culpritCheck) print("<color=red>GOTCHA BITCH: " + input);
        culpritCheck = false;

        if (ownStats.isStunned)
        {
            outputAnim = "airKnocked";
            if (isGrounded)
            {
                outputAnim = "confused";
            }
        }

        if (lastVanityAnim != outputAnim)
        {
            lastVanityAnim = outputAnim;

            if (outputAnim == "jump") culpritCheck = true;

            if (input == "x_shieldRelease" && !ownStats.isKnocked)
            {
                animManager.playAnimation(outputAnim, true);
                return;
            }

            animManager.playAnimation(outputAnim);
        }
    }
}