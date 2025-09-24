using UnityEngine;
public enum vmd
{
    Add,
    Set
}
public class CharacterAnimationEvents : MonoBehaviour
{
    public Rigidbody2D ownRb;
    public PlayerAnimationManager pam;
    public void setHorVelo(float input)
    {
        if (Mathf.Abs(ownRb.linearVelocityX) < Mathf.Abs(input)) ownRb.linearVelocityX = input * pam.lookDire;
    }
    public void setVertVelo(float input)
    {
        if (ownRb.linearVelocityY < input) ownRb.linearVelocityY = input;
    }
}
