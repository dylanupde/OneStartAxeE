using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydrantSquirter : MonoBehaviour
{
    [SerializeField] ParticleSystem squirtParticles;
    [SerializeField] float squirtForce;

    GameObject hydrantObj;
    MeshCollider meshCollider;
    float colliderHeight;
    bool checkingForHydrant = true;

    // Start is called before the first frame update
    void Start()
    {
        hydrantObj = transform.parent.GetChild(0).gameObject;
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.enabled = false;
        colliderHeight = 46.66f * transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkingForHydrant)
        {
            if (hydrantObj.GetComponent<FixedJoint>() == null)
            {
                squirtParticles.Play(true);
                meshCollider.enabled = true;
                checkingForHydrant = false;
            }
        }
    }


    void OnTriggerStay(Collider other)
    {
        Rigidbody otherRigidbody = other.attachedRigidbody;

        if (otherRigidbody && otherRigidbody.transform.parent != transform.parent)
        {
            RaycastHit raycastHit;
            if (Physics.SphereCast(transform.parent.position, 0.5f, Vector3.up, out raycastHit, colliderHeight))
            {
                if (raycastHit.rigidbody)
                {
                    float distToTarget = Mathf.Abs(transform.parent.position.y - otherRigidbody.transform.position.y);
                    float forceToAdd = Mathf.Abs(colliderHeight - distToTarget) * squirtForce;
                    otherRigidbody.AddForceAtPosition(Vector3.up * forceToAdd, raycastHit.point);
                    otherRigidbody.AddForce(Vector3.up * forceToAdd);
                }
            }
        }
    }
}
