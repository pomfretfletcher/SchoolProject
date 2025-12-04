using UnityEngine;

public class MultiArrowFire : MonoBehaviour, IsAbility
{
    public GameObject projectile;
    private ProjectileScript projectileController;
    GameObject player;
    
    void Awake()
    {
        player = GameObject.Find("Player");
    }

    public bool OnActivation()
    {
        SpawnProjectile();
        return false;
    }
    
    public void SpawnProjectile()
    {
        float angle = 0f;
        if (player.transform.localScale.x < 0)
        {
            angle = 135f;
        }
        else
        {
            angle = -45f;
        }
        GameObject firedProjectile = Instantiate(projectile, new Vector3(player.transform.position.x + 0.3f, player.transform.position.y, player.transform.position.z), Quaternion.Euler(0, 0, angle));

        projectileController = firedProjectile.GetComponent<ProjectileScript>();
        if (angle == 135f) { projectileController.FlipDirection(); }
    }
}
