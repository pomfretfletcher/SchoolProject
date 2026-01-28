using UnityEngine;

public class ConsumableBob : MonoBehaviour
{
    // Customizable Values
    public float amplitude = 0.25f;  
    public float frequency = 1.5f; 

    // Internal/External logic variables
    public Vector3 startPos;
    private float yOffset;

    private void Start()
    {
        // Store the start position of the consumable, used for the bob calculation
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}
