using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
public enum dir
{
    Up,
    Down,
    Left,
    Right,
    Horizontal,
    Vertical
}
/*[System.Serializable]
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
}//*/

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

    public Collider2D collision, crouchCollision;
    public PhysicsMaterial2D matGround, matAir;

    [Space(20)]
    // Desired
    public Vector2 desMovementVector;
    public bool desJump, isholdingJump;

    [Space]
    public bool isGrounded;

    public bool isWalking, isRunning, isShielding;

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

    public TMP_Text speedText;

    public bool runHeld, runReleased;

    private void Awake()
    {
        ownRb.gravityScale = moveset.gravityStrength;
        jumpsRemaining = moveset.jumpAmount;
        //vanityAnim("idle");
    }
    public void duckCollisions(bool mode)
    {
        collisions[0].SetActive(!mode);
        collisions[1].SetActive(mode);
    }

    private void FixedUpdate()
    {
        if (!ownStats.isAlive) return;

        groundCheck();

        sprintCooldown = Mathf.Max(sprintCooldown - Time.fixedDeltaTime, 0f);
        if (runReleased && sprintCooldown == 0) runReleased = false;
        sprintPushCooldown = Mathf.Max(sprintPushCooldown - Time.fixedDeltaTime, 0f);

        movement();

        if (isGrounded && desJump || jumpsRemaining > 0 && desJump)
        {
            changeDuckState(false);

            if (!isGrounded)
            {
                jumpsRemaining = Mathf.Max(jumpsRemaining - 1, 0); // just in case stupid shit happens
                //vanityAnim("jumpAir");
                playAnim("jumpAir");
            }
            else
            {
                playAnim("jump");
                //vanityAnim("jump");
            }
            //forceJump(moveset.jumpHeight);
        }
        desJump = false;

        speedText.text = ownRb.linearVelocityX + "";
    }
    void changeDuckState(bool input)
    {
        if (input) changeSprintMult();
        duckState = input;
        animManager.setBool("isDucking", input);
        duckCollisions(input);
    }

    void movement()
    {
        if (!ownStats.isAlive || ownStats.isStunned || ownStats.isKnocked)
        {
            desMovementVector = Vector2.zero;
            print("CALLING MOVEMENT");
            return;
        }

        if (animManager.isState("noAttack") || duckState) return;
        //if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack") && isGrounded || ownStats.shieldMode || ownStats.isKnocked && isGrounded || ownStats.isStunned) return;
        //vertVelo = ownRb.linearVelocityY;

        //ownRb.AddForce(desMovementVector * moveset.speed, ForceMode2D.)
        //Vector2 ogPos = ownRb.position;
        //ogPos.x += desMovementVector.x * moveset.speed * sprintMultiplier * Time.fixedDeltaTime;
        //ownRb.position = ogPos;

        //float horVelocity = ownRb.linearVelocityX;

        //float direToDesiredVelo = Mathf.Sign(horVelocity - (desMovementVector.x * moveset.speed));

        //print("movement");
        Vector2 horMovVec = desMovementVector;
        horMovVec.y = 0;

        float useSpeed = moveset.speed;
        if (!isGrounded && !ownStats.isKnocked) useSpeed *= 0.8f;

        //animManager.ownAnimator.SetBool("isWalking", desMovementVector.x > 0.01f);

        horVelo = ownRb.linearVelocityX;

        float knockMult = 1f;
        if (ownStats.isKnocked) knockMult = 0.5f;

        if (sprintStep && sprintPushCooldown == 0f)
        {
            sprintPushCooldown = 0.4f; // to prevent sprintspamming
            sprintStep = false;
            ownRb.linearVelocityX = desMovementVector.x * useSpeed * moveset.sprintStepMult;
            //print("<color=cyan>SPRINT INITIAL PUSH");
        }
        else if (Mathf.Abs(horVelo) < 0.1f && desMovementVector.x != 0)
        {
            ownRb.linearVelocityX = desMovementVector.x * useSpeed * moveset.startStepMult;
            print("<color=red>INITIAL PUSH; add some flair and effects");
        }
        else if (Mathf.Abs(horVelo) < useSpeed * sprintMultiplier)
        {
            ownRb.AddForce(horMovVec * useSpeed * knockMult, ForceMode2D.Impulse);
        }
    }
    void groundCheck()
    {
        bool oldGrounded = isGrounded;

        coyoteTime = Mathf.Max(coyoteTime - Time.fixedDeltaTime, 0f);

        isGrounded = Physics2D.BoxCast(gcPosition.position, gcScale, 0, Vector2.down, 0, groundMask);

        if (isGrounded != oldGrounded)
        {
            groundTap();
        }
    }
    void groundTap()
    {
        animManager.setBool("isGrounded", isGrounded);

        ownRb.sharedMaterial = matAir;

        //duckState = false;

        if (isGrounded)
        {
            isholdingJump = false;
            ownRb.sharedMaterial = matGround;

            // land on ground
            //print("Landed!");
            changeSprintMult();
            changeDuckState(false);
            //print("<color=blue>(b)" + desMovementVector.x);
            if (desMovementVector.x != 0) animManager.switchDirection(desMovementVector.x);
            jumpsRemaining = moveset.jumpAmount;

            // knock check
            if (desMovementVector.x > 0.01f)
            {
                //vanityAnim("walk");
            }
            else
            {
                //vanityAnim("idle");
            }

            if (GetComponent<AIController>())
            {
                float switchDireVal = -1;
                if (GetComponent<AIController>().enabled) switchDireVal = GetComponent<AIController>().inputAxis.x;
                // else switchDireVal = ownController.Players[0].inputAxes[0].value; // change l8r to ownController.Players[PlayerIndex].inputAxes[0].value

                animManager.switchDirection(switchDireVal, false, true);
            }
            //print("DIRE: " + ownController.Players[0].inputAxes[0].value);

            if (ownStats.isKnocked) ownStats.knockProgress = 1.5f; // knockState for 2 secs when knocked touches floor

            return;
        }
        // leave ground
        //print("left the ground");

        //vanityAnim("x_shieldRelease");

        //animManager.playAnimation("idleAir");
        changeDuckState(false);
        jumpsRemaining = Mathf.Max(jumpsRemaining - 1, 0);
    }

    void forceJump(float input)
    {
        if (animManager.isState("noAttack") || animManager.isState("specialFall")) return;
        //isGrounded = false;
        changeSprintMult();
        ownRb.linearVelocityY = input * 10f;
    }

    public void changeSprintMult(float input = 1f)
    {
        print("sprintMultChange");
        sprintMultiplier = input;

        isRunning = input != 1f;
        animManager.setBool("isRunning", isRunning);
        return;
    }

    void playAnim(string input)
    {
        animManager.setAnimation(input);
    }
    public void contrInput(InputAction.CallbackContext obj)
    {
        //print(obj.action.name);

        //print("device: " + obj.action.activeControl.device.displayName);
        bool mode = obj.action.triggered;

        //print("press: " + mode);

        if (ownStats.respawnOnInput) ownStats.respawn();

        if (!ownStats.isAlive || ownStats.isStunned)
        {
            animManager.setBool("isWalking", false);
            animManager.setBool("isRunning", false);
            return;
        }

        if (ownStats.isKnocked)
        {
            if (ownStats.knockProgress == 0f)
            {
                ownStats.bootOutOfKnock();
            }

            animManager.setBool("isWalking", false);
            animManager.setBool("isRunning", false);

            if (obj.action.name == "Move") desMovementVector = obj.action.ReadValue<Vector2>();
            if (ownRb.linearVelocityY > 0) return;
        }

        //print(obj.action.name + " || " + obj.action.triggered);
        //print(obj.action.name + ": " + obj.action.ReadValue<Vector2>());
        switch (obj.action.name)
        {
            default:
                print("WHAT; this should be impossible");
                break;
            case "Move":
                //pInputSides(mode);

                //desMovementVector = Vector2.ClampMagnitude(obj.action.ReadValue<Vector2>(), 1f);
                desMovementVector = obj.action.ReadValue<Vector2>();

                //print("> " + desMovementVector);

                isWalking = (Mathf.Abs(desMovementVector.x) >= 0.01f);
                animManager.setBool("isWalking", isWalking);

                //if (Mathf.Abs(desMovementVector.x) < 0.01f) return;

                //print(desMovementVector.x + " " + (Mathf.Abs(desMovementVector.x) > 0.9f)); // run

                runHeld = mode;

                if (mode)
                {
                    if (obj.action.activeControl.device.displayName != "Keyboard" && desMovementVector.magnitude > 0.99f && isDire(desMovementVector, dir.Up) && !isholdingJump) inpJump(mode);
                    else if (isGrounded)
                    {
                        if (obj.action.activeControl.device.displayName == "Keyboard")
                        {
                            //print("kb");
                            if (runHeld && runReleased && sprintCooldown != 0 && isDire(desMovementVector, dir.Horizontal))
                            {
                                //print("SPRINT");
                                changeSprintMult(moveset.sprintMult);
                                sprintStep = true;
                            }
                            else if (isDire(desMovementVector, dir.Horizontal)) sprintCooldown = 0.24f;
                        }
                        else if (sprintCooldown != 0 && Mathf.Abs(desMovementVector.x) >= 0.9f && isDire(desMovementVector, dir.Horizontal))
                        {
                            //print("SPRINT");
                            changeSprintMult(moveset.sprintMult);
                            sprintStep = true;
                        }
                    }
                }
                else
                {
                    if (obj.action.activeControl.device.displayName != "Keyboard")
                    {
                        sprintCooldown = 0.24f;
                        isholdingJump = false;
                    }

                    //if (runReleased) runReleased = false;
                    runReleased = true;
                    changeSprintMult();
                }

                bool duckMode = isDire(desMovementVector, dir.Down);
                if (isGrounded)
                {
                    //if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")) return;
                    changeDuckState(duckMode);

                    //if (duckMode) vanityAnim("duck");
                    //else vanityAnim("idle");

                    if (desMovementVector.x != 0 && Mathf.Sign(desMovementVector.x) != animManager.lookDire)
                    {
                        print("DIRESWITCH: " + desMovementVector.x);
                        animManager.switchDirection(Mathf.Sign(desMovementVector.x));
                    }
                }
                else if (duckMode && ownRb.linearVelocityY <= 0 && ownRb.linearVelocityY > fastFallVelo)
                {
                    //print("if above, set down velo to some amount; that fastfall thingy from smash");
                    ownRb.linearVelocityY = fastFallVelo;
                } // Fastfall

                // if timer != 0 && tapped, invoke sprint
                // else, set timer to a value

                //GameObject.Find("inputVec").GetComponent<TMP_Text>().text = obj.action.ReadValue<Vector2>() + "";
                break;
            case "Jump":
                inpJump(mode);
                break;
            case "Run":
                print("invoke run regardless of input strength (on true only)");
                break;
            case "Attack":
                inpSmash(mode, desMovementVector);
                break;
            case "Shot":
                changeDuckState(false);
                if (mode)
                {
                    //duckCollisions(false);
                    if (isDire(desMovementVector, dir.Up))
                    {
                        if (!animManager.isState("noAttack") && !animManager.isState("specialFall"))
                        {
                            //print("recovery");
                            animManager.switchDirection(desMovementVector.x, true);
                            //animManager.playAnimation("x_recovery");

                            playAnim("x_recovery");

                            //forceJump(default, moveset.recoveryForce); // the animation will handle it

                            //playAnim("x_recovery");

                            jumpsRemaining = 0;
                        } // idfc, this is a check if u can even do recovery
                        return;
                    }
                    //playAnim("x_shot");
                    //animManager.playAnimation("x_shot");

                    if (!animManager.isState("noAttack") && !animManager.isState("specialFall")) playAnim("x_shot");

                    //print("shot");
                }
                break;
            case "Direction Smash":
                inpSmash(mode, obj.action.ReadValue<Vector2>(), true);
                break;
            case "Shield":
                if (!animManager.isName("x_shield") && (animManager.isState("noAttack") || animManager.isState("specialFall"))) break;
                isShielding = false;
                if (isGrounded)
                {
                    isShielding = mode;
                    duckCollisions(false);
                    if (isShielding)
                    {
                        //if (animManager.ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")) return;

                        //print("shield");
                        //vanityAnim("x_shield");
                        playAnim("x_shield");
                        return;
                    }
                    else playAnim("idle");

                    //print("shieldRelease");
                    //shieldTime = 0.35f;
                    //vanityAnim("x_shieldRelease");
                }
                break;
            case "Taunt":
                if (animManager.isState("noAttack") || animManager.isState("specialFall")) break;
                if (mode && isGrounded)
                {
                    duckCollisions(false);
                    playAnim("taunt");

                    //print("taunt");
                    //animManager.playAnimation("taunt");
                    return;
                }
                break;
            case "Pause":
                break;
            case "devSkinSwitch":
                //print("devSkinSwitch");
                if (mode) GetComponent<PlayerSkinApplier>().skinSwitch();
                break;
        }
    }
    void inpJump(bool mode)
    {
        if (animManager.isState("noAttack") || animManager.isState("specialFall")) return;
        //duckCollisions(false);
        changeDuckState(false);
        desJump = mode;
        if (isholdingJump && !mode && ownRb.linearVelocityY > 0 && !animManager.isState("noAttack")) ownRb.linearVelocityY *= 0.6f;
        isholdingJump = mode;

        //Gamepad.current.SetMotorSpeeds(0f, 0f);
    }
    void inpSmash(bool mode, Vector2 dire, bool isDireSmash = false)
    {
        if (mode && (!isDireSmash || isDireSmash && dire.magnitude > 0.3f))
        {
            changeDuckState(false);

            string atkName = "n";

            if (isDire(dire, dir.Up))
            {
                atkName = "u";
            }
            else if (isDire(dire, dir.Down))
            {
                atkName = "d";
            }
            else if (isDire(dire, dir.Horizontal))
            {
                animManager.switchDirection(dire.x, true);
                atkName = "s";
            }
            // not adding "n" in else, it's already covered by default parameter

            atkName += "Smash";
            if (!isGrounded || desJump) atkName += "Air";

            //print(atkName);
            if (!animManager.isState("noAttack") && !animManager.isState("specialFall")) playAnim("x_" + atkName);
        }
    }
    public void manageInputs(string input)
    {
        print("later when you start working on ai stuff");
    }
    public void disconnected()
    {

    }
    public bool isDire(Vector2 input, dir direction)
    {
        input = input.normalized;
        switch (direction)
        {
            case dir.Up:
                return (Vector2.Dot(input, Vector2.up) >= 0.5f);
            case dir.Down:
                return (Vector2.Dot(input, Vector2.down) >= 0.5f);
            case dir.Left:
                return (Vector2.Dot(input, Vector2.left) > 0.5f);
            case dir.Right:
                return (Vector2.Dot(input, Vector2.right) > 0.5f);
            case dir.Horizontal:
                //print(Vector2.Dot(new Vector2(Mathf.Abs(input.x), input.y), Vector2.right));
                return (Vector2.Dot(new Vector2(Mathf.Abs(input.x), input.y), Vector2.right) > 0.71f);
            case dir.Vertical:
                return (Vector2.Dot(new Vector2(input.x, Mathf.Abs(input.y)), Vector2.up) > 0.71f);
        }
        return false;
    }
}