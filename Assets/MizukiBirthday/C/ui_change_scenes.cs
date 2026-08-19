using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTransition;

public class ui_change_scenes : MonoBehaviour
{
    // 場景名稱
    public string sceneName;

    // 改成拖這個
    public TransitionSettings transitionSettings;

    public float loadDelay = 0f;

    public bool restart = false;

    // 開啟後會自動回到上一個場景
    public bool goToPreviousScene = false;


    // 按鈕呼叫
    public void ChangeScene()
    {
        string targetScene = sceneName;

        // 如果開啟「回到上一個場景」
        if (goToPreviousScene)
        {
            targetScene = mizukiDataBackup.PreviousScene;

            // 防止沒有上一個場景
            if (string.IsNullOrEmpty(targetScene))
            {
                Debug.LogWarning("找不到上一個場景！");
                return;
            }
        }

        TransitionManager.Instance().Transition(
            targetScene,
            transitionSettings,
            loadDelay
        );
    }


    public void ChangeSceneRestart()
    {
        if (restart)
        {
            mizuki.att01 = 1;
            mizuki.att02 = 0;
            mizuki.att03 = 0;
            mizuki.att04 = 0;
            mizuki.att05 = 0;
            mizuki.att06 = 0;
            mizuki.att07 = 0;

            mizuki.level = 0;
            mizuki.exp = 0;

            mizuki.card = 0;
            mizuki.cardMax = 0;

            mizuki.Time = 0;
        }

        string targetScene = sceneName;

        // 如果開啟「回到上一個場景」
        if (goToPreviousScene)
        {
            targetScene = mizukiDataBackup.PreviousScene;

            if (string.IsNullOrEmpty(targetScene))
            {
                Debug.LogWarning("找不到上一個場景！");
                return;
            }
        }

        TransitionManager.Instance().Transition(
            targetScene,
            transitionSettings,
            loadDelay
        );
    }
}