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

    // Attack Variables
    [SerializeField]
    private float attackDamage;
    public float damageIncrease;
    public Vector2 knockback = Vector2.zero;

    // This is a start rather than awake as the 'current' variables for stuff like melee damage are done within the Awake methods, so we want these to be done after
    private void Start() 
    {
        // Grabs all linked scripts + components
        parent = transform.root.gameObject;
        controller = parent.GetComponent<UniversalController>();

        // Increases attack damaged based on the damageIncrease variable within the inspector
        attackDamage = controller.MeleeDamageAmount + damageIncrease;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
        collisionParent = collision.transform.root.gameObject;
        // See if it can be hit
        HPHandler hpHandler = collisionParent.GetComponent<HPHandler>();

        collisionController = collisionParent.GetComponent<UniversalController>();
        if ((collisionParent.gameObject.tag == "Enemy" && parent.gameObject.tag == "Player") || (collisionParent.gameObject.tag == "Player" && parent.gameObject.tag == "Enemy"))
        {
            if (!collisionController.IsInvulnerable)
            {
                hpHandler.TakeDamage(attackDamage);
            }
        }
    }
}