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
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	// 按鈕呼叫
    public void ChangeScene()
    {
		TransitionManager.Instance().Transition(sceneName, transitionSettings, loadDelay);
    }
	
	public void ChangeSceneRestart()
    {
		AAA_font.time = 0;
		AAA_font.HPUPUP = 0;
		AAA_font.BBOS = false;
        TransitionManager.Instance().Transition(sceneName, transitionSettings, loadDelay);
    }
}
