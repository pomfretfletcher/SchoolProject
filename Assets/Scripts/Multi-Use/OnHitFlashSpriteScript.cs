using UnityEngine;

public class OnHitFlashSpriteScript : MonoBehaviour
{
    Transform parentObject;
    SpriteRenderer flashOverlay;
    SpriteRenderer parentRenderer;

    private void Awake()
    {
        parentObject = transform.parent;
        flashOverlay = GetComponent<SpriteRenderer>();
        parentRenderer = parentObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        flashOverlay.sprite = parentRenderer.sprite;
    }
}
