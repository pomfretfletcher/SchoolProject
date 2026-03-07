using UnityEngine;

public class RoomCreation : MonoBehaviour
{
    // Script + Component Links
    RoomData roomData;
    RoomLogic roomLogicScript;
    GameData gameData;

    // Internal Logic Variables
    private int currentEnemyCount = 0;
    private int currentPickupCount = 0;
    private int currentAbilityCount = 0;
    private int currentPowerupCount = 0;
    private bool createdSpecificEnemy = false;
    private bool createdSpecificPickup = false;
    private bool createdSpecificAbility = false;
    private bool createdSpecificPowerup = false;

    public void SetupRoom(string roomEnterDirection, float difficultyScale, string specificEnemyType, int enemyLimit, string specificPickupType, int pickupLimit, string specificAbilityType, int abilityLimit, string specificPowerupType, int powerupLimit)
    {
        // Grabs needed linked scripts + components
        roomData = GetComponent<RoomData>();
        roomLogicScript = GetComponent<RoomLogic>();
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();

        GameObject roomsFolder = GameObject.Find("RunParts/Rooms");
        this.gameObject.transform.parent = roomsFolder.transform;

        if (roomData.isStartingRoom)
        {
            // Since starting room is special as it has no pickups/enemies, we just call a specific function for entering it as it only has one node
            roomLogicScript.EnterStartingRoom();
        }
        else
        {
            // Place each component of the room
            PlaceEnemies(difficultyScale, specificEnemyType, enemyLimit);
            PlacePickups(specificPickupType, pickupLimit);
            PlaceAbilities(specificAbilityType, abilityLimit);
            PlacePowerups(specificPowerupType, powerupLimit);

            // Enter room after creation
            roomLogicScript.EnterNewRoom(roomEnterDirection, this.gameObject);
        }
    }

    private void PlaceEnemies(float difficultyScale, string specificEnemyType, int enemyLimit)
    {
        for (var i = 0; i < roomData.enemySpawnPositions.Count; i++)
        {
            // Grabs next node to spawn in
            Transform currentNode = roomData.enemySpawnPositions[i];

            // If we can spawn more, spawn more
            if (currentEnemyCount < enemyLimit)
            {
                // If we do not need to create a specific enemy or we have already created said specific enemy, a random enemy is made with the game handler
                if (specificEnemyType == null || createdSpecificEnemy)
                {
                    // Load all prefabs in the folder enemies
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Enemies");

                    // Pick one at random
                    GameObject chosenEnemy = prefabs[Random.Range(0, prefabs.Length)];

                    // Instantiate the chosen enemy
                    GameObject createdEnemy = Instantiate(chosenEnemy, currentNode.transform.position, Quaternion.identity);

                    // Setup enemy data
                    EnemyController enemyController = createdEnemy.GetComponent<EnemyController>();
                    enemyController.DifficultyScale(difficultyScale);
                    enemyController.SetWaypoints(currentNode.transform);    

                    // Parent enemy under this room
                    createdEnemy.transform.parent = this.gameObject.transform;

                    // Add to game data for storage
                    gameData.runEnemies.Add(createdEnemy);
                }
                else
                {
                    // Load all prefabs in the folder enemies
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Enemies");
                    GameObject chosenEnemy = prefabs[0];

                    // Loop through prefabs until found correct one
                    foreach (GameObject prefab in prefabs)
                    {
                        if (prefab.name == specificEnemyType)
                        {
                            chosenEnemy = prefab;
                        }
                    }
                    // Instantiate specific enemy
                    GameObject createdEnemy = Instantiate(chosenEnemy, currentNode.transform.position, Quaternion.identity);

                    // Setup enemy data
                    EnemyController enemyController = createdEnemy.GetComponent<EnemyController>();
                    enemyController.DifficultyScale(difficultyScale);
                    enemyController.SetWaypoints(currentNode.transform);

                    // Parent enemy under this room
                    createdEnemy.transform.parent = this.gameObject.transform;

                    // Flag to say no more of specific enemy needs to be made
                    createdSpecificEnemy = true;

                    // Add to game data for storage
                    gameData.runEnemies.Add(createdEnemy);
                }

                // Iterate to keep track of how many enemies made
                currentEnemyCount++;
            }
        }
    }

