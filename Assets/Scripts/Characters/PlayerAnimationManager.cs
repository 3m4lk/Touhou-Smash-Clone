using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public PlayerMoveset moveset;

    [Header("1: right;\n-1: left")]
    public float lookDire;
    public Transform modelCont;

    [Space]
    public Transform[] directionRefs;

    [Space]
    public AnimationCurve turnCurve;
    public float turnTime;
    private float turnProg;
    private float progDire;

    //public Animator ownAnimator;
    public string currentAnimation;

    public string currentTag;
    private void Awake()
    {
        turnProg = turnTime;
        progDire = 1f;
    }

    private void Update()
    {
        turnProg = Mathf.Clamp(turnProg + Time.deltaTime * progDire, 0f, turnTime);

        turnAnim();
    }
    public void switchDirection(float input, bool ignoreAirCheck = false, bool isRecovery = false)
    {
        if ((input == 0 || !moveset.isGrounded && !ignoreAirCheck || moveset.duckState /*|| ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")*/) && !isRecovery || moveset.ownStats.isKnocked || moveset.ownStats.isStunned) return;
        //print("direChange");

        if (lookDire != Mathf.Sign(input) || ignoreAirCheck)
        {
            moveset.sprintMultiplier = 1f;

            lookDire = Mathf.Sign(input);
            progDire = lookDire;

            //print("changeDire");
            //moveset.sprintCooldown = 0;
        }
    }
    void turnAnim()
    {
        modelCont.rotation = Quaternion.Lerp(directionRefs[0].rotation, directionRefs[1].rotation, turnCurve.Evaluate(turnProg / turnTime));
    }
    public void playAnimation(string input, bool surpassNoMove = false)
    {
        //currentTag = ownAnimator.GetCurrentAnimatorStateInfo(0).tagHash + "";
        //print("play anim: " + input);

        if (/*!ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack") || */surpassNoMove)
        {
            if (!moveset.duckState)
            {
                moveset.duckCollisions(false);
                //if (input == currentAnimation) return;
                currentAnimation = input;

                // get some solution for replaying alr playing animations

                //ownAnimator.Play(input, -1, 0);
            }
            else
            {
                currentAnimation = input;

                //ownAnimator.Play(input, -1, 0);
            }
        }
    }
}
