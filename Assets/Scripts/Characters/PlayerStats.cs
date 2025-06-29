using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public PlayerMoveset moveset;
    public MatchManager match;
    public PlayerAnimationManager animManager;

    public Hitbox ownHitbox;


    public bool isAlive;

    public float tillRespawnTime;
    public float platformStallTime;

    [Space]
    public float tillRespawnCooldown;
    public float platformStallCooldown;

    private bool hasReplacedPlayerOnStage = false;
    private int respawnPlatformIndex;

    [HideInInspector]
    public bool respawnOnInput;

    [Space]
    public float invulnerability;

    public float damage;
    public TMP_Text damageText;

    public float maxKnockStrength;

    [Space]
    public float testDmg;
    public Vector2 testKnockVec;

    [Space]
    public bool shieldMode;

    public Transform shield;
    public Vector2 shieldSizes;
    public float shieldMaxHealth;
    public float shieldProgress;
    private float shieldDire;

    public bool desUnshield;
    public float shieldPunishmentWindow;

    [Space]
    public bool isKnocked;

    [Tooltip("what gets applied on knock")]
    public float knockTime;
    public float knockProgress;

    // kb under x doesn't wake up from knock (fox's blaster)
    // kb above x disables knock (regular jab to wake up)
    // kb above y applies knock (any side smash / stronger attack)
    public Vector2 kbKnockThreshold;

    // knocked:
    // > aerial speed is halved
    // > bouncing off walls at high velo / getting hit when knocked / getting grounded when knocked resets knockTime
    // > when knockTime runs out, any Player input boots out of knock (unless moving faster than walk speed on horizontal) (no non-movement, in brawl that works too)

    public bool isStunned;
    public float stunTime;
    public float stunProgress;
    public Vector2 shieldbreakBlast;

    private bool unstunFaster = false;

    private void Update()
    {
        if (moveset && moveset.ownController.devMode && Input.GetKeyDown(KeyCode.R)) kill();

        if (Input.anyKeyDown) unstunFaster = true;
    }
    private void FixedUpdate()
    {
        if (!isAlive)
        {
            tillRespawnCooldown = Mathf.Max(tillRespawnCooldown - Time.fixedDeltaTime, 0f);
            if (tillRespawnCooldown == 0)
            {
                if (!hasReplacedPlayerOnStage)
                {
                    hasReplacedPlayerOnStage = true;
                    replaceOnStage();
                }

                platformStallCooldown = Mathf.Max(platformStallCooldown - Time.fixedDeltaTime, -10f);

                // if below 0 (after 2 sec.), enable respawning on any input

                // if reaches -10, respawn forcefully
                respawnOnInput = (platformStallCooldown <= 0f);

                if (platformStallCooldown == -6f) respawn();
            }
        } // dead
        else
        {
            float unstunMult = 1f;
            if (unstunFaster) unstunMult = 6f; // keymashing unstuns you faster

            stunProgress = Mathf.Max(stunProgress - unstunMult * Time.fixedDeltaTime, 0f);
            if (stunProgress == 0f && isStunned)
            {
                // boot out of stun animation
                //print("DESTUN ANIM");
                animManager.playAnimation("idle");
            }
            isStunned = (stunProgress != 0f);

            ownHitbox.switchHitboxType(hbt.Invincible);
            if (invulnerability == 0f)
            {
                ownHitbox.switchHitboxType(hbt.Hitbox);
                /*if (isStunned && stunProgress > 0f)
                {
                    //ownHitbox.switchHitboxType(hbt.Invincible);
                } //*/ // why? // 8w i have an idea//*/ // ON KNOCK NOT STUN YOU IMBECILE

                if (isKnocked && knockProgress > 0f && moveset.isGrounded)
                {
                    ownHitbox.switchHitboxType(hbt.Invincible);
                }
            }

            knockProgress = Mathf.Max(knockProgress - Time.fixedDeltaTime, 0f);

            shieldPunishmentWindow = Mathf.Max(shieldPunishmentWindow - Time.fixedDeltaTime, 0f);

            invulnerability = Mathf.Max(invulnerability - Time.fixedDeltaTime, 0f);

            shieldDire = 0.7f; // higher shield regen time to compensate for spam punishment
            if (shieldMode) shieldDire = -1f;

            shield.gameObject.SetActive(shieldMode); // to prevent the shieldbug

            shieldProgress = Mathf.Clamp(shieldProgress + shieldDire * Time.fixedDeltaTime, 0f, shieldMaxHealth);

            shield.localScale = Vector2.Lerp(Vector2.one * shieldSizes.x, Vector2.one * shieldSizes.y, 1f - shieldProgress / shieldMaxHealth);

            if (shieldProgress == 0f)
            {
                print("<color=red>SHIELDBREAK");
                shieldBreak();
            }

            unstunFaster = false;
        } // alive
    }
    void shieldBreak()
    {
        if (!isStunned)
        {
            //ptinr("sb");
            // >ptinr

            print("sb");

            isStunned = true;
            moveset.ownRb.linearVelocity = shieldbreakBlast * 10f;
            stunProgress = stunTime;
        }
    }
    public void dealDmg(float amount, Vector2 knockVec) // set up kb later
    {
        damage = Mathf.Clamp(damage + amount, 0f, 999.9f);
        updateDamageText();

        //Vector2 hitKb = testKnockVec * 10f * Mathf.Lerp(1f, maxKnockStrength, damage / 999.9f);

        Vector2 hitKb = knockVec * 10f * Mathf.Lerp(1f, maxKnockStrength, damage / 999.9f);

        //moveset.ownRb.AddForce(hitKb, ForceMode2D.Impulse);

        //moveset.ownRb.AddForce(hitKb, ForceMode2D.Force);

        Vector2 oldVelo = moveset.ownRb.linearVelocity;
        moveset.ownRb.linearVelocity = hitKb;

        //print("do a hitstun if char is knocked with like, a shitton of kb");
        print("kb amount: " + hitKb.magnitude);

        // very weak kb attacks won't apply knock
        if (hitKb.magnitude > kbKnockThreshold.x * 10f)
        {
            // cancel recovery fall animation
            if (!moveset.isGrounded)
            {
                animManager.playAnimation("idleAir", true);
            }
            isKnocked = false; // attacks above minimum strength disable knock...

            if (hitKb.magnitude > kbKnockThreshold.y * 10f)
            {
                isKnocked = true; // ...unless strength exceeds another threshold
                knockProgress = knockTime;
                print("<color=green>Apply Knock");
                animManager.playAnimation("airKnocked", true);
            }
            else
            {
                print("<color=yellow>boot out of knock");
            }
        }
        else
        {
            moveset.ownRb.linearVelocity = oldVelo; // matter of fact they won't even apply knockback, lmfao
            print("<color=black>don't boot out of knock; matter of fact, don't even apply knockback... LMFAO");
        }

        // if above kb threshold, apply knock
    }
    /*float distInDesDire(Vector2 input, Vector2 desiredDirection)
    {
        return input.magnitude * Vector2.Dot(input.normalized, desiredDirection.normalized);
    }//*/
    public void kill()
    {
        if (!isAlive) return;

        shieldProgress = shieldMaxHealth;
        hasReplacedPlayerOnStage = false;
        respawnOnInput = false;
        isAlive = false;

        moveset.ownRb.bodyType = RigidbodyType2D.Static;

        tillRespawnCooldown = tillRespawnTime;
        platformStallCooldown = platformStallTime;
        toggleShield(false);
        shieldMode = false;
        shield.gameObject.SetActive(false);
    }
    public void respawn()
    {
        if (!isAlive)
        {
            animManager.ownAnimator.Play("idleAir");
            isAlive = true;
            respawnOnInput = false;
            moveset.ownRb.bodyType = RigidbodyType2D.Dynamic;
            match.freeRespPlatform(respawnPlatformIndex);
            respawnPlatformIndex = -1;
            //invulnerability = 1f; // unless needed
            //animManager.playAnimation("idle");

            if (invulnerability > 4f)
            {
                invulnerability = 3f;
            } // so Player doesn't get a shitton of invulnerab when stepping right outta the platform
        }
    }
    void replaceOnStage()
    {
        if (!animManager) return;

        isStunned = false;
        stunProgress = 0;

        isKnocked = false;
        knockProgress = 0;

        damage = 0;
        updateDamageText();
        animManager.ownAnimator.Play("idle");
        //animManager.playAnimation("idle");
        respawnPlatformIndex = match.pickRespawnPlatform();
        moveset.ownRb.position = match.respawnLocations[respawnPlatformIndex].position;
        // place Player on resp platform
    }
    void updateDamageText()
    {
        damageText.text = moveset.moveset.name + "\n<size=64><color=#FFFFFF><b>" + damage.ToString("0.0") + "%";
    }
    public void toggleShield(bool mode)
    {
        print("SHIELD TOGGLE " + mode);

        shieldMode = mode;

        if (mode)
        {
            shield.gameObject.SetActive(true);
        }
        else
        {
            if (shieldPunishmentWindow > 0f)
            {
                //shieldProgress = Mathf.Clamp(shieldProgress - 1.2f, 0.03f, shieldMaxHealth); // punishment for shieldspamming
                shieldProgress = Mathf.Max(shieldProgress - 1.2f, 0f);
                print("SHIELDPUNISH");
            }
            shieldPunishmentWindow = 0.3f;
        }
    }
    public void bootOutOfKnock()
    {
        isKnocked = false;
        moveset.vanityAnim("idle");
    }
}