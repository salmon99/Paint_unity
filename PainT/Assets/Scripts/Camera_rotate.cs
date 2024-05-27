using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_rotate : MonoBehaviour
{
    public GameObject character;

    private float xRotateMove, yRotateMove;

    private Vector2 touchStartPos;
    private Vector2 touchDelta;

    public float rotateSpeed = 500.0f;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            xRotateMove = Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;

            Vector3 characterPosition = character.transform.position;
            Vector3 cameraPosition = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 eulerRotation = rotation.eulerAngles;

            transform.RotateAround(characterPosition, Vector3.up, xRotateMove);
            transform.LookAt(characterPosition);
            //Debug.Log("Camera Position: " + cameraPosition);
        }
    }
}

