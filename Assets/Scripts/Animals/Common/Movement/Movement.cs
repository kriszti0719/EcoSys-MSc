using UnityEngine;

public class Movement : MonoBehaviour
{
    [HideInInspector] public Animal animal;
    public float moveSpeed;
    public float rotSpeed;

    private Vector3 targetPosition;
    private bool hasTarget;
    private float wanderTimer;
    
    private const float MIN_WANDER_DIST = 5f;
    private const float MAX_WANDER_DIST = 15f;
    private const float WANDER_TIME_LIMIT = 10f;

    private void Start()
    {
        if (animal == null) animal = GetComponent<Animal>();
    }

    public void StartMoving()
    {
        PickNewWanderTarget();
    }

    private void Update()
    {
        if (animal == null) return;

        rotSpeed = moveSpeed * 30f;

        if (animal.status == Status.MOVE_TOWARDS && animal.targetRef != null)
        {
            targetPosition = animal.targetRef.transform.position;
            hasTarget = true;
        }
        else if (animal.status == Status.WANDER || 
                 animal.status == Status.SEARCH_FOOD || 
                 animal.status == Status.SEARCH_DRINK || 
                 animal.status == Status.SEARCH_MATE)
        {
            UpdateWander();
        }
        else if (animal.status == Status.FLEE)
        {
            UpdateFlee();
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

    private void UpdateFlee()
    {
        if (animal.spottedThreats == null || animal.spottedThreats.Count == 0)
        {
            hasTarget = false;
            return;
        }

        Vector3 weightedFleeVector = Vector3.zero;

        foreach (var threat in animal.spottedThreats)
        {
            if (threat == null) continue;

            Vector3 directionAway = transform.position - threat.transform.position;
            float distance = directionAway.magnitude;

            if (distance > 0)
            {
                weightedFleeVector += directionAway.normalized / distance;
            }
        }

        if (weightedFleeVector.sqrMagnitude > 0.001f)
        {
            targetPosition = transform.position + weightedFleeVector.normalized * 5f;
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
        }
    }

    private void UpdateWander()
    {
        wanderTimer -= Time.deltaTime;
        
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

        int groundLayerMask = LayerMask.GetMask("Island");

        float currentMinDist = MIN_WANDER_DIST;
        float currentMaxDist = MAX_WANDER_DIST;

        if (animal != null && animal.status == Status.WANDER && Random.value < 0.15f)
        {
            currentMinDist = 30f;
            currentMaxDist = 75f; 
        }

        while (!validPointFound && attempts < 15)
        {
            attempts++;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float dist = Random.Range(currentMinDist, currentMaxDist);
        
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;
            Vector3 potentialTarget = transform.position + offset;

            if (Physics.Raycast(new Vector3(potentialTarget.x, 100f, potentialTarget.z), Vector3.down, out RaycastHit hit, 200f, groundLayerMask))
            {
                if (hit.point.y >= 22f)
                {
                    targetPosition = hit.point;
                    validPointFound = true;
                }
            }
        }

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
            
            // Use much faster rotation speed when actively targeting or fleeing
            float currentRotSpeed = rotSpeed;
            if (animal.status == Status.MOVE_TOWARDS || animal.status == Status.FLEE)
            {
                currentRotSpeed *= 5f; // 5x faster turn when focused
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentRotSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    public void ChangeDirection(float amount, float all)
    {
        float direction = 360 / amount * all;
        transform.Rotate(transform.up, -direction);
        
        targetPosition = transform.position + transform.forward * 10f;
        wanderTimer = WANDER_TIME_LIMIT;
    }
}
