using UnityEngine;
using System;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    // Script + Component Links
    PlayerController playerController;
    SpriteRenderer renderer;

    // List of all the sprites used to show health levels - listed in inspector
    public List<Sprite> sprites = new List<Sprite>();

    private void Awake()
    {
        // Grabs all linked scripts + components
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // Calculates what amount of health bar the player has remaining and chooses the most appropiate sprite for said health amount (a sprite every 5 percent)
        // Since there are 20 health levels (not including no health), we multiply it by 20
        int interval = (int)Math.Ceiling((decimal)((playerController.currentHealth * 20)/ playerController.fullHealth));
        if (interval >= 0)
        {
            renderer.sprite = sprites[interval];
        }
        else
        {
            renderer.sprite = sprites[0];
        }
    }
}