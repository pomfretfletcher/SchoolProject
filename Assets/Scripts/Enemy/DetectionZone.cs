using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    // Unity Events
        // Method that will trigger when a new collisions is detected within the detection zone (such as a player entering the collider)
    public UnityEvent newCollision;
        // Method that will trigger when no collisions are within the detection zone (such as the ground ending into a cliff)
    public UnityEvent noCollisionsRemain;
        //
    public UnityEvent collisionStay;

    // Misc Variables
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    public int zoneType; // 0 for on enter zones and 1 for exit zones and 2 for stay zones

    // Stores every collision within the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
    }

    private void FixedUpdate()
    {
        // If a collider is within the detection zone, call the associated method every set interval of 0.02 seconds
        if (detectedColliders.Count > 0 && zoneType == 2)
        {
            collisionStay.Invoke();
        }
    }

    // Removes collisions from collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
        if (detectedColliders.Count <= 0 && zoneType == 1)
        {
            noCollisionsRemain.Invoke();
        }
    }
}