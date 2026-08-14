using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_rotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 180f;

    private void Update()
    {
        transform.Rotate(
            0f,
            0f,
            rotateSpeed * Time.deltaTime
        );
    }
}
