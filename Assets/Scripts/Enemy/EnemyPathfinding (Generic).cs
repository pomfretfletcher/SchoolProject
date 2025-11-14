using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    // Enemy Controller and Player Links
    public EnemyController controller;
    public Object player;

    // Waypoint Variables
    private Transform _nextWaypoint;
    private List<Transform> _waypointList;

    // Distance Variables
    public float distanceToPlayer;
    private int _playerRequiredProximity;

    // Cooldown Variables
    private float _attackCooldown;
    
    // Movement Variables
    private int _maxSpeed;
    private float _currentSpeed;

    // Dictionaries storing information necessary for cooldown functions. Each cooldown has a shared key among these three dicts, with the cooldown length,
    // current time progressed and timer status stored in their respective dicts.
    Dictionary<string, float> cooldownDict = new Dictionary<string, float>();
    Dictionary<string, float> timerDict = new Dictionary<string, float>();
    Dictionary<string, int> timerStatusDict = new Dictionary<string, int>();

    // Additional variables used for handling cooldown timers
    // Used for knowing how many cooldowns to loop through
    private int cooldownCount;
    // Stores the keys of the dictionaries, in order to be able to loop through
    public List<string> cooldownKeys;
    // Stores the current key being used to access dictionaries
    private string currentKey;

    // Movement States
    public bool CanMove { get; private set; }
    public bool CanAttack { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
            // Movement Variables
        _nextWaypoint = controller.nextWaypoint;
        _waypointList = controller.waypointList;
        _playerRequiredProximity = controller.playerRequiredProximity;
        _maxSpeed = controller.maxSpeed;
        _currentSpeed = controller.currentSpeed;
            // Cooldown Variables
        _attackCooldown = controller.attackCooldown;
        
        // Add cooldown variables to list and creates corresponding timer
        cooldownDict.Add("attackCooldown", _attackCooldown); timerDict.Add("attackCooldown", 0); timerStatusDict.Add("attackCooldown", 0); cooldownKeys.Add("attackCooldown"); cooldownCount += 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalcDistanceToPlayer() { }
    public void FindOptimalMovePath() { }
    public void DecideNextWaypoint() { }
    public void MoveEnemy() { }
    public void ExecuteEnemyAttack() { }
}
