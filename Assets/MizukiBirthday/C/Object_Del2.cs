using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Del2 : MonoBehaviour
{
    [Header("設定幾秒後刪除物件")]
    [SerializeField] private float delaySeconds = 3.0f;

    void Start()
    {
        // 進入 Start 後，在 delaySeconds 秒之後銷毀掛載此腳本的 GameObject
        Destroy(gameObject, delaySeconds);
    }
}
