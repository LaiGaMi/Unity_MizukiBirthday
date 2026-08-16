using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_startcharacter : MonoBehaviour
{
    // 最大旋轉角度（例如 15 = 原始角度左右各15度）
    public float maxAngle = 15f;

    // 旋轉速度（越大越快）
    public float rotateSpeed = 2f;

    // 原始 Z 軸角度
    private float originalAngle;


    // =========================================================
    // 初始化
    // =========================================================

    private void Start()
    {
        // 記錄物件一開始的 Z 軸角度
        originalAngle = transform.eulerAngles.z;
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        // 以原始角度為中心左右旋轉
        float angle =
            originalAngle +
            Mathf.Sin(Time.unscaledTime * rotateSpeed) * maxAngle;

        // 套用到 Z 軸旋轉
        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            angle
        );
    }
}