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

    private int amountOfLayersDone = 0;

    void Awake()
    {
        // Grabs all linked scripts + components
        player = GameObject.Find("Player");
        cooldownHandler = GetComponent<CooldownTimer>();

        // Setup cooldown for how long between each fire layer
        List<string> keyList = new List<string> { "timeBetweenLayers" };
        List<float> lengthList = new List<float> { timeBetweenLayers };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    private void FixedUpdate()
    {
        cooldownHandler.CheckCooldowns();
    }

    public void CooldownEndProcess(string key) 
    {
        Debug.Log((amountOfLayersDone, rainLayers));
        if (amountOfLayersDone < rainLayers)
        {
            RainFire();
            amountOfLayersDone++;
            cooldownHandler.timerStatusDict["timeBetweenLayers"] = 1;
        }
        else
        {
            amountOfLayersDone = 0;
        }
    }

    public void OnActivation() 
    {
        RainFire();
        amountOfLayersDone++;
        cooldownHandler.timerStatusDict["timeBetweenLayers"] = 1;
        Debug.Log((amountOfLayersDone, rainLayers, cooldownHandler.timerStatusDict["timeBetweenLayers"]));
    }

    public void RainFire()
    {
        // Decides the position for the first arrow to be fired from
        float currentFirePosition = (float)(player.transform.position.x + 0.5 * fireRange);
        for (var i = 1; i <= ballCount; i++)
        {
            // Creates the firing projectile
            GameObject firedProjectile = Instantiate(projectile, new Vector3(currentFirePosition, player.transform.position.y + 0.4f, player.transform.position.z), Quaternion.Euler(0, 0, 0));

            // Seperates each arrow evenly across the fire range
            currentFirePosition -= (fireRange / ballCount);
        }
    }
}
