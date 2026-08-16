using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_startcharacter : MonoBehaviour
{
    // 最大旋轉角度（例如 15 = 左右各15度）
    public float maxAngle = 15f;

    // 旋轉速度（越大越快）
    public float rotateSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        // 使用不受 Time.timeScale 影響的時間
        float angle = Mathf.Sin(Time.unscaledTime * rotateSpeed) * maxAngle;

        // 套用到 Z 軸旋轉（2D UI 通常轉 Z）
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}