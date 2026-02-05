using UnityEngine;
using System.Collections.Generic;

public class CooldownTimer : MonoBehaviour
{
    // Script + Component Links
    UsesCooldown usingScript;

    // Dictionaries for Cooldown Storage
    public Dictionary<string, float> cooldownDict = new Dictionary<string, float>();
    public Dictionary<string, float> timerDict = new Dictionary<string, float>();
    public Dictionary<string, int> timerStatusDict = new Dictionary<string, int>();
    public Dictionary<string, UsesCooldown> scriptUsing = new Dictionary<string, UsesCooldown> {};
    public List<StringList> visibleLists = new List<StringList>();

    // Internal Logic Variables
    private int cooldownCount;
    public List<string> cooldownKeys;
    private string currentKey;

    private void FixedUpdate()
    {
        CheckCooldowns();
        UpdateVisibleLists();
    }

    // Called by the scripts using cooldowns
    public void SetupTimers(List<string> keys, List<float> lengths, UsesCooldown callingScript)
    {
        usingScript = callingScript;

        List<string> completedKeys = new List<string>();

        for (var i = 0; i < keys.Count; i++)
        {
            // Temp variables for better access
            string key = keys[i];
            float length = lengths[i];

            // Setup logic dictionaries
            cooldownDict[key] = length;
            timerDict[key] = 0;
            timerStatusDict[key] = 0;
            scriptUsing[key] = usingScript;

            // Setup visual list
            StringList timer = new StringList();
            timer.objects.Add(key); timer.objects.Add(length.ToString()); timer.objects.Add("0"); timer.objects.Add("False");
            visibleLists.Add(timer);

            cooldownKeys.Add(key);

            // Iterate to keep count stored for logic
            cooldownCount++;

            completedKeys.Add(key);
        }
    }

    public void CheckCooldowns()
    {
        // Checks each timer, if they are currently ticking up and running (thus greater than zero), checks if the timer has been running longer than its
        // cooldown. If so, it sets the status of that cooldown to 0, so it will no longer tickup as well as reseting the timer value itself.
        for (var i = 0; i < cooldownCount; i++)
        {
            // Grabs the name of the currently checked cooldown from the cooldowns string list
            currentKey = cooldownKeys[i];
            // Grabs the currently  being checked cooldown, timer and status variables
            float cooldown = cooldownDict[currentKey];
            float duration = timerDict[currentKey];
            int status = timerStatusDict[currentKey];
            // If the timer is active, tick it up by the time since last frame
            if (status == 1)
            {
                timerDict[currentKey] = duration + Time.deltaTime;
            }
            // If the timer has reached past the relevant cooldown, the timer is reset, the active status is set to 0 (false). This allows the relevant ability/action to be used again
            if (duration >= cooldown && status == 1)
            {
                timerDict[currentKey] = 0;
                timerStatusDict[currentKey] = 0;
                // Some cooldowns ending have processes to complete, the following function completes these
                scriptUsing[currentKey].CooldownEndProcess(currentKey);
            }
        }
    }
    
    private void UpdateVisibleLists()
    {
        for (var i = 0; i < visibleLists.Count; i++) 
        {
            StringList timer = visibleLists[i];
            // Length
            timer.objects[1] = cooldownDict[timer.objects[0]].ToString();
            // Timer Progression
            timer.objects[2] = timerDict[timer.objects[0]].ToString();
            // Timer Status
            timer.objects[3] = timerStatusDict[timer.objects[0]] == 1 ? "True" : "False";
        }
    }
}