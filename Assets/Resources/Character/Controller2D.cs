// File: Controller2D.cs
// Author: Brendan Robinson
// Date Created: 06/11/2018
// Date Last Modified: 07/15/2018
// Description: 

using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour
{
    protected const float skinWidth = 0.015625f;

    public Transform objTransform;
    public CollisionInfo collisions;
    protected LayerMask platformCollisionMask;
    protected LayerMask noPlatformCollisionMask;

    public int horizontalRayCount = 8;
    public int verticalRayCount = 8;
    [HideInInspector] public new BoxCollider2D collider;
    public bool ignorePlatform;
    public float maxFallSpeed = 100;


    public List<PolygonCollider2D> colliders = new List<PolygonCollider2D>();
    public List<Int2> colPositions = new List<Int2>();

    protected float width;
    protected float height;
    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;
    protected float maxDescendAngle = 50f;
    protected float maxClimbAngle = 89.5f;
    protected float gravity = 6f;
    protected float jumpVelocity;
    protected float velocityXSmoothing;
    protected bool facingRight = true;
    protected Animator animator;
    protected RaycastOrigins raycastOrigins;
    protected World world;
    protected Int2 blockPosition = Int2.zero;
    protected Int2 oldBlockPosition = Int2.one;
    protected LayerMask currentCollisionMask;
    protected Vector2 velocity;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        world = GameManager.Get<WorldManager>().currentWorld;
        platformCollisionMask = (1 << 18) | (1 << 20);
        noPlatformCollisionMask = (1 << 20);

        currentCollisionMask = platformCollisionMask;

        width = collider.size.x;
        height = collider.size.y;
    }

    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle;
        public Vector3 velocityOld;
        public float slopeAngleOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    #region Public Methods



    public virtual void Move(Vector2 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;
        UpdateColliders(velocity);
        if (velocity.y < 0) DescendSlope(ref velocity);

        if (velocity.x != 0) HorizontalCollisions(ref velocity);
        if (velocity.y != 0) VerticalCollisions(ref velocity);
        objTransform.Translate(velocity);
    }

    #endregion

    #region Movement Calculations

    /*
    protected virtual void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            BlockHit hit = world.HorizontalCollision(rayOrigin, directionX * rayLength);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit.hit)
            {

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle == 45)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;
                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }
    */
    private void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit =
                Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, noPlatformCollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    private void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        if (ignorePlatform || directionY == 1) currentCollisionMask = noPlatformCollisionMask;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, currentCollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = (directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
                                Vector2.up * velocity.y;
            RaycastHit2D hit =
                Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, currentCollisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }

        currentCollisionMask = platformCollisionMask;
    }

    private void ClimbSlope(ref Vector2 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    private void DescendSlope(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, noPlatformCollisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
                if (Mathf.Sign(hit.normal.x) == directionX)
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
        }
    }

    #endregion

    #region RayInfo

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    #endregion

    protected void UpdateColliders(Vector2 velocity)
    {
        blockPosition = new Int2((int)(transform.position.x + velocity.x),
            (int)(transform.position.y + velocity.y));

        if (blockPosition.x != oldBlockPosition.x || blockPosition.y != oldBlockPosition.y)
        {
            oldBlockPosition = blockPosition;
            foreach (Int2 pos in colPositions) GameManager.Get<ChunkManager>().DeleteCollider(pos);
            colPositions.Clear();

            for (int x = (int)(transform.position.x + velocity.x - width / 2f - 1);
                x <= (int)(transform.position.x + velocity.x + width / 2f + 1);
                x++)
                for (int y = (int)(transform.position.y + velocity.y - 1);
                    y <= (int)(transform.position.y + velocity.y + height + 1);
                    y++)
                {
                    colPositions.Add(new Int2(x, y));
                    GameManager.Get<ChunkManager>().AddCollider(new Int2(x, y));
                }
        }
    }
}