using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    // Script + Component Links
    public ContactFilter2D castFilter;
    public Collider2D collider;
    public Animator animator;

    // Customizable Variables
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;

    // Internal Logic Variables
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    private Vector2 wallCheckDirection;

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

    private void Start() 
    {
        // Grabs all linked scripts + components
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void CheckCollisions()
    {
        // Checks which direction to check walls for
        wallCheckDirection = collider.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Updates the states of the script that called touching directions
        IsGrounded = collider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = collider.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = collider.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;

        // Edge Cases
        if (IsOnCeiling) { IsGrounded = false; }
    }
}