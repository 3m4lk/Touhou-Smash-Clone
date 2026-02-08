using UnityEngine;
using UnityEngine.InputSystem;

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
    public float jumpGravityMult = 1.6f, gravityStrength = 1f;

    [Header("different jump heights")]
    public float jumpLow;
    public float jumpHigh, jumpAir;

    [Space]
    public float sprintMult = 1f;
    public float startStepMult = 1f;
    public float sprintStepMult = 1f;

    public Vector2 recoveryForce;
}
public class NewPlayerController : MonoBehaviour
{
    public mvst moveset;

    public Rigidbody2D ownRb;

    bool isJumping; // when pressed jump set to true, false when input released, otherwise finished or jump is interrupted
    [SerializeField] bool isGrounded;
    int jumpAmount;

    Vector2 desMovementVector;
    bool jumpHeld;
    [SerializeField] float jumpAnimTime;
    float jumpProgress, facingDirection;

    float coyoteProgress;
    [SerializeField] float coyoteTime;

    [SerializeField] Transform gcPosition, modelVis;
    [SerializeField] Vector2 gcScale;
    [SerializeField] LayerMask groundMask;
    [SerializeField] PhysicsMaterial2D groundMat, airMat;

    [SerializeField] Animator ownAnimator;

    private void Awake()
    {
        changeGravity(moveset.gravityStrength);
        jumpProgress = -1;
        ownRb.sharedMaterial = groundMat;
        facingDirection = 1;
    }
    private void FixedUpdate()
    {
        movement();

        groundCheck();

        if (jumpProgress != -1)
        {
            jumpProgress = Mathf.Max(jumpProgress - Time.fixedDeltaTime, 0f);
            if (jumpProgress == 0)
            {
                jumpProgress = -1;

                float jumpStrength = moveset.jumpLow;
                if (!isGrounded) jumpStrength = moveset.jumpAir;
                else if (jumpHeld) jumpStrength = moveset.jumpHigh;
                doJump(jumpStrength);
            }
        }

        if (isJumping && ownRb.linearVelocityY <= 0)
        {
            isJumping = false;
            changeGravity(moveset.gravityStrength);
        }
    }
    void movement()
    {
        if (Mathf.Abs(ownRb.linearVelocityX) < moveset.speed)
        {
            //ownRb.AddForceX(desMovementVector.x * moveset.speed, ForceMode2D.Force);
            //float targetSpeed = desMovementVector.x * moveset.speed;
            //ownRb.AddForceX(targetSpeed * 4f, ForceMode2D.Impulse);
            //ownRb.linearVelocityX = Mathf.Clamp(ownRb.linearVelocityX, -Mathf.Abs(targetSpeed), Mathf.Abs(targetSpeed));

            if (isGrounded)
            {
                ownRb.linearVelocityX = desMovementVector.x * moveset.speed;
                //ownRb.linearVelocityX = Mathf.MoveTowards(ownRb.linearVelocityX, desMovementVector.x * moveset.speed, 1f);
            }
            else
            {
                ownRb.linearVelocityX = Mathf.MoveTowards(ownRb.linearVelocityX, desMovementVector.x * moveset.speed, moveset.speed * 0.1f);
            }
        }
    }
    void doJump(float height)
    {
        //print("jump height: " + height);
        isJumping = true;
        changeGravity(moveset.jumpGravityMult);
        ownRb.linearVelocityY = Mathf.Sqrt(height * -2f * (Physics2D.gravity.y * ownRb.gravityScale));
        isGrounded = false;
        ownRb.sharedMaterial = airMat;
        coyoteProgress = 0;
    }
    void changeGravity(float input)
    {
        ownRb.gravityScale = input; // can make alterations here for Metal
        // when interrupted, change to x12 gravity scale regardless of what moveset.gravityStrength is
    }
    void groundCheck()
    {
        bool oldGrounded = isGrounded;

        coyoteProgress = Mathf.Max(coyoteProgress - Time.fixedDeltaTime, 0f);

        isGrounded = coyoteProgress != 0;

        //isGrounded = (Physics2D.BoxCast(gcPosition.position, gcScale, 0, Vector2.down, 0, groundMask) || coyoteProgress != 0);

        if (Physics2D.BoxCast(gcPosition.position, gcScale, 0, Vector2.down, 0, groundMask))
        {
            //print(Physics2D.BoxCast(gcPosition.position, gcScale, 0, Vector2.down, 0, groundMask).collider.name);
            coyoteProgress = coyoteTime;
        }

        if (isGrounded != oldGrounded)
        {
            groundTap();
        }
    }
    void groundTap()
    {
        // isGrounded = new grounded state
        if (isGrounded)
        {
            ownRb.sharedMaterial = groundMat;
            jumpAmount = moveset.jumpAmount;
            if (desMovementVector.x != 0 && Mathf.Sign(desMovementVector.x) != facingDirection) switchDirection(Mathf.Sign(desMovementVector.x));
        }
        else
        {
            ownRb.sharedMaterial = airMat;
            jumpAmount--;
        }
    }
    void switchDirection(float input)
    {
        facingDirection = input;
        modelVis.localScale = new Vector3(1, 1, facingDirection);
        ownAnimator.Play("turnAround");
    }
    public void contrInput(InputAction.CallbackContext obj)
    {
        // print(obj.action.name);
        // obj.action.activeControl.device.displayName != "Keyboard"

        //print("device: " + obj.action.activeControl.device.displayName);
        bool mode = obj.action.triggered;

        switch (obj.action.name)
        {
            default:
                print("WHAT; this should be impossible");
                break;
            case "Move":
                desMovementVector = obj.action.ReadValue<Vector2>();
                if (isGrounded && mode && Mathf.Sign(desMovementVector.x) != facingDirection) switchDirection(Mathf.Sign(desMovementVector.x));
                break;
            case "Jump":
                // not doing "isJumping = mode;" cause that would cause just too many problems.... unless i make it if also has sufficient amount of jumps? stream of thought things are incomprehensible my life is komeiji and i am a koishi denpa what is denpa what is denpa what is denpa what is denpa what is

                jumpHeld = (mode && jumpProgress == -1);

                if (mode)
                {
                    if (jumpAmount > 0 && jumpProgress == -1)
                    {
                        jumpAmount--;
                        jumpProgress = jumpAnimTime;

                        /*jumpAmount--;
                        isJumping = true;
                        changeGravity(moveset.jumpGravityMult);
                        ownRb.linearVelocityY = Mathf.Sqrt(moveset.jumpHeight * -2f * (Physics2D.gravity.y * ownRb.gravityScale));//*/
                    }
                }
                break;
            case "Run":
                print("invoke run regardless of input strength (on true only)");
                break;
            case "Attack":
                break;
            case "Shot":
                break;
            case "Direction Smash":
                break;
            case "Shield":
                break;
            case "Taunt":
                if (isGrounded) ownAnimator.Play("taunt");
                break;
            case "Pause":
                break;
            case "devSkinSwitch":
                //print("devSkinSwitch");
                if (mode) GetComponent<PlayerSkinApplier>().skinSwitch();
                break;
        }
    }
}
