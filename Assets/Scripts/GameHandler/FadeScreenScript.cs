using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FadeScreenScript : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    public GameObject colorScreen;
    public Image colorScreenImage;
    CooldownTimer cooldownHandler;
    
    // Customizable Values
    public float timeOnScreen;
    public float timeFading;

    private float blackScreenTime = 0.2f;

    private void Awake()
    {
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "timeOnScreen", "blackScreenTime" };
        List<float> lengthList = new List<float> { timeOnScreen, blackScreenTime };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
        cooldownHandler.timerStatusDict["blackScreenTime"] = 1;
    }

    public void FixedUpdate()
    {
        if (cooldownHandler.timerStatusDict["timeOnScreen"] == 1)
        {
            // Decides percentage of how much the screen will be faded and then applies that to the screen
            float alphaPercent = 1 - ((cooldownHandler.timerDict["timeOnScreen"] / timeFading) / 2);
            Color color = colorScreenImage.color;
            color.a = Mathf.Clamp01(alphaPercent);
            colorScreenImage.color = color;
        }
    }

    public void CooldownEndProcess(string key)
    {
        if (key == "timeOnScreen")
        {
            Destroy(this.gameObject);
        }
        else if (key == "blackScreenTime")
        {
            cooldownHandler.timerStatusDict["timeOnScreen"] = 1;
        }
    }
}
