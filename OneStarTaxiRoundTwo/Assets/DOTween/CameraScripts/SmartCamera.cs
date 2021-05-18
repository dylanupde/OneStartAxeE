using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour
{
    //in case we want more than 2 players ever
    public List<Transform> targets;
    //in case we want to adjust the position of our camera
    public Vector3 offset;
    //for the smoothing
    public float smoothTime = .7f;
    private Vector3 velocity;

    //updates a frame after the normal one. will make smooth like butter
    void LateUpdate()
    {
        float distance = Vector3.Distance(targets[0].position, targets[1].position);
        
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset - (transform.forward * distance);
        //adjust position
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
    
    //self explanatory.
    public Vector3 GetCenterPoint()
    {
        //in case only one is on screen because the other was blown up or something
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        //bounds does the thing where it encapsulates two targets for
        //you and gets the center. Kind of lucky it just exists
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        Vector3 center = bounds.center;

        return center;
    }
    //use bounds to check distance. then pull back relative to the distance between the two
}
