using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;
using static UnityEngine.UI.Image;

public class TouchingDirections : MonoBehaviour
{
    // Script + Component Links
    public ContactFilter2D castFilter;
    public Collider2D collider;
    public Animator animator;
    Collider2D wallDetectionZone;

    // Customizable Variables
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;

    // Internal Logic Variables
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    private Vector2 wallCheckDirection;
    private float castDistance = 0.1f;
    private RaycastHit2D[] hits = new RaycastHit2D[5];

    // Private variables/objects for filter
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[16];

    // Collision States
    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; animator.SetBool("isGrounded", value); } }
    [SerializeField]
    private bool isGrounded = false;
    public bool IsOnWall { get { return isOnWall; } set { isOnWall = value; animator.SetBool("isOnWall", value); } }
    [SerializeField]
    private bool isOnWall = false;
    public bool IsOnCeiling { get { return isOnCeiling; } set { isOnCeiling = value; animator.SetBool("isOnCeiling", value); } }
    [SerializeField]
    private bool isOnCeiling = false;
    public bool WallStop { get { return wallStop; } set { wallStop = value; } }
    [SerializeField]
    private bool wallStop = false;

    private void Awake() 
    {
        // Grabs all linked scripts + components
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        wallDetectionZone = transform.Find("WallDetectionZone").GetComponent<Collider2D>();

        // Sets up filter for collisions with walls
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        int layerIndex = LayerMask.NameToLayer("Collidable");
        filter.layerMask = 1 << layerIndex;
    }

    public void CheckCollisions()
    {
        // Checks which direction to check walls for
        wallCheckDirection = collider.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        IsGrounded = collider.Cast(Vector2.down, filter, hits, castDistance) > 0;
        IsOnWall = collider.Cast(Vector2.right * transform.localScale.x, filter, hits, castDistance) > 0;
        IsOnCeiling = collider.Cast(Vector2.up, filter, hits, castDistance) > 0;

        // Detects if colliding will wall, if so, deletes self
        WallStop = wallDetectionZone.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;

        Debug.Log("Ground hits: " + collider.Cast(Vector2.down, castFilter, hits, 0.1f));
    }
}