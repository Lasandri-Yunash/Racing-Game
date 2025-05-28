using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCar : MonoBehaviour
{
   
   [Header("Car Engine")]
   public float maxSpeed;
   public float currentSpeed;
   public float acceleration = 1f;
   public float turningSpeed = 30f;
   public float breakSpeed = 12f;

   [Header("Destination Var")]
   public Vector3 destination;
   public bool destinationReached;

   private Rigidbody rb;
    
    [Header("Respawn")]
    public float respawnTimer = 0f;
    public const float respawnTimeThreshold = 10f;

    [Header("Lap")]
    public int maxLaps;
    public int currentLap;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        maxLaps = FindObjectOfType<LapSystem>().maxLaps;

    }

    void Update()
    {
        Drive();

        if(!destinationReached)
        {
            respawnTimer += Time.deltaTime;

            if(respawnTimer >= respawnTimeThreshold)
            {
                //respawn the car
                RespawnAtDestination();
            }
        }
        else
        {
            respawnTimer = 0f;
        }
    }

    public void Drive()
    {
        if(!destinationReached)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;
            float destinationDistance = destinationDirection.magnitude;

            if(destinationDistance >= breakSpeed)
            {
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);

                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration* Time.deltaTime);

                rb.velocity = transform.forward * currentSpeed;
            }
            else
            {
                destinationReached = true;
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void RespawnAtDestination()
    {
        respawnTimer = 0f;
        currentSpeed = 5f;

        transform.position = destination;
        destinationReached = false;
    }

    public void LocateDestination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;
    }

    public void ResetAcceleration()
    {
        currentSpeed = Random.Range(38f, 46f);
        acceleration = Random.Range(3.5f,5f);
    }

    public void IncreaseLap()
    {
        currentLap++;
        Debug.Log(gameObject.name + " Lap:" + currentLap);
    }
}
