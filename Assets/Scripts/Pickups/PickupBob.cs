using UnityEngine;

public class PickupBob : MonoBehaviour
{
    //[Header("Bobbing Settings")]
    public float amplitude = 0.25f;   // How high it moves
    public float frequency = 1.5f;    // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}
