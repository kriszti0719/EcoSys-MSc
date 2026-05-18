using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;
    public Animal animal;

    private Vector3 targetPosition;
    private bool hasTarget;
    private float wanderTimer;
    
    private const float MIN_WANDER_DIST = 5f;
    private const float MAX_WANDER_DIST = 15f;
    private const float WANDER_TIME_LIMIT = 10f;

    public void StartMoving()
    {
        animal = GetComponent<Animal>();
        rotSpeed = moveSpeed * 30f;
        PickNewWanderTarget();
    }

    private void Update()
    {
        if (animal.status == Status.DIE || animal.status == Status.CAUGHT || animal.status == Status.WAIT)
        {
            hasTarget = false;
            return;
        }

        if (animal.status == Status.MOVE_TOWARDS && animal.targetRef != null)
        {
            targetPosition = animal.targetRef.transform.position;
            hasTarget = true;
        }
        else if (animal.status == Status.WANDER || animal.status == Status.SEARCH_FOOD || 
                 animal.status == Status.SEARCH_DRINK || animal.status == Status.SEARCH_MATE)
        {
            UpdateWander();
        }
        else
        {
            hasTarget = false;
        }

        if (hasTarget)
        {
            MoveTowardsTarget();
        }
    }

    private void UpdateWander()
    {
        wanderTimer -= Time.deltaTime;
        
        // Pick new target if time is up, we reached the current one, or we accidentally went into water
        if (wanderTimer <= 0 || Vector3.Distance(transform.position, targetPosition) < 1.5f || transform.position.y < 21f)
        {
            PickNewWanderTarget();
        }
        hasTarget = true;
    }

    private void PickNewWanderTarget()
    {
        bool validPointFound = false;
        int attempts = 0;

        // Try to find a point on land (Island layer)
        int groundLayerMask = LayerMask.GetMask("Island");

        while (!validPointFound && attempts < 15)
        {
            attempts++;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float dist = Random.Range(MIN_WANDER_DIST, MAX_WANDER_DIST);
            
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;
            Vector3 potentialTarget = transform.position + offset;

            // Raycast down to check the height of the island at that point
            if (Physics.Raycast(new Vector3(potentialTarget.x, 100f, potentialTarget.z), Vector3.down, out RaycastHit hit, 200f, groundLayerMask))
            {
                if (hit.point.y >= 22f)
                {
                    targetPosition = hit.point;
                    validPointFound = true;
                }
            }
        }

        // Fallback: If no land found nearby, turn around and try a short distance
        if (!validPointFound)
        {
            targetPosition = transform.position - transform.forward * 5f;
        }

        wanderTimer = WANDER_TIME_LIMIT;
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Keep movement horizontal

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    public void ChangeDirection(float amount, float all)
    {
        // Spreads animals out during spawning
        float direction = 360 / amount * all;
        transform.Rotate(transform.up, -direction);
        
        // Pick a target in the new forward direction
        targetPosition = transform.position + transform.forward * 10f;
        wanderTimer = WANDER_TIME_LIMIT;
    }
}
