using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardController : MonoBehaviour
{
    public enum MovementType {LocalPositon, GlobalPosition}
    [SerializeField]private MovementLogic _movementLogic;
    [SerializeField] private MovementType _movementType;

    private void FixedUpdate()
    {
        MovementUpdate();
        RotationUpdate();
    }

    private void MovementUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        if (movement.magnitude != 0 && _movementType == MovementType.GlobalPosition)
        {
            movement = transform.InverseTransformDirection(movement);
        }
        _movementLogic.StrengthDirection= movement;
    }

    private void RotationUpdate()
    {
        short rotatateDirection = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            rotatateDirection = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotatateDirection = 1;
        }
        _movementLogic.RotationVector = new Vector3(0, rotatateDirection, 0);
    }
}
