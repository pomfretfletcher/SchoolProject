using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    // Method that will trigger when no collisions are within the detection zone (such as the ground ending into a cliff)
    public UnityEvent noCollisionsRemain;

    Collider2D col;
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    private void Awake()
    {
        // Grabs collider child object from enemy
        col = GetComponent<Collider2D>();
    }

    // Stores every collision within the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
    }

    // Removes collisions from collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
        if (detectedColliders.Count <= 0)
        {
            noCollisionsRemain.Invoke();
        }
    }
}