using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
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

    public Animator ownAnimator;
    public string currentAnimation;
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
    public void switchDirection(float input)
    {
        if (input == 0) return;
        //print("direChange");
        lookDire = Mathf.Sign(input);
        progDire = lookDire;
    }
    void turnAnim()
    {
        modelCont.rotation = Quaternion.Lerp(directionRefs[0].rotation, directionRefs[1].rotation, turnCurve.Evaluate(turnProg / turnTime));
    }
    public void playAnimation(string input, bool surpassNoMove = false)
    {
        print("play anim: " + input);

        if (!ownAnimator.GetCurrentAnimatorStateInfo(0).IsTag("noAttack") || surpassNoMove)
        {

            if (input == currentAnimation) return;
            currentAnimation = input;
            ownAnimator.StopPlayback();
            ownAnimator.enabled = false;
            ownAnimator.enabled = true;
            ownAnimator.Play(input);
        }
    }
}
