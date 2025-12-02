using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectile;
    private ProjectileScript projectileController;

    public void SpawnProjectile()
    {
        float angle = 0f;
        if (transform.localScale.x < 0)
        {
            angle = 135f;
        }
        else
        {
            angle = -45f;
        }
        GameObject firedProjectile = Instantiate(projectile, new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z), Quaternion.Euler(0, 0, angle));

        projectileController = firedProjectile.GetComponent<ProjectileScript>();
        if (angle == 135f) { projectileController.FlipDirection(); }

        if (gameObject.tag == "Enemy")
        {
            projectileController.AssignOwner(gameObject);
        }
    }
}
