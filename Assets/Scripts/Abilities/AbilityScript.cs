using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    IsAbility specificAbilityScript;

    public float cooldown;
    public float timerProgression;

    void Awake()
    {
        specificAbilityScript = GetComponent<IsAbility>();
    }

    public void OnUse()
    {
        Debug.Log("ability part 2 works");
        bool needToDelete = specificAbilityScript.OnActivation();
        if (needToDelete)
        {
            Destroy(this.gameObject);
        }
        Debug.Log("ability part 4 works");
    }
}
