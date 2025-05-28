using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum CarType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }
    public CarType carType = CarType.FourWheelDrive;

    public enum ControlMode
    {
        Keyboard,
        Button
    };

    public ControlMode control;

    [Header("Wheel GameObject Meshes")]
    public GameObject FrontWheelLeft;
    public GameObject FrontWheelRight;
    public GameObject BackWheelLeft;
    public GameObject BackWheelRight;

    [Header("WheelCollider")]
    public WheelCollider FrontWheelLeftCollider;
    public WheelCollider FrontWheelRightCollider;
    public WheelCollider BackWheelLeftCollider;
    public WheelCollider BackWheelRightCollider;
    

    [Header("Movement, Steering, and Breaking")]
    private float currentSpeed;
    public float maximumMotorTorque;
    public float maximumSteeringAngle= 20f;
    public float maximumSpeed;
    public float breakPower;
    public Transform COM;
    float carSpeed;
    float carSpeedConverter;
    float motorTorque;
    float tireAngle;
    float vertical = 0f;
    float horizontal = 0f;
    bool  handBrake = false;
    Rigidbody carRigidBody;


    [Header("Sounds and Effects")]
    public ParticleSystem[] smokeEffects;
    private bool smokeEffectEnabled;

    public AudioSource engineSound;
    public AudioClip engineClip;

    [Header("Lap")]
    public int maxLaps;
    public int currentLap;


    void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
        if(carRigidBody != null)
        {
            carRigidBody.centerOfMass = COM.localPosition;
        }

        engineSound.loop = true;
        engineSound.playOnAwake = false;
        engineSound.volume = 0.5f;
        engineSound.pitch = 1f;

        engineSound.Play();
        engineSound.Pause();

        maxLaps = FindObjectOfType<LapSystem>().maxLaps;
    }

    void Update()
    {
        GetInputs();
        CalculateCarMovement();
        CalculateSteering();
        ApplyTransformToWheels();
    }

    void GetInputs()
    {
        if(control == ControlMode.Keyboard)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
    }

    void CalculateCarMovement()
    {
        carSpeed = carRigidBody.velocity.magnitude;
        carSpeedConverter = Mathf.Round(carSpeed*3.6f);

        if(Input.GetKey(KeyCode.Space))
            handBrake = true;
        else
            handBrake = false;
        
        if(handBrake)
        {
            motorTorque = 0;
            ApplyBrake();
            if(!smokeEffectEnabled)
            {
                EnableSmokeEffect(true);
                smokeEffectEnabled = true;
            }
        }
        else
        {
            ReleaseBrake();

            if(carSpeedConverter < maximumSpeed)
                motorTorque = maximumMotorTorque * vertical;
            else
                motorTorque = 0; 
            if(smokeEffectEnabled)
            {
                EnableSmokeEffect(false);
                smokeEffectEnabled = false;
            }  

            if(carSpeedConverter > 0 || handBrake)
            {
                engineSound.UnPause();

                float gearRatio = currentSpeed / maximumSpeed;
                int numberOfGears = 6;
                int currentGear = Mathf.Clamp(Mathf.FloorToInt(gearRatio * numberOfGears) + 1, 1, numberOfGears);

                float pitchMultiplier = 0.5f + 0.5f * (carSpeedConverter / maximumSpeed);
                float volumeMultipiler = 0.5f + 0.8f * (carSpeedConverter / maximumSpeed);

                engineSound.pitch = Mathf.Lerp(0.5f, 1.0f, pitchMultiplier) * currentGear;
                engineSound.volume = volumeMultipiler;
            }
            else
            {
                engineSound.UnPause();
                engineSound.pitch = 0.5f;
                engineSound.volume = 0.2f;
            }

        }

        ApplyMotorTorque();
    }


    void CalculateSteering()
    {
        tireAngle = maximumSteeringAngle * horizontal;
        FrontWheelLeftCollider. steerAngle = tireAngle;
        FrontWheelRightCollider. steerAngle = tireAngle;
    }    

    void ApplyMotorTorque()
    {
        if(carType == CarType.FrontWheelDrive)
        {
            FrontWheelLeftCollider.motorTorque = motorTorque;
            FrontWheelRightCollider.motorTorque = motorTorque;
        }
        else if(carType == CarType.RearWheelDrive)
        {
            BackWheelLeftCollider.motorTorque = motorTorque;
            BackWheelRightCollider.motorTorque = motorTorque;
        }
        else if(carType == CarType.FourWheelDrive)
        {
            FrontWheelLeftCollider.motorTorque = motorTorque;
            FrontWheelRightCollider.motorTorque = motorTorque;
            BackWheelLeftCollider.motorTorque = motorTorque;
            BackWheelRightCollider.motorTorque = motorTorque;
        }
    }

    void ApplyBrake()
    {
        FrontWheelLeftCollider.brakeTorque = breakPower;
        FrontWheelRightCollider.brakeTorque = breakPower;
        BackWheelLeftCollider.brakeTorque = breakPower;
        BackWheelRightCollider.brakeTorque = breakPower;
    }

    void ReleaseBrake()
    {
        FrontWheelLeftCollider.brakeTorque = 0;
        FrontWheelRightCollider.brakeTorque = 0;
        BackWheelLeftCollider.brakeTorque = 0;
        BackWheelRightCollider.brakeTorque = 0;
    }


    public void ApplyTransformToWheels()
    {
        Vector3 position;
        Quaternion rotation;

        FrontWheelLeftCollider.GetWorldPose(out position, out rotation);
        FrontWheelLeft.transform.position = position;
        FrontWheelLeft.transform.rotation = rotation;

        FrontWheelRightCollider.GetWorldPose(out position, out rotation);
        FrontWheelRight.transform.position = position;
        FrontWheelRight.transform.rotation = rotation;

        BackWheelLeftCollider.GetWorldPose(out position, out rotation);
        BackWheelLeft.transform.position = position;
        BackWheelLeft.transform.rotation = rotation;

        BackWheelRightCollider.GetWorldPose(out position, out rotation);
        BackWheelRight.transform.position = position;
        BackWheelRight.transform.rotation = rotation;
    }

    private void EnableSmokeEffect(bool enable)
    {
        foreach(ParticleSystem smokeEffect in smokeEffects)
        {
            if(enable)
            {
                smokeEffect.Play();
            }
            else
            {
                smokeEffect.Stop();
            }
        }
    }

    public void IncreaseLap()
    {
        currentLap++;
        Debug.Log(gameObject.name + " Lap:" + currentLap);
    }

}
