using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

public class HPHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    UniversalController controller;
    LogicScript logicScript;
    CooldownTimer cooldownHandler;
    Animator animator;
    VisualAndSoundEffectHandling vsfxHandler;
    GameEndHandler gameEndHandler;
    SpriteRenderer renderer;
    Transform onHitFlashSprite;
    SpriteRenderer onHitFlashSpriteRenderer;
    GameData gameData;
    Color baseColor;

    // Sound played when entity hit - assigned in inspector
    public AudioClip hitSound;

    public float whiteFlashInterval = 0.2f;
    public float isFlashingLength = 0.5f;
    public float flashTime = 0.02f;
    public float drainDamageInterval = 1f;

    private float currentDrainAmount;
    private GameObject currentDrainEffect;
    private string currentDrainType;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GetComponent<UniversalController>();
        logicScript = GetComponent<LogicScript>();
        animator = GetComponent<Animator>();
        cooldownHandler = GetComponent<CooldownTimer>();
        vsfxHandler = GameObject.Find("GameHandler").GetComponent<VisualAndSoundEffectHandling>();
        gameEndHandler = GameObject.Find("GameHandler").GetComponent<GameEndHandler>();
        renderer = GetComponent<SpriteRenderer>();
        onHitFlashSprite = this.gameObject.transform.Find("OnHitFlashSprite");
        onHitFlashSpriteRenderer = onHitFlashSprite.GetComponent<SpriteRenderer>();
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();

        baseColor = new Color(255, 255, 255);
        baseColor.a = 0f;
        onHitFlashSpriteRenderer.color = baseColor;
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "flashInterval",
                                                  "isFlashing",
                                                  "flashLength",
                                                  "drainDamageInterval",
                                                  "drainTimeLeft"
                                                  };
        List<float> lengthList = new List<float> { whiteFlashInterval,
                                                   isFlashingLength,
                                                   flashTime,
                                                   drainDamageInterval,
                                                   0f // Filler, this will be changed for each drain amount
                                                   };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    public void TakeDamage(float damageDealt) 
    {
        // Last health is used so the animator trigger to enter death state isn't repeatably called while the entity is in death state
        float lastHealth = controller.CurrentHealth;

        // Don't do damage if entity is invulnerable
        if (controller.IsInvulnerable) { return; }

        // Take damage and tell animator damage has been taken
        controller.CurrentHealth = controller.CurrentHealth - damageDealt;
        animator.SetTrigger("damaged");

        // Start an invulnerability period to prevent attacks spamming damage dealt
        controller.IsInvulnerable = true;
        cooldownHandler.timerStatusDict["invulnerableOnHitTime"] = 1;

        // If there is an assigned hit sound, it is played
        if (hitSound != null) { vsfxHandler.PlaySound(hitSound, 2f); }

        cooldownHandler.timerStatusDict["flashInterval"] = 1;
        cooldownHandler.timerStatusDict["isFlashing"] = 1;

        // If just become dead
        if (controller.CurrentHealth <= 0 && lastHealth > 0)
        {
            // Tell animator entity is in death state
            animator.SetTrigger("dead");
            animator.SetBool("inDeathState", true);

            // Handle the death logic based on entity type
            if (gameObject.tag == "Enemy")
            {
                // Add to player's kill count
                gameData.playerKillCount++;

                // Start delay to death animation, and tell logic script to deactivate its processes
                cooldownHandler.timerStatusDict["deathDelay"] = 1;
                logicScript.Deactivate();
            }
            else if (gameObject.tag == "Player")
            {
                // Start delay to death animation and beging ending the game
                cooldownHandler.timerStatusDict["deathDelay"] = 1;
                gameEndHandler.EndGame();
            }
        }
    }

    public void HealDamage(float damageHealed) 
    {
        // Gain health
        controller.CurrentHealth += damageHealed;

        // Limit health to maximum health
        if (controller.CurrentHealth > controller.FullHealth)
        {
            controller.CurrentHealth = controller.FullHealth;   
        }
    }

    public void ResetHealth() 
    {
        controller.CurrentHealth = controller.FullHealth;
    }

    public void StartDrain(float drainDamageAmount, float drainTimeLeft, GameObject drainEffect, string drainType) 
    {
        // Can only start if drain is not already occuring
        if (cooldownHandler.timerStatusDict["drainTimeLeft"] != 1)
        {
            // Set logic variables based on new drain type - tells hp handler how to handle said effect
            currentDrainAmount = drainDamageAmount;
            currentDrainEffect = drainEffect;
            currentDrainType = drainType;

            // Sets cooldown to appropiate new value and starts the ticking time
            cooldownHandler.cooldownDict["drainTimeLeft"] = drainTimeLeft;
            cooldownHandler.timerStatusDict["drainTimeLeft"] = 1;
            cooldownHandler.timerStatusDict["drainDamageInterval"] = 1;
        }
    }

    public void ChangeDrainAmount(float newDrainDamageAmount, float newDrainTimeLeft, GameObject newDrainEffect) { }
    public void EndDrain() { }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key)
    {
        if (key == "flashInterval")
        {
            if (cooldownHandler.timerStatusDict["isFlashing"] == 1)
            {
                vsfxHandler.PlayVisualEffect("whiteflash", 0.75f, onHitFlashSpriteRenderer);

                cooldownHandler.timerStatusDict["flashLength"] = 1;
                cooldownHandler.timerStatusDict["flashInterval"] = 1;
            }
        }
        if (key == "flashLength")
        {
            // Reset renderer color, make entity regular sprite
            onHitFlashSpriteRenderer.color = baseColor;
        }
        if (key == "drainDamageInterval")
        {
            if (cooldownHandler.timerStatusDict["drainTimeLeft"] == 1)
            {
                // Store previous health
                float lastHealth = controller.CurrentHealth;

                // Take tick damage
                controller.CurrentHealth = controller.CurrentHealth - currentDrainAmount;

                // If toxic effect, spawn effect
                if (currentDrainType == "toxicEffect")
                {
                    GameObject spawnedEffect = Instantiate(currentDrainEffect, new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(-0.5f, 0.5f), transform.position.z), Quaternion.Euler(0, 0, 0));
                    vsfxHandler.PlayVisualEffect("toxicEffect", 1, spawnedEffect.GetComponent<SpriteRenderer>());
                }

                // If just become dead
                if (controller.CurrentHealth <= 0 && lastHealth > 0)
                {
                    // Tell animator entity is in death state
                    animator.SetTrigger("dead");
                    animator.SetBool("inDeathState", true);

                    // Handle the death logic based on entity type
                    if (gameObject.tag == "Enemy")
                    {
                        // Add to player's kill count
                        gameData.playerKillCount++;

                        // Start delay to death animation, and tell logic script to deactivate its processes
                        cooldownHandler.timerStatusDict["deathDelay"] = 1;
                        logicScript.Deactivate();
                    }
                    else if (gameObject.tag == "Player")
                    {
                        // Start delay to death animation and beging ending the game
                        cooldownHandler.timerStatusDict["deathDelay"] = 1;
                        gameEndHandler.EndGame();
                    }
                }

                // Cause another interval for drain
                cooldownHandler.timerStatusDict["drainDamageInterval"] = 1;
            }
        }
        if (key == "drainTimeLeft")
        {
            currentDrainAmount = 0f;
            currentDrainEffect = null;
            currentDrainType = null;
        }
    }
}