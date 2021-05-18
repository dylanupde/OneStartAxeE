using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerCollider : MonoBehaviour
{
    public PassengerDropoff myDropoff;
    [SerializeField] float maxSpeedForPickup = 0.2f;

    MeshCollider meshCollider;
    MeshRenderer meshRenderer;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        GameManager.Instance.pickups.Add(this);
        transform.parent.gameObject.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        VehicleControllerV3 otherVehicleController = null;

        // Check if there's an attached rigidbody first so we don't throw an error
        if (other.attachedRigidbody)
        {
            otherVehicleController = other.attachedRigidbody.transform.GetComponent<VehicleControllerV3>();
        }

        if (otherVehicleController)
        {
            if (other.attachedRigidbody.velocity.magnitude <= maxSpeedForPickup)
            {
                meshRenderer.enabled = false;
                transform.parent.parent = other.attachedRigidbody.transform;
                meshCollider.enabled = false;
                otherVehicleController.currentPassengerTransform = transform.parent;
                StartCoroutine(MovePassengerToCarCoroutine(otherVehicleController.passengerLocation));
            }
        }
    }



    IEnumerator MovePassengerToCarCoroutine(Transform inputTargetTransform)
    {
        Vector3 startPos = transform.parent.position;
        float lerpSpeed = 4f;
        float t = 0f;

        while (t <= 1f)
        {
            t += lerpSpeed * Time.deltaTime;
            transform.parent.position = Vector3.Lerp(startPos, inputTargetTransform.position, t);
            yield return null;
        }

        transform.parent.position = inputTargetTransform.position;
    }
}
