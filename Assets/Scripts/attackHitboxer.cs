using UnityEngine;

public class attackHitboxer : MonoBehaviour
{
    public Hitbox ownAttackHitbox;
    public void changeAngle()
    {
        ownAttackHitbox.angle = GetComponentInParent<PlayerAnimationManager>().lookDire;
    } // on attack start
    public void changeHitAmount(int input)
    {
        ownAttackHitbox.hitAmount = input;
    } // on attack start
    public void changeAxis(int input)
    {
        ownAttackHitbox.direction = Mathf.Sign(input);
    } // on attack start
    public void changeType(hbt input)
    {
        ownAttackHitbox.switchHitboxType(input);
    } // well that was useless... EXCEPT FOR CHANGE TYPE, PERCHANCE?????? CAUSE IT'S DEFINITELY HITAMOUNT YO
}
