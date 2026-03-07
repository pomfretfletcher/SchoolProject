using UnityEngine;

public class ToxicEffectScript : MonoBehaviour, EffectScript
{
    // Script + Component Links
    SpriteRenderer renderer;
    GameData gameData;

    // Internal logic variables
    private float timeAlive;
    private float originalAlpha;

    // Customizable Values
    public float lifeTime;

    private void Awake()
    {
        // Grabs all linked scripts + components
        renderer = GetComponent<SpriteRenderer>();
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();

        // Set color values for logic
        Color col = renderer.color;
        originalAlpha = col.a * gameData.universalVisualEffectOpacity;
    }

    private void FixedUpdate()
    {
        // Fades the effect from original alpha
        timeAlive += Time.deltaTime;
        Color col = renderer.color;
        col.a = originalAlpha * 1 - (timeAlive / lifeTime);
        renderer.color = col;

        // Once light enough, delete effect
        if (col.a < 0.1)
        {
            Destroy(this.gameObject);
        }
    }

    public void HideEffect()
    {
        originalAlpha = 0;
    }
}
