using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealChecker : MonoBehaviour
{
    [SerializeField] float minImpulseToSteal = 1000f;
    [SerializeField] float stealableRechargeTime = 1.5f;

    VehicleControllerV3 myVehicleController;
    [HideInInspector] public bool isStealable = true;

    // Start is called before the first frame update
    void Start()
    {
        myVehicleController = transform.GetComponent<VehicleControllerV3>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isStealable) return;

        Rigidbody otherRigidbody = collision.collider.attachedRigidbody;

        if (myVehicleController.currentPassengerTransform != null && otherRigidbody)
        {
            VehicleControllerV3 otherVehicleController = otherRigidbody.GetComponent<VehicleControllerV3>();

            if (otherVehicleController && otherVehicleController.GetComponent<StealChecker>().isStealable)
            {
                if (collision.impulse.magnitude > minImpulseToSteal)
                {
                    if (GetComponent<PlayerInput>().isPlayer2) Debug.Log("Impulse force: " + collision.impulse.magnitude);
                    if (GetComponent<PlayerInput>().isPlayer2) Debug.Log("OtherRB: ", myVehicleController.currentPassengerTransform);
                    if (GetComponent<PlayerInput>().isPlayer2) Debug.Log("OtherVehicleController: ", otherVehicleController.gameObject);
                    if (GetComponent<PlayerInput>().isPlayer2) Debug.Log("my VehicleController: ", myVehicleController.gameObject);

                    StartCoroutine(MovePassengerToOtherVehicleCoroutine(myVehicleController.currentPassengerTransform, otherVehicleController.passengerLocation));
                    myVehicleController.currentPassengerTransform.parent = otherRigidbody.transform;
                    otherVehicleController.currentPassengerTransform = myVehicleController.currentPassengerTransform;
                    myVehicleController.currentPassengerTransform = null;

                    isStealable = false;
                    Invoke("MakeStealable", stealableRechargeTime);
                }
            }
        }
    }



    IEnumerator MovePassengerToOtherVehicleCoroutine(Transform inputPassengerTransform, Transform inputTargetTransform)
    {
        Vector3 startPos = inputPassengerTransform.position;
        float lerpSpeed = 4f;
        float t = 0f;

        while (t <= 1f)
        {
            t += lerpSpeed * Time.deltaTime;
            inputPassengerTransform.position = Vector3.Lerp(startPos, inputTargetTransform.position, t);
            yield return null;
        }

        inputPassengerTransform.position = inputTargetTransform.position;
    }

    void MakeStealable()
    {
        isStealable = true;
    }
}
