using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_tips : MonoBehaviour
{
    [Header("提示 UI")]
    [SerializeField] private GameObject hintObject;

    [Header("等待時間")]
    [SerializeField] private float waitTime = 0.5f;

    private void Start()
    {
		Time.timeScale = 1f;
        // 開始等待
        StartCoroutine(ShowHintAfterDelay());
    }

    private IEnumerator ShowHintAfterDelay()
    {
        // 等待 0.5 秒
        yield return new WaitForSecondsRealtime(waitTime);

        // 顯示提示
        if (hintObject != null)
        {
            hintObject.SetActive(true);
        }

        // 暫停遊戲時間
        Time.timeScale = 0f;
    }

    // 給 Button 的 OnClick() 綁定
    public void Confirm()
    {
        // 隱藏提示
        if (hintObject != null)
        {
            hintObject.SetActive(false);
        }

        // 恢復遊戲
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        // 避免離開場景時遊戲仍然處於暫停狀態
        Time.timeScale = 1f;
    }
}