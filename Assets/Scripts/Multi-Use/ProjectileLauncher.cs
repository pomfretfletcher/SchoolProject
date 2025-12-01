using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectile;
    private ArrowScript projectileController;

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

        projectileController = firedProjectile.GetComponent<ArrowScript>();
        if (angle == 135f) { projectileController.moveSpeed *= -1; }
    }
}
