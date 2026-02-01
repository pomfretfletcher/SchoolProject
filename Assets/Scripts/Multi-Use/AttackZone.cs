using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class AttackZone : MonoBehaviour
{
    // Script + Component Links
    GameObject parent;
    GameObject collisionParent;
    Collider2D attackHitbox;
    UniversalController controller;
    UniversalController collisionController;

    // Customizable Values
    public float damageIncrease;
    public Vector2 knockback = Vector2.zero;

    private void Awake() 
    {
        // Grabs all linked scripts + components
        parent = transform.root.gameObject;
        controller = parent.GetComponent<UniversalController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Decide what knockback is applied
        Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

        // Grab the object that is colliding with
        collisionParent = collision.transform.gameObject;
        collisionController = collisionParent.GetComponent<UniversalController>();
        HPHandler hpHandler = collisionParent.GetComponent<HPHandler>();
        KnockbackLogic knockbackHandler = collisionParent.GetComponent<KnockbackLogic>();

        // If of opposing tags, deal damage to the opponent collider
        if (collisionParent.gameObject.tag == "Player" && parent.gameObject.tag == "Enemy")
        {
            if (!collisionController.IsInvulnerable)
            {
                hpHandler.TakeDamage(controller.MeleeDamageAmount + damageIncrease);
                if (deliveredKnockback.x != 0)
                {
                    knockbackHandler.ExperienceKnockback(deliveredKnockback);
                }
            }
        }

        // If of opposing tags, deal damage to the opponent collider. As well as making sure damage doesnt go below minimum damage
        else if (collisionParent.gameObject.tag == "Enemy" && parent.gameObject.tag == "Player")
        {
            if (!collisionController.IsInvulnerable)
            {
                PlayerController playerController = parent.GetComponent<PlayerController>();
                if (controller.MeleeDamageAmount + damageIncrease > playerController.minMeleeDamage)
                {
                    hpHandler.TakeDamage(controller.MeleeDamageAmount + damageIncrease);
                    if (deliveredKnockback.x != 0)
                    {
                        knockbackHandler.ExperienceKnockback(deliveredKnockback);
                    }
                }
                else
                {
                    hpHandler.TakeDamage(playerController.minMeleeDamage);
                    if (deliveredKnockback.x != 0)
                    {
                        knockbackHandler.ExperienceKnockback(deliveredKnockback);
                    }
                }
            }
        }
    }
}