using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public VehicleControllerV3 myVehicleScript;
    [HideInInspector] public Vector3 currentFacingVector;
    [HideInInspector] public bool upIsPressed, downIsPressed, leftIsPressed, rightIsPressed, moveButtonIsPressed;

    public bool isPlayer2 = false;

    Transform cameraTransform;
    GameManager gameManager;
    Vector3 rightMovementVector, upMovementVector;
    KeyCode leftButton, rightButton, upButton, downButton, boostButton;

    void Start()
    {
        gameManager = GameManager.Instance;
        cameraTransform = Camera.main.transform;

        rightMovementVector = cameraTransform.right.normalized;
        upMovementVector = cameraTransform.forward;
        upMovementVector.y = 0f;
        upMovementVector = upMovementVector.normalized;

        currentFacingVector = rightMovementVector;

        if (!isPlayer2)
        {
            leftButton = KeyCode.A;
            rightButton = KeyCode.D;
            upButton = KeyCode.W;
            downButton = KeyCode.S;
            boostButton = KeyCode.Space;
        }
        else
        {
            leftButton = KeyCode.Keypad4;
            rightButton = KeyCode.Keypad6;
            upButton = KeyCode.Keypad8;
            downButton = KeyCode.Keypad5;
            boostButton = KeyCode.Keypad0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameHasStarted) return;

        GetPlayerInput();
    }




    private void GetPlayerInput()
    {
        if (Input.GetKey(leftButton)) leftIsPressed = true;
        if (Input.GetKey(rightButton)) rightIsPressed = true;
        if (Input.GetKey(upButton)) upIsPressed = true;
        if (Input.GetKey(downButton)) downIsPressed = true;
        if (Input.GetKeyDown(boostButton)) myVehicleScript.Boost();
    }


    public void GetCurrentFacingDirection()
    {
        if (leftIsPressed)
        {
            if (upIsPressed)
            {
                currentFacingVector = Vector3.Slerp(upMovementVector, -rightMovementVector, 0.5f);
            }
            else if (downIsPressed)
            {
                currentFacingVector = Vector3.Slerp(-rightMovementVector, -upMovementVector, 0.5f);
            }
            else
            {
                currentFacingVector = -rightMovementVector;
            }
        }
        else if (rightIsPressed)
        {
            if (upIsPressed)
            {
                currentFacingVector = Vector3.Slerp(upMovementVector, rightMovementVector, 0.5f);
            }
            else if (downIsPressed)
            {
                currentFacingVector = Vector3.Slerp(rightMovementVector, -upMovementVector, 0.5f);
            }
            else
            {
                currentFacingVector = rightMovementVector;
            }
        }
        else if (upIsPressed)
        {
            currentFacingVector = upMovementVector;
        }
        else if (downIsPressed)
        {
            currentFacingVector = -upMovementVector;
        }

        moveButtonIsPressed = upIsPressed || downIsPressed || leftIsPressed || rightIsPressed;
        currentFacingVector = currentFacingVector.normalized;
    }


    public void ResetPlayerInput()
    {
        leftIsPressed = false;
        rightIsPressed = false;
        upIsPressed = false;
        downIsPressed = false;
        moveButtonIsPressed = false;
    }
}
