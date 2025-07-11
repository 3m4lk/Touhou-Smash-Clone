using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public PlayerMoveset moveset;

    [Header("1: right;\n-1: left")]
    public float lookDire;
    public Transform modelCont;
    public float turnCooldown;
    private float turnProg;

    private void Update()
    {
        //turnProg = Mathf.Clamp(turnProg + Time.deltaTime * progDire, 0f, turnCooldown);

        //turnAnim();
    }
    public void switchDirection(float input, bool ignoreAirCheck = false, bool isRecovery = false)
    {
        if ((input == 0 || !moveset.isGrounded && !ignoreAirCheck || moveset.duckState /*|| ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack")*/) && !isRecovery || moveset.ownStats.isKnocked || moveset.ownStats.isStunned) return;

        if (lookDire != Mathf.Sign(input) || ignoreAirCheck)
        {
            print("direChange; call for \"turn around\" animation");
            moveset.sprintMultiplier = 1f;

            lookDire = Mathf.Sign(input);
            transform.localScale = new Vector3(lookDire, 1f, 1f);
        }
    }
}
