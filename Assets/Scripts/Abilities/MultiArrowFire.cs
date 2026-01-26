using UnityEngine;

public class MultiArrowFire : MonoBehaviour, IsAbility
{
    // Script + Component Links
    ProjectileScript projectileController;
    GameObject player;

    // Customizable Values
    public int arrowCount;
    public float fireRange;

    // The arrow that will be fired - assigned in inspector
    public GameObject projectile;

    // Private internal logic variables
    private float angle;

    private void Awake()
    {
        // Grabs all linked scripts + components
        player = GameObject.Find("Player");
    }

    public void OnActivation() 
    { 
        FireArrows(); 
    }
    
    public void FireArrows()
    {
        // Used to make sure arrow is aligned properly based on fire direction
        if (player.transform.localScale.x < 0)
        {
            angle = 135f;
        }
        else
        {
            angle = -45f;
        }

        // Decides the position for the first arrow to be fired from
        float currentFirePosition = (float)(player.transform.position.y + 0.5 * fireRange);
        for (var i = 1; i <= arrowCount; i++)
        {
            // Creates the firing projectile
            GameObject firedProjectile = Instantiate(projectile, new Vector3(player.transform.position.x + 0.5f, currentFirePosition, player.transform.position.z), Quaternion.Euler(0, 0, angle));
            
            // Seperates each arrow evenly across the fire range
            currentFirePosition -= (fireRange / (arrowCount - 1));
            
            // Flips fired projectile if needed
            if (angle == 135f) 
            { 
                projectileController = firedProjectile.GetComponent<ProjectileScript>();
                projectileController.FlipDirection(); 
            }
        }
    }
}
