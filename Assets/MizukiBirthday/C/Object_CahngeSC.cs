using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTransition;

public class Object_CahngeSC : MonoBehaviour
{
	// 場景名稱
    public string sceneName;

    // 改成拖這個
    public TransitionSettings transitionSettings;

    public float loadDelay = 0f;
	
    void OnEnable()
    {
        StartCoroutine(WaitAndTransition());
    }

    IEnumerator WaitAndTransition()
    {
        // 等待遊戲時間啟動
        while (Time.timeScale <= 0f)
        {
            // 使用 unscaled time 等待，避免 Time.timeScale = 0 時 Coroutine 卡住
            yield return null;
        }

        // 遊戲時間已經啟動，才開始等待 loadDelay
        if (loadDelay > 0f)
        {
            float timer = 0f;

            while (timer < loadDelay)
            {
                // 如果遊戲時間再次停止，就暫停這個等待
                if (Time.timeScale > 0f)
                {
                    timer += Time.deltaTime;
                }

                yield return null;
            }
        }

        // 確定遊戲時間是啟動狀態才切換
        while (Time.timeScale <= 0f)
        {
            yield return null;
        }

        TransitionManager.Instance().Transition(
            sceneName,
            transitionSettings,
            0f
        );
    }
}