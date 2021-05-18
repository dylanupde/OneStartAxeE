using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControllerV2 : MonoBehaviour
{
    [SerializeField] Transform centerOfMassTransform;
    [SerializeField] Transform frontTransform;
    [SerializeField] WheelCollider frontLWheelColl, frontRWheelColl, rearLWheelColl, rearRWheelColl;
    [SerializeField] float acceleration;
    [SerializeField] float maxSteeringAngle = 30f;
    [SerializeField] float downForce = 5f;

    Transform frontLWheelModelTransform, frontRWheelModelTransform, rearLWheelModelTransform, rearRWheelModelTransform;
    Rigidbody rigidBody;
    float steeringAngle = 0f;
    float horizontalInput = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMassTransform.localPosition;

        frontLWheelModelTransform = frontLWheelColl.transform.GetChild(0);
        frontRWheelModelTransform = frontRWheelColl.transform.GetChild(0);
        rearLWheelModelTransform = rearLWheelColl.transform.GetChild(0);
        rearRWheelModelTransform = rearRWheelColl.transform.GetChild(0);
        
        frontLWheelColl.motorTorque = 0.0000001f;
        frontRWheelColl.motorTorque = 0.0000001f;
        rearLWheelColl.motorTorque = 0.0000001f;
        rearRWheelColl.motorTorque = 0.0000001f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Steer();
        UpdateWheelPoses();

        //frontTransform.forward = transform.forward;
        //frontTransform.Rotate(0f, steeringAngle / 3f, 0f, Space.Self);

        if (Input.GetKey(KeyCode.W))
        {
            //rigidBody.AddForceAtPosition(frontTransform.forward * acceleration, frontTransform.position);
            rigidBody.AddForce(transform.forward * acceleration);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //rigidBody.AddForceAtPosition(-frontTransform.forward * acceleration, frontTransform.position);
            rigidBody.AddForce(-transform.forward * acceleration);
        }

        rigidBody.AddForceAtPosition(Vector3.down * downForce, frontLWheelModelTransform.position);
        rigidBody.AddForceAtPosition(Vector3.down * downForce, frontRWheelModelTransform.position);

        horizontalInput = Input.GetAxis("Horizontal");
    }


    private void Steer()
    {
        steeringAngle = maxSteeringAngle * horizontalInput;
        frontLWheelColl.steerAngle = steeringAngle;
        frontRWheelColl.steerAngle = steeringAngle;
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
