using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    // Enemy Controller Link
    public EnemyController controller;

    // Player Link
    public Object player;

    // Waypoint Variables
    private Transform _nextWaypoint;
    private List<Transform> _waypointList;

    // Distance Variables
    public float distanceToPlayer;
    private int _playerRequiredProximity;

    // Attack Variables
    private int _attackCooldown;
    private float timeSinceAttack;

    // Movement Variables
    private int _maxSpeed;
    private float _currentSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<EnemyController>();
        _nextWaypoint = controller.nextWaypoint;
        _waypointList = controller.waypointList;
        _playerRequiredProximity = controller.playerRequiredProximity;
        _attackCooldown = controller.attackCooldown;
        _maxSpeed = controller.maxSpeed;
        _currentSpeed = controller.currentSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalcDistanceToPlayer() { }
    public void FindOptimalMovePath() { }
    public void DecideNextWaypoing() { }
    public void MoveEnemy() { }
    public void ExecuteEnemyAttack() { }
}
