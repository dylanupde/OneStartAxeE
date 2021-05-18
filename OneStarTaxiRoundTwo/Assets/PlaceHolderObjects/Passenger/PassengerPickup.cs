using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("literally triggered");

        if(other.gameObject.tag == "PickUp")
        {
            Debug.Log("pickup pls");
            Destroy(this);
        }
    }
}
