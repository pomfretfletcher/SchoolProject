using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    // Script + Component Links
    private ProjectileScript projectileController;

    // Projectile to be fired, able to be assigned in inspector
    public GameObject projectile;
    
    // Private internal logic variables
    private float angle;

    public void SpawnProjectile()
    {
        // Used to make sure projectile is aligned properly based on fire direction
        if (transform.localScale.x < 0)
        {
            angle = 135f;
        }
        else
        {
            angle = -45f;
        }

        // Creates the firing projectile
        GameObject firedProjectile = Instantiate(projectile, new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z), Quaternion.Euler(0, 0, angle));

        // Flips projectile if needed
        projectileController = firedProjectile.GetComponent<ProjectileScript>();
        if (angle == 135f) { projectileController.FlipDirection(); }

        // Needed for enemy collision logic
        if (gameObject.tag == "Enemy")
        {
            projectileController.AssignOwner(gameObject);
        }
    }
}
