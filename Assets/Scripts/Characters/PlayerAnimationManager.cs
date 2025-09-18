using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public PlayerMoveset moveset;
    public PlayerStats ownStats;

    [Header("1: right;\n-1: left")]
    public float lookDire;
    public Transform modelCont;
    public float turnCooldown;
    [SerializeField]
    private Animator ownAnimator;

    private float lateSwitchDire = 0;

    private void FixedUpdate()
    {
        if (lateSwitchDire != 0) switchDirection(lateSwitchDire);
    }
    public void switchDirection(float input, bool ignoreAirCheck = false, bool isRecovery = false)
    {
        //print("switchLookDireTo " + Mathf.Sign(input));

        if (isState("noAttack") || isState("specialFall"))
        {
            lateSwitchDire = Mathf.Sign(input);
            return;
        }
        lateSwitchDire = 0;

        if ((input == 0 || !moveset.isGrounded && !ignoreAirCheck || moveset.duckState /*|| ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")*/) && !isRecovery || moveset.ownStats.isKnocked || moveset.ownStats.isStunned) return;

        if (lookDire != Mathf.Sign(input) || ignoreAirCheck)
        {
            print("direChange; call for \"turn around\" animation");
            //moveset.sprintMultiplier = 1f;
            moveset.changeSprintMult();

            lookDire = Mathf.Sign(input);

            //if (moveset.isGrounded && Mathf.Abs(moveset.ownRb.linearVelocityX) >= 0.1f) setAnimation("turnAround"); // for now
            if (moveset.isGrounded) setAnimation("turnAround"); // for now

            transform.localScale = new Vector3(lookDire, 1f, 1f);
        }
    }
    public void setAnimation(string input)
    {
        ownAnimator.Play(input); // for now
    }
    public void setBool(string input, bool value)
    {
        ownAnimator.SetBool(input, value);
    }
    public bool isState(string input)
    {
        return ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag(input);
    }
    public bool isName(string input)
    {
        return ownAnimator.GetCurrentAnimatorStateInfo(0).IsName(input);
    }
}
