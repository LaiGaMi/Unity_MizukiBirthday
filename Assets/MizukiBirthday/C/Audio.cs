using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio Instance { get; private set; }

    // 音效名稱 → AudioSource
    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        // 場景只能有一個 Audio 管理器
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 找出 Audio 物件底下「所有」AudioSource
        AudioSource[] sources = GetComponentsInChildren<AudioSource>(true);

        foreach (AudioSource source in sources)
        {
            // 使用 GameObject 名稱當作音效名稱
            string audioName = source.gameObject.name;

            // 防止重複名稱
            if (audioSources.ContainsKey(audioName))
            {
                Debug.LogWarning(
                    $"Audio：發現重複的音效名稱「{audioName}」，已忽略這個 AudioSource。",
                    source.gameObject
                );

                continue;
            }

            audioSources.Add(audioName, source);
        }
    }

    /// <summary>
    /// 播放指定音效。
    /// 如果音效正在播放，會先停止再重新播放。
    /// </summary>
    public void Play(string audioName)
    {
        if (!audioSources.TryGetValue(audioName, out AudioSource source))
        {
            Debug.LogWarning($"Audio：找不到音效「{audioName}」");
            return;
        }

        // 先停止
        source.Stop();

        // 從頭播放
        source.Play();
    }

    /// <summary>
    /// 停止指定音效。
    /// </summary>
    public void Stop(string audioName)
    {
        if (!audioSources.TryGetValue(audioName, out AudioSource source))
        {
            Debug.LogWarning($"Audio：找不到音效「{audioName}」");
            return;
        }

        source.Stop();
    }

    /// <summary>
    /// 判斷指定音效是否正在播放。
    /// </summary>
    public bool IsPlaying(string audioName)
    {
        if (!audioSources.TryGetValue(audioName, out AudioSource source))
        {
            return false;
        }

        return source.isPlaying;
    }
}