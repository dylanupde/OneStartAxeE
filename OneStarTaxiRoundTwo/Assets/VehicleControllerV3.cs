using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControllerV3 : MonoBehaviour
{
    [Tooltip("Don't fuck with dis. It's where the passenger will sit")]
    public Transform passengerLocation;
    [Tooltip("Don't fuck with dis. It's all the wheel colliders")]
    [SerializeField] SphereCollider[] wheelColls;
    [Tooltip("Don't fuck with dis. I use it to set the rigidbody's center of mass wherever it is")]
    [SerializeField] Transform centerOfMassTransform;
    [Tooltip("This material gets assigned to the wheels when the player is holding down a move button")]
    [SerializeField] PhysicMaterial zeroFrictionMaterial;
    [Tooltip("This material gets assigned to the wheels when the player isn't holding down a move button. Too high friction and the car will fuckin flip")]
    [SerializeField] PhysicMaterial highFrictionMaterial;
    [Tooltip("The material that flashes when the player isn't ready to butt boost")]
    [SerializeField] Material dullMaterial;
    [Tooltip("How much AddForce we get when pressing a move key")]
    [SerializeField] float acceleration = 50f;
    [Tooltip("How quickly we LOOK like we're turning. Fucking with this won't actually make us any more maneuverable")]
    [SerializeField] float turnSpeed = 1f;
    [Tooltip("When we're going faster than this, pressing a key won't actually add any more force.")]
    [SerializeField] float maxSpeed = 9999999f;
    [Tooltip("Don't fuck with dis. It's how far downwards the wheels check to see if they're actually touching the ground.")]
    [SerializeField] float wheelGroundCheckDist = 0.1f;
    [Tooltip("To prevent rolling from side to side, if we're more than degreesOffToRotateBack degrees tilted, we rotate back at this speed")]
    [SerializeField] float rotateBackUpSpeed = 5f;
    [Tooltip("When we're this many degrees off side-to-side tilt, we start rotating back up.")]
    [SerializeField] float degreesOffToRotateBack = 30f;
    [Tooltip("Don't fuck with dis. It's how far down we test the angle of the ground")]
    [SerializeField] float groundTestDist = 0.5f;
    [Tooltip("How much instantaneous force our butt boost gives forward")]
    [SerializeField] float buttBoostForce = 5000f;
    [Tooltip("How much instantaneous force our butt boost gives upwards")]
    [SerializeField] float buttBoostUpForce = 500f;
    [Tooltip("How much our butt boost makes us randomly rotate")]
    [SerializeField] float buttBoostTorque = 500f;
    [Tooltip("How long after butt boosting we can butt boost again")]
    [SerializeField] float buttBoostCooldownTime = 3f;
    [Tooltip("MidairAcceleration")]
    [SerializeField] float midairAcceleration = 3f;

    [HideInInspector] public Transform currentPassengerTransform;

    Rigidbody rigidBody;
    MeshRenderer bodyMeshRenderer;
    PlayerInput playerInput;
    Material defaultMaterial;
    int layerMask;
    int frameCount = 0;
    int flashTime = 17;
    bool boostAllowed = true;
    

    void Awake()
    {
        GetComponent<PlayerInput>().myVehicleScript = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody>();
        bodyMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        defaultMaterial = bodyMeshRenderer.materials[0];
        rigidBody.centerOfMass = centerOfMassTransform.localPosition;
        layerMask = LayerMask.GetMask("Ground", "Wall");
        rigidBody.maxAngularVelocity = 20f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frameCount = (frameCount + 1) % (flashTime * 2);

        if (frameCount < flashTime && !boostAllowed)
        {
            bodyMeshRenderer.material = dullMaterial;
        }
        else
        {
            bodyMeshRenderer.material = defaultMaterial;
        }



        playerInput.GetCurrentFacingDirection();
        float numOfWheelsOnTheGround = HowManyWheelsAreOnTheGround();

        float currentAcceleration = acceleration;
        if (numOfWheelsOnTheGround < 2) currentAcceleration = midairAcceleration;
        
        if (playerInput.moveButtonIsPressed)
        {
            SetAllWheelsToPhysMat(zeroFrictionMaterial);

            Vector2 flattenedVelocityVector = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
            if (flattenedVelocityVector.magnitude <= maxSpeed)
            {

                RaycastHit raycastHit;
                if (Physics.Raycast(transform.position, -transform.up, out raycastHit, groundTestDist))
                {
                    Vector3 desiredMoveVector = (Vector3.ProjectOnPlane(playerInput.currentFacingVector, raycastHit.normal)).normalized;
                    rigidBody.AddForce(desiredMoveVector * currentAcceleration);
                }
                else
                {
                    rigidBody.AddForce(playerInput.currentFacingVector * currentAcceleration);
                }
            }
        }
        else
        {
            SetAllWheelsToPhysMat(highFrictionMaterial);
        }

        if (numOfWheelsOnTheGround >= 3) RotateToVelocityDirection();
        if (boostAllowed) RotateBackToNormal();

        playerInput.ResetPlayerInput();
    }




    private void SetAllWheelsToPhysMat(PhysicMaterial inputPhysMat)
    {
        foreach (SphereCollider thisColl in wheelColls)
        {
            thisColl.material = inputPhysMat;
        }
    }
    

    private void RotateToVelocityDirection()
    {
        Vector2 flattenedForwardVector = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 flattenedVelocityVector = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
        float angle = Vector2.SignedAngle(flattenedForwardVector, flattenedVelocityVector);

        if (flattenedVelocityVector.magnitude > 1.5f)
        {
            if (angle > 0f)
            {
                transform.Rotate(0f, Mathf.Clamp(-turnSpeed, -Mathf.Abs(angle), Mathf.Abs(angle)), 0f, Space.World);
            }
            else
            {
                transform.Rotate(0f, Mathf.Clamp(turnSpeed, -Mathf.Abs(angle), Mathf.Abs(angle)), 0f, Space.World);
            }
        }
    }


    private int HowManyWheelsAreOnTheGround()
    {
        int numOfWheelsOnTheGround = 0;

        foreach (SphereCollider thisColl in wheelColls)
        {
            RaycastHit raycastHit;

            if (Physics.SphereCast(thisColl.transform.position, (thisColl.radius * thisColl.transform.lossyScale.z) - (wheelGroundCheckDist * 0.25f), -transform.up, out raycastHit, wheelGroundCheckDist))
            {
                numOfWheelsOnTheGround++;
            }
        }

        return numOfWheelsOnTheGround;
    }



    private void RotateBackToNormal()
    {
        float adjustedRotZ = transform.localEulerAngles.z % 360f;
        if (adjustedRotZ < 0f) adjustedRotZ += 360f;

        if (adjustedRotZ >= 0f && adjustedRotZ < 180f)
        {
            if (adjustedRotZ > degreesOffToRotateBack)
            {
                //Debug.Log("I should rotate right cuz adjustedrotz is " + adjustedRotZ);
                transform.Rotate(0f, 0f, Mathf.Clamp(-rotateBackUpSpeed, -Mathf.Abs(adjustedRotZ), Mathf.Abs(adjustedRotZ)), Space.Self);
            }

        }
        else
        {
            float degreesAwayFromPerfection = 360 - adjustedRotZ;
            if (degreesAwayFromPerfection > degreesOffToRotateBack)
            {
                //Debug.Log("I should rotate left cuz degrees away is " + degreesAwayFromPerfection + " and adjustedrotz is " + adjustedRotZ);
                transform.Rotate(0f, 0f, Mathf.Clamp(rotateBackUpSpeed, -Mathf.Abs(degreesAwayFromPerfection), Mathf.Abs(degreesAwayFromPerfection)), Space.Self);
            }
        }
    }



    public void Boost()
    {
        if (!boostAllowed) return;

        Vector3 boostVector = transform.forward * buttBoostForce + Vector3.up * buttBoostUpForce;
        rigidBody.AddForce(boostVector);
        Vector3 torqueToAdd = Random.insideUnitCircle * buttBoostTorque;
        rigidBody.AddTorque(torqueToAdd);
        boostAllowed = false;
        Invoke("ReloadBoost", buttBoostCooldownTime);
    }

    private void ReloadBoost()
    {
        boostAllowed = true;
    }
}
