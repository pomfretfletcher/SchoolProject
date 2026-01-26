using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    // Script + Component Links
    private ProjectileScript projectileController;

    // Fired Projectile - Assigned in inspector
    public GameObject projectile;
    
    // Internal Logic Variables
    private float angle;
    private float offset;

    public void SpawnProjectile()
    {
        // Used to make sure projectile is aligned properly based on fire direction
        if (transform.localScale.x < 0)
        {
            angle = 135f;
            offset = -0.5f;
        }
        else
        {
            angle = -45f;
            offset = 0.5f;
        }

        // Creates the firing projectile
        GameObject firedProjectile = Instantiate(projectile, new Vector3(transform.position.x + offset, transform.position.y - 0.1f, transform.position.z), Quaternion.Euler(0, 0, angle));

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