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
    public float jumpHeight, jumpGravityMult = 1.6f, gravityStrength = 1f;

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
    bool isGrounded;

    private void Awake()
    {
        changeGravity(moveset.gravityStrength);
    }
    private void FixedUpdate()
    {
        if (isJumping && ownRb.linearVelocityY <= 0)
        {
            isJumping = false;
            changeGravity(moveset.gravityStrength);
        }
    }
    void changeGravity(float input)
    {
        ownRb.gravityScale = input; // can make alterations here for Metal
        // when interrupted, change to x12 gravity scale regardless of what moveset.gravityStrength is
    }
    public void contrInput(InputAction.CallbackContext obj)
    {
        //print(obj.action.name);

        //print("device: " + obj.action.activeControl.device.displayName);
        bool mode = obj.action.triggered;

        switch (obj.action.name)
        {
            default:
                print("WHAT; this should be impossible");
                break;
            case "Move":
                break;
            case "Jump":
                // not doing "isJumping = mode;" cause that would cause just too many problems.... unless i make it if also has sufficient amount of jumps? stream of thought things are incomprehensible my life is komeiji and i am a koishi denpa what is denpa what is denpa what is denpa what is denpa what is
                if (mode)
                {
                    isJumping = true;
                    changeGravity(moveset.jumpGravityMult);
                    ownRb.linearVelocityY = Mathf.Sqrt(moveset.jumpHeight * -2f * (Physics2D.gravity.y * ownRb.gravityScale));
                }
                else
                {
                    if (isJumping)
                    {
                        isJumping = false;
                        changeGravity(moveset.gravityStrength);
                        ownRb.linearVelocityY *= 0.3f;
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
