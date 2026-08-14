using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Move : MonoBehaviour
{
    [Header("目標物件")]
    [SerializeField] private Transform target; // 想要瞬間跟隨的目標物件

    [Header("Z 軸偏移鎖定")]
    [SerializeField] private bool keepOwnZPosition = true; // 是否保持自己原本的 Z 軸深度（2D 遊戲常用）

    private void Update()
    {
        // 確保有指定目標，否則不執行
        if (target == null) return;

        if (keepOwnZPosition)
        {
            // 僅同步 X 和 Y 軸，保留原本的 Z 軸（避免物件因為 Z 軸被覆蓋而消失在攝影機前）
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
        else
        {
            // 完全複製目標的所有座標 (X, Y, Z)
            transform.position = target.position;
        }
    }

    // 方便在遊戲執行中隨時動態更換目標
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}