using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerDropoff : MonoBehaviour
{
    [SerializeField] float maxSpeedForDropoff = 0.2f;

    [HideInInspector] public PassengerCollider myPassengerCollider;

    MeshCollider meshCollider;

    // Start is called before the first frame update
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        GameManager.Instance.dropoffs.Add(this);
        gameObject.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        VehicleControllerV3 otherVehicleController = null;

        // Check if there's an attached rigidbody first so we don't throw an error
        if (other.attachedRigidbody)
        {
            otherVehicleController = other.attachedRigidbody.transform.GetComponent<VehicleControllerV3>();
        }

        if (otherVehicleController && otherVehicleController.currentPassengerTransform != null)
        {
            if (other.attachedRigidbody.velocity.magnitude <= maxSpeedForDropoff)
            {
                Transform theirPassenger = otherVehicleController.currentPassengerTransform;
                otherVehicleController.currentPassengerTransform.parent = null;
                otherVehicleController.currentPassengerTransform = null;
                meshCollider.enabled = false;

                if (otherVehicleController.GetComponent<PlayerInput>().isPlayer2)
                {
                    GameManager.Instance.AddToScore(false);
                }
                else
                {
                    GameManager.Instance.AddToScore(true);
                }

                StartCoroutine(MovePassengerToDropoffPointCoroutine(transform, theirPassenger));
            }
        }
    }


    IEnumerator MovePassengerToDropoffPointCoroutine(Transform inputTargetTransform, Transform inputPassengerTransform)
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
        gameObject.SetActive(false);
    }
}
