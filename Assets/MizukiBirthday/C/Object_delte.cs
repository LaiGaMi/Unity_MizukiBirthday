using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_delte : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector3 viewportPosition =
            mainCamera.WorldToViewportPoint(transform.position);

        // 完全離開攝影機畫面
        if (viewportPosition.x < 0f ||
            viewportPosition.x > 1f ||
            viewportPosition.y < 0f ||
            viewportPosition.y > 1f)
        {
            Destroy(gameObject);
        }
    }
}
