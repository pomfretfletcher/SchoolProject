using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    // Script + Component Links
    private Collider2D selfCollider;

    [Header("Unity Events")]
    public UnityEvent newCollision;
    public UnityEvent noCollisionsRemain;
    public UnityEvent collisionStay;

    // Private variables/objects for dealing with collision layer filtering
    private ContactFilter2D filter;
    private ContactFilter2D playerFilter;
    private Collider2D[] tempResults = new Collider2D[16];
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    // Private variables to store current state of collision logic
    private bool hasCollisions = false;
    private bool playerSeen = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        selfCollider = GetComponent<Collider2D>();

        // Sets up filter for collisions with floors
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        int layerIndex = LayerMask.NameToLayer("Collidable");
        filter.layerMask = 1 << layerIndex;

        // Sets up filter for detecting player
        playerFilter = new ContactFilter2D();
        playerFilter.useLayerMask = true;
        int playerLayerIndex = LayerMask.NameToLayer("Player");
        playerFilter.layerMask = 1 << playerLayerIndex;
    }

    private void FixedUpdate()
    {
        // Detect if player seen
        playerSeen = selfCollider.Overlap(playerFilter, tempResults) > 0;
        // Populate the temp array with all collisons with collidable layer objects
        int count = selfCollider.Overlap(filter, tempResults);
        
        // Copy valid results into the List - used to keep track of values in a more readable list
        detectedColliders.Clear();
        for (int i = 0; i < count; i++)
        {
            if (tempResults[i] != null)
            {
                detectedColliders.Add(tempResults[i]);
            }
        }
        hasCollisions = detectedColliders.Count > 0;

        // Fire events
        if (playerSeen)
        {
            // Most likely attack
            collisionStay.Invoke();
        }
        else if (!hasCollisions)
        {
            // Most likely flip direction
            noCollisionsRemain.Invoke();
        }
        else if (hasCollisions)
        {
            newCollision.Invoke();
        }
    }
}