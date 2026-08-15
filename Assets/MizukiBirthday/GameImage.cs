using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "GameImage",
    menuName = "Game/Game Image"
)]
public class GameImage : ScriptableObject
{
    // =========================
    // 立繪清單
    // =========================

    [Header("立繪清單")]
    public List<CharacterImageData> characterImageList =
        new List<CharacterImageData>();


    // =========================
    // 背景清單
    // =========================

    [Header("背景清單")]
    public List<BackgroundImageData> backgroundImageList =
        new List<BackgroundImageData>();


    // =========================
    // 音效清單
    // =========================

    [Header("音效清單")]
    public List<SoundData> soundList =
        new List<SoundData>();
		
	// =========================
	// BGM 清單
	// =========================

	[Header("BGM清單")]
	public List<BGMData> bgmList =
	    new List<BGMData>();


    // =========================
    // 取得立繪
    // =========================

    public Sprite GetCharacterImage(string id)
    {
        foreach (CharacterImageData data in characterImageList)
        {
            if (data.id == id)
            {
                return data.image;
            }
        }

        Debug.LogWarning(
            "找不到立繪 ID : " + id
        );

        return null;
    }


    // =========================
    // 取得背景
    // =========================

    public Sprite GetBackgroundImage(string id)
    {
        foreach (BackgroundImageData data in backgroundImageList)
        {
            if (data.id == id)
            {
                return data.image;
            }
        }

        Debug.LogWarning(
            "找不到背景 ID : " + id
        );

        return null;
    }


    // =========================
    // 取得音效
    // =========================

    public AudioClip GetSound(string id)
    {
        // 沒有指定音效
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }


        foreach (SoundData data in soundList)
        {
            if (data.id == id)
            {
                return data.sound;
            }
        }


        Debug.LogWarning(
            "找不到音效 ID : " + id
        );

        return null;
    }
	
	// =========================
	// 取得 BGM
	// =========================

	public AudioClip GetBGM(string id)
	{
	    // 沒有指定 BGM
	    if (string.IsNullOrEmpty(id))
	    {
   	     return null;
  	  }


  	  foreach (BGMData data in bgmList)
  	  {
  	      if (data.id == id)
   	     {
   	         return data.bgm;
   	     }
   	 }


   	 Debug.LogWarning(
   	     "找不到 BGM ID : " + id
   	 );
	
	    return null;
	}
}


// =========================
// 立繪資料
// =========================

[System.Serializable]
public class CharacterImageData
{
    public string id;
    public Sprite image;
}


// =========================
// 背景資料
// =========================

[System.Serializable]
public class BackgroundImageData
{
    public string id;
    public Sprite image;
}


// =========================
// 音效資料
// =========================

[System.Serializable]
public class SoundData
{
    public string id;
    public AudioClip sound;
}

// =========================
// BGM 資料
// =========================

[System.Serializable]
public class BGMData
{
    public string id;
    public AudioClip bgm;
}