using UnityEngine;

public class ToxicEffectScript : MonoBehaviour
{
    SpriteRenderer renderer;

    private float timeAlive;
    private float originalAlpha;
    public float lifeTime;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        Color col = renderer.color;
        originalAlpha = col.a;
    }

    private void FixedUpdate()
    {
        timeAlive += Time.deltaTime;
        Color col = renderer.color;
        col.a = originalAlpha * 1 - (timeAlive / lifeTime);
        renderer.color = col;
    }
}
