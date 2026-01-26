using UnityEngine;
using System.Collections.Generic;

public class FireRain : MonoBehaviour, IsAbility, UsesCooldown
{
    // Script + Component Links
    public GameObject projectile;
    GameObject player;
    CooldownTimer cooldownHandler;

    // Customizable Values
    public int ballCount;
    public int rainLayers;
    public float fireRange;
    public float damage;
    public float timeBetweenLayers;

    // Private internal logic variables
    private int amountOfLayersDone = 0;

    private void Awake()
    {
        // Grabs all linked scripts + components
        player = GameObject.Find("Player");
        cooldownHandler = GetComponent<CooldownTimer>();

        // Setup cooldown for how long between each fire layer
        List<string> keyList = new List<string> { "timeBetweenLayers" };
        List<float> lengthList = new List<float> { timeBetweenLayers };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    public void CooldownEndProcess(string key) 
    {
        // If there are still layers to spawn, spawn one and reactivate the timer
        if (amountOfLayersDone < rainLayers)
        {
            RainFire();
            amountOfLayersDone++;
            cooldownHandler.timerStatusDict["timeBetweenLayers"] = 1;
        }
        // If all layers have been spawned, reset the private layersdone variable to be ready for next activation
        else
        {
            amountOfLayersDone = 0;
        }
    }

    public void OnActivation() 
    {
        // Spawn a layer of fire and activate the timer that once ended, will spawn another layer
        RainFire();
        amountOfLayersDone++;
        cooldownHandler.timerStatusDict["timeBetweenLayers"] = 1;
    }

    public void RainFire()
    {
        // Decides the position for the first arrow to be fired from
        float currentFirePosition = (float)(player.transform.position.x + 0.5 * fireRange);
        for (var i = 1; i <= ballCount; i++)
        {
            // Creates the firing projectile
            GameObject firedProjectile = Instantiate(projectile, new Vector3(currentFirePosition, player.transform.position.y + 0.7f, player.transform.position.z), Quaternion.Euler(0, 0, 0));

            // Seperates each arrow evenly across the fire range
            currentFirePosition -= (fireRange / (ballCount - 1));
        }
    }
}