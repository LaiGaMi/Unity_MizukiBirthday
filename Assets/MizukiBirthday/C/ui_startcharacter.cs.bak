using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_startcharacter : MonoBehaviour
{
	// 最大旋轉角度（例如 15 = 左右各15度）
    public float maxAngle = 15f;

    // 旋轉速度（越大越快）
    public float rotateSpeed = 2f;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 使用時間產生平滑的sin波動
        float angle = Mathf.Sin(Time.time * rotateSpeed) * maxAngle;

        // 套用到Z軸旋轉（2D UI通常轉Z）
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
