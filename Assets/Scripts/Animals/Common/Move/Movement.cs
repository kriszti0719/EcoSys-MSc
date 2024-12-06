using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;

    public bool isPassive;
    public bool isWandering = false;
    public bool isRotatingLeft = false;
    public bool isRotatingRight = false;
    public bool isWalking = false;

    // Define the minimum and maximum Y positions where the bunnies can wander
    public float minY = 21f;
    public Animal animal;
    public bool isDying = false;

    public bool isTurningAway;

    private Coroutine wanderingCoroutine; // Coroutine referencia

    private float walkTime;
    private int rotateDir;
    private float rotAngle;
    private float rotTime;

    public int passedSeconds = 0;

    //Flags 
    private bool wanderInitialized = false;
    private bool turningAwayEnded = true;
    private bool turningEnded = true;

    public void StartMoving()
    {
        wanderingCoroutine = null;
        animal = GetComponent<Animal>();
        rotSpeed = moveSpeed * 30;
    }
    private void Update()
    {
        if (animal.status == Status.WANDER || 
                animal.status == Status.SEARCH_FOOD ||
                animal.status == Status.SEARCH_DRINK ||
                animal.status == Status.SEARCH_MATE)
        {
            Wandering();
        }
        else if (animal.status == Status.MOVE_TOWARDS)
        {
            if (wanderingCoroutine != null)
                StopWander();            
            MovingTowards();
        }
        else if (animal.status == Status.DIE && !isDying)
        {
            if (wanderingCoroutine != null)
                StopWander();
            isDying = true;
            StartCoroutine(Die());
        }
    }
    private void Wandering()
    {
        if (!isWandering)
        {
            if (wanderingCoroutine != null)
                Debug.Log("RIP");
            wanderingCoroutine = StartCoroutine(Wander());
            //isWandering = true;
        }
        if (isRotatingRight)
        {
            transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }
        if (isRotatingLeft)
        {
            transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        }
        if (isWalking)
        {
            if (animal.transform.localPosition.y > 22 || isTurningAway)
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            else
            {
                isTurningAway = true;
                isWalking = false;
                RestartWander();               
            }
        }
    }
    private void StopWander()
    {
        StopCoroutine(wanderingCoroutine);
        wanderingCoroutine = null;

        isRotatingLeft = false;
        isRotatingRight = false;
        isWandering = false;
        isTurningAway = false;
    }
    private void RestartWander()
    {
        StopCoroutine(wanderingCoroutine);
        isWandering = false;
        isRotatingLeft = false;
        isRotatingRight = false;
        isWandering = false;
        wanderingCoroutine = StartCoroutine(Wander()); 
        //isWandering = true;
    }
    IEnumerator Wander()
    {
        walkTime = Random.Range(5,7);
        rotateDir = Random.Range(0, 2); 
        rotAngle =  Random.Range(10f, 180);

       isWandering = true;

        if(isTurningAway)
        {
            rotAngle = Random.Range(90f, 140f);
            rotTime = rotAngle / rotSpeed;
            
            isRotatingLeft = true;
            
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
            rotateDir = Random.Range(0, 2);
            rotAngle = Random.Range(10f, 50f);
            walkTime = Random.Range(5, 10);
        }
        rotTime = rotAngle / rotSpeed;

        //Walking
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;

        if (isTurningAway)
        {
            isTurningAway = false;
        }

        //Turning
        rotateDir = Random.Range(0, 2);
        if (rotateDir == 0)
        {
            isRotatingLeft = true;
        }
        else
        {
            isRotatingRight = true;
        }
        yield return new WaitForSeconds(rotTime);
        isRotatingLeft = false;
        isRotatingRight = false;

        isWandering = false;
        wanderingCoroutine = null;
    }
    private void StartWander()
    {
        walkTime = Random.Range(5, 7);
        rotateDir = Random.Range(0, 2);
        rotAngle = Random.Range(10f, 180);

        wanderInitialized = true;
    }
    private void StartWalking()
    {
        isWalking = true;
    }
    private void StopWalking()
    {
        isWalking = false;
    }
    private void StartTurning()
    {
        rotateDir = Random.Range(0, 2);
        if (rotateDir == 0)
        {
            isRotatingLeft = true;
        }
        else
        {
            isRotatingRight = true;
        }
        turningEnded = false;
    }
    private void StopTurning()
    {
        isRotatingLeft = false;
        isRotatingRight = false;

        isWandering = false;
        wanderingCoroutine = null;

        turningEnded = true;
    }
    private void StartTurningAway()
    {
        rotAngle = Random.Range(90f, 140f);
        rotTime = rotAngle / rotSpeed;

        isRotatingLeft = true;

        turningAwayEnded = false;
    }
    private void StopTurningAway()
    {
        isRotatingLeft = false;
        rotateDir = Random.Range(0, 2);
        rotAngle = Random.Range(10f, 50f);
        walkTime = Random.Range(5, 10);
        rotTime = rotAngle / rotSpeed;

        turningAwayEnded = true;
    }
    public void WanderTo()
    {
        if (!wanderInitialized)
        {
            StartWander();

            if (isTurningAway)
            {
                StartTurningAway();
            }
            else
            {
                rotTime = rotAngle / rotSpeed;
                StartWalking();
            }
        }
        else if (!turningAwayEnded && passedSeconds >= rotTime * 10)
        {
            StopTurningAway();
            StartWalking();
        }
        else if (isWalking && (isTurningAway && passedSeconds >= (rotTime + walkTime) * 10) || (!isTurningAway && passedSeconds >= (walkTime) * 10))
        {
            StopWalking();
            if (isTurningAway)
            {
                isTurningAway = false;
            }
            StartTurning();
        }
        else if (!turningEnded && (isTurningAway && passedSeconds >= (rotTime + walkTime + rotTime) * 10) || (isTurningAway && passedSeconds >= (walkTime + rotTime) * 10))
        {
            StopTurning();
            passedSeconds = 0;
            isTurningAway = false;
        }
        passedSeconds++;
    }
    public void ChangeDirection(float amount, float all)
    {
        float direction = 360 / amount * all;
        transform.Rotate(transform.up, -(direction));
    }
    private void MovingTowards()
    {
        if (animal.targetRef != null)
        {
            Vector3 directionToTarget = animal.targetRef.transform.position - transform.position;
            directionToTarget.y = 0; // Keep the movement in the horizontal plane

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            float distanceToTarget = Vector3.Distance(transform.position, animal.targetRef.transform.position);
            if (distanceToTarget < 3f)
            {
                isWalking = false;
            }
            else
            {
                isWalking = true;
            }
        }
    }
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);

        animal.barsContainer.transform.SetParent(null);

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 90f);
        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        animal.die.Destroy();
    }
}
