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