    private void PlacePickups(string specificPickupType, int pickupLimit) 
    {
        for (var i = 0; i < roomData.pickupSpawnPositions.Count; i++)
        {
            // Grabs next node to spawn in
            Transform currentNode = roomData.pickupSpawnPositions[i];

            // If we can spawn more, spawn more
            if (currentPickupCount < pickupLimit)
            {
                // If we do not need to create a specific pickup or we have already created said specific pickup, a random pickup is made with the game handler
                if (specificPickupType == null || createdSpecificPickup)
                {
                    // Load all prefabs in the folder pickups
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Pickups");

                    // Pick one at random
                    GameObject chosenPickup = prefabs[Random.Range(0, prefabs.Length)];

                    // Instantiate the chosen pickup
                    GameObject createdPickup = Instantiate(chosenPickup, currentNode.transform.position, Quaternion.identity);

                    // Parent pickup under this room
                    createdPickup.transform.parent = this.gameObject.transform;
                }
                else
                {
                    // Load all prefabs in the folder pickups
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Pickups");
                    GameObject chosenPickup = prefabs[0];
                    
                    // Loop through folder until find the correct pickup
                    foreach (GameObject prefab in prefabs)
                    {
                        if (prefab.name == specificPickupType) 
                        { 
                            chosenPickup = prefab; 
                        }
                    }
                    
                    // Instantiate the specific pickup
                    GameObject createdPickup = Instantiate(chosenPickup, currentNode.transform.position, Quaternion.identity);

                    // Parent pickup under this room
                    createdPickup.transform.parent = this.gameObject.transform;

                    // Flag to say no more of the specific pickup needs to be made
                    createdSpecificPickup = true;
                }

                // Iterate to keep track of how many pickups made
                currentPickupCount++;
            }
        }
    }

    private void PlaceAbilities(string specificAbilityType, int abilityLimit) 
    {
        for (var i = 0; i < roomData.abilitySpawnPositions.Count; i++)
        {
            // Grabs next node to spawn in
            Transform currentNode = roomData.abilitySpawnPositions[i];

            // If we can spawn more, spawn more
            if (currentAbilityCount < abilityLimit)
            {
                // If we do not need to create a specific ability or we have already created said specific ability, a random ability is made with the game handler
                if (specificAbilityType == null || createdSpecificAbility)
                {
                    // Load all prefabs in the folder abilities
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Abilities");

                    // Pick one at random
                    GameObject chosenAbility = prefabs[Random.Range(0, prefabs.Length)];

                    // Instantiate the chosen ability
                    GameObject createdAbility = Instantiate(chosenAbility, currentNode.transform.position, Quaternion.identity);

                    // Setup data for created ability
                    AbilityScript createdAbilityScript = createdAbility.GetComponent<AbilityScript>();
                    createdAbilityScript.SetToConsumableMode(currentNode.transform.position);

                    // Parent ability to this room
                    createdAbility.transform.parent = this.gameObject.transform;
                }
                else
                {
                    // Load all prefabs in the folder abilities
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Abilities");
                    GameObject chosenAbility = prefabs[0];

                    // Loop through folder until we find the correct ability
                    foreach (GameObject prefab in prefabs)
                    {
                        if (prefab.name == specificAbilityType)
                        {
                            chosenAbility = prefab;
                        }
                    }

                    // Instantiate the chosen ability
                    GameObject createdAbility = Instantiate(chosenAbility, currentNode.transform.position, Quaternion.identity);

                    // Setup data for created ability
                    AbilityScript createdAbilityScript = createdAbility.GetComponent<AbilityScript>();
                    createdAbilityScript.SetToConsumableMode(currentNode.transform.position);

                    // Parent ability to this room
                    createdAbility.transform.parent = this.gameObject.transform;

                    // Flag to say no more of specific ability needs to be made
                    createdSpecificAbility = true;
                }

                // Iterate to keep track of how many abilities made
                currentAbilityCount++;
            }
        }
    }

    private void PlacePowerups(string specificPowerupType, int powerupLimit) 
    {
        for (var i = 0; i < roomData.powerupSpawnPositions.Count; i++)
        {
            // Grabs next node to spawn in
            Transform currentNode = roomData.powerupSpawnPositions[i];

            // If we can spawn more, spawn more
            if (currentPowerupCount < powerupLimit)
            {
                // If we do not need to create a specific powerup or we have already created said specific powerup, a random powerup is made with the game handler
                if (specificPowerupType == null || createdSpecificPowerup)
                {
                    // Load all prefabs in the folder Powerups
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Powerups");

                    // Pick one at random
                    GameObject chosenPowerup = prefabs[Random.Range(0, prefabs.Length)];

                    // Instantiate the chosen powerup
                    GameObject createdPowerup = Instantiate(chosenPowerup, currentNode.transform.position, Quaternion.identity);

                    // Parent the powerup to this room
                    createdPowerup.transform.parent = this.gameObject.transform;
                }
                else
                {
                    // Load all prefabs in the folder Powerups
                    GameObject[] prefabs = Resources.LoadAll<GameObject>("Powerups");
                    GameObject chosenPowerup = prefabs[0];

                    // Loop through folder until find the correct powerup
                    foreach (GameObject prefab in prefabs)
                    {
                        if (prefab.name == specificPowerupType)
                        {
                            chosenPowerup = prefab;
                        }
                    }

                    // Instantiate the chosen powerup
                    GameObject createdPowerup = Instantiate(chosenPowerup, currentNode.transform.position, Quaternion.identity);

                    // Parent the powerup to this room
                    createdPowerup.transform.parent = this.gameObject.transform;

                    // Flag to say no more of specific powerup needs to be made
                    createdSpecificPowerup = true;
                }

                // Iterate to keep track of how many Powerups made
                currentPowerupCount++;
            }
        }
    }
}