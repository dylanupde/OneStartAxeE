// Some code derived from https://www.youtube.com/watch?v=j6_SMdWeGFI


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField] Transform centerOfMassTransform;
    [SerializeField] WheelCollider frontLWheelColl, frontRWheelColl, rearLWheelColl, rearRWheelColl;
    [SerializeField] Transform frontLWheelModelTransform, frontRWheelModelTransform, rearLWheelModelTransform, rearRWheelModelTransform;
    [SerializeField] float downForce = 5f;
    [SerializeField] float motorForce = 50f;
    [SerializeField] float maxSteeringAngle = 30f;
    [SerializeField] bool isPlayer1;

    float horizontalInput;
    [SerializeField] float verticalInput;
    float steeringAngle;
    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMassTransform.localPosition;
    }


    void FixedUpdate()
    {
        rigidBody.AddForce(Vector3.down * downForce);

        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }



    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }


    private void Steer()
    {
        steeringAngle = maxSteeringAngle * horizontalInput;
        frontLWheelColl.steerAngle = steeringAngle;
        frontRWheelColl.steerAngle = steeringAngle;
    }


    private void Accelerate()
    {
        frontLWheelColl.motorTorque = motorForce * verticalInput;
        frontRWheelColl.motorTorque = motorForce * verticalInput;
        rearLWheelColl.motorTorque = motorForce * verticalInput;
        rearLWheelColl.motorTorque = motorForce * verticalInput;
    }


    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLWheelColl, frontLWheelModelTransform);
        UpdateWheelPose(frontRWheelColl, frontRWheelModelTransform);
        UpdateWheelPose(rearLWheelColl, rearLWheelModelTransform);
        UpdateWheelPose(rearRWheelColl, rearRWheelModelTransform);
    }


    private void UpdateWheelPose(WheelCollider inputWheelColl, Transform inputWheelTransform)
    {
        Vector3 wheelPos = inputWheelTransform.position;
        Quaternion wheelRot = inputWheelTransform.rotation;

        inputWheelColl.GetWorldPose(out wheelPos, out wheelRot);
        inputWheelTransform.position = wheelPos;
        inputWheelTransform.rotation = wheelRot;
    }
}
