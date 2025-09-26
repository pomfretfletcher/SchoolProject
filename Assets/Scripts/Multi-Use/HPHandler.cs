using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class HPHandler : MonoBehaviour
{
    // Universal Controller Link
    public UniversalController controller;

    // Health Variables
    private int _fullHealth;
    private int _currentHealth;

    // Damage Draining Variables
    private int currentDrainingDamage;
    private int timeLeftDamageDrain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Upon game initilisation, the universal controller is grabbed from the scope. This allows us to then grab the fullHealth and currentHealth variables
        // from the player controller's initial state, using the universal controller as an intermediate.
        controller = GetComponent<UniversalController>();
        _fullHealth = controller._fullHealth;
        _currentHealth = controller._currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void TakeDamage(int damageDealt) { }
    public void HealDamage(int damageHealed) { }
    public void ResetHealth() { }
    public void StartDrain(int drainDamageAmount, int drainTimeLeft) { }
    public void ChangeDrainAmount(int newDrainDamageAmount, int newDrainTimeLeft) { }
    public void EndDrain() { }
}
