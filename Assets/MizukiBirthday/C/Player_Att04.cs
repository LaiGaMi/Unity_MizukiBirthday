using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Att04 : MonoBehaviour
{
    [Header("雨傘預置物")]
    public GameObject umbrellaPrefab;

    [Header("指定中心物件")]
    public Transform targetObject;


    private int currentLevel = 0;


    private void Start()
    {
        UpdateUmbrellas();
    }


    private void Update()
    {
        // 技能等級發生變化時重新設定雨傘
        if (mizuki.att04 != currentLevel)
        {
            UpdateUmbrellas();
        }
    }


    // =========================================================
    // 更新雨傘
    // =========================================================

    private void UpdateUmbrellas()
    {
        currentLevel = mizuki.att04;


        // 沒有技能
        if (currentLevel <= 0)
        {
            ClearUmbrellas();
            return;
        }


        // 先刪除原本的雨傘
        ClearUmbrellas();


        // 根據技能等級決定數量
        int umbrellaCount = 0;


        switch (currentLevel)
        {
            case 1:
                umbrellaCount = 1;
                break;

            case 2:
                umbrellaCount = 2;
                break;

            case 3:
                umbrellaCount = 4;
                break;
        }


        // 生成雨傘
        CreateUmbrellas(umbrellaCount);
    }


    // =========================================================
    // 生成雨傘
    // =========================================================

    private void CreateUmbrellas(int count)
    {
        if (umbrellaPrefab == null)
        {
            Debug.LogWarning("沒有設定雨傘預置物！");
            return;
        }

        if (targetObject == null)
        {
            Debug.LogWarning("沒有設定指定中心物件！");
            return;
        }


        for (int i = 0; i < count; i++)
        {
            float angle = 360f / count * i;

            // 圓形分布
            float radian = angle * Mathf.Deg2Rad;

            Vector3 position = new Vector3(
                Mathf.Cos(radian),
                Mathf.Sin(radian),
                0f
            );


            // 生成雨傘
            GameObject umbrella = Instantiate(
                umbrellaPrefab,
                targetObject
            );


            // 設定雨傘位置
            umbrella.transform.localPosition = position;


            // 設定旋轉角度
            umbrella.transform.localRotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    }


    // =========================================================
    // 刪除原本的雨傘
    // =========================================================

    private void ClearUmbrellas()
    {
        if (targetObject == null)
        {
            return;
        }


        for (int i = targetObject.childCount - 1; i >= 0; i--)
        {
            Transform child = targetObject.GetChild(i);

            if (child.CompareTag("Umbrella"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}