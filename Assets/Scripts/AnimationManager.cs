using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public PlayerMoveset ownPlayer;

    public Animator ownAnimator;

    public float walkAnimVeloThreshold;

    private string currentAnim;


    // New Animation Naming Convention:

    // aBBBBCCC

    // a - input
    //      d - no input (default)
    //      m - movement (WASD)
    //      x - action (JKLT Space)

    // BBBB - animation name
    //      NSmash - jab
    //      USmash - up smash
    //      SSmash - side smash
    //      DSmash - down smash

    //      NAir - neutral air
    //      UAir - up air
    //      SAir - side air
    //      DAir - down air

    //      NShot - shot (can add directional specials at a later point in development)
    //      Recovery - recovery

    // CCC - airborne
    //      Air - airborne

    // Examples:

    // dIdle - grounded idle
    // xUAttackAir - airborne up smash (up air)
    // xNShot - grounded shot

    private void Update()
    {
        // add a check for ducking

        if (true) // if isn't using a move or taunting (add later)
        {
            if (ownPlayer.isGrounded)
            {
                if (ownPlayer.ownStats.isStunned)
                {
                    playAnimation("stun");
                } // play stunned animation
                else if (ownPlayer.ownStats.isKnocked)
                {
                    playAnimation("knockGround");
                } // play knocked animation
                else
                {
                    if (Mathf.Abs(ownPlayer.ownRb.linearVelocityX) > walkAnimVeloThreshold)
                    {
                        if (true)
                        {
                            playAnimation("run");
                        } // play run animation
                        else if (true)
                        {
                            playAnimation("walk");
                        } // play walk animation
                        else
                        {
                            playAnimation("idle");
                        } // play idle animation (sliding on ground when blasted)
                    } // if horizontal velocity is above threshold
                    else
                    {
                        playAnimation("idle");
                    } // else, play idle animation
                }
            } // if grounded
            else
            {
                if (ownPlayer.ownStats.isStunned || ownPlayer.ownStats.isKnocked)
                {
                    playAnimation("knockAir");
                } // if stunned / knocked, play airKnock animation
                else
                {
                    if (true)
                    {
                        playAnimation("idleAir");
                    } // if not in jump animation
                }
            } // if airborne
        } // if isn't using a move or taunting (add later)
    }
    void playAnimation(string input)
    {
        if (currentAnim != input)
        {
            currentAnim = input;
            print("current anim: " + currentAnim);
        }
    }
}
