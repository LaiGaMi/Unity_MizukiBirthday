using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talk : MonoBehaviour
{
    public string talkFileName;
    private GameObject[] childObjects;

    private DialogueFile dialogueFile;
    private int currentIndex = 0;

    [Header("UI父物件")]
    public GameObject dialogueUI;


    // =========================================================
    // 背景
    // =========================================================

    [Header("背景")]
    public Image backgroundImage;

    // 第二層背景
    // 用來進行淡入效果
    public Image backgroundTransitionImage;


    // =========================================================
    // 角色立繪
    // =========================================================

    [Header("UI圖片")]
    public Image ch01Image;
    public Image ch02Image;
    public Image ch03Image;


    // =========================================================
    // 文字
    // =========================================================

    [Header("文字")]
    public Text nameText;
    public Text talkText;


    // =========================================================
    // 圖片資料
    // =========================================================

    [Header("圖片資料")]
    public GameImage imageDatabase;


    // =========================================================
    // 結束後啟用
    // =========================================================

    [Header("結束後啟用")]
    public GameObject[] enableObjects;


    // =========================================================
    // 對話狀態
    // =========================================================

    private bool talking = false;

    // 是否正在播放文字 / 背景 / 立繪動畫
    private bool isTextAnimating = false;

    // 文字是否正在播放
    private bool textAnimating = false;

    // 背景是否正在切換
    private bool backgroundAnimating = false;

    // 立繪是否正在切換
    private bool ch01Animating = false;
    private bool ch02Animating = false;
    private bool ch03Animating = false;


    // =========================================================
    // 文字動畫設定
    // =========================================================

    [Header("文字動畫")]
    public float textSpeed = 0.05f;


    // =========================================================
    // 背景動畫設定
    // =========================================================

    [Header("背景切換")]
    public float backgroundFadeTime = 1f;


    // =========================================================
    // 立繪動畫設定
    // =========================================================

    [Header("立繪切換動畫")]

    // 立繪從原本位置往下掉多少
    public float characterDropDistance = 100f;

    // 立繪移動時間
    public float characterMoveTime = 0.4f;

    // 移動速度曲線
    public AnimationCurve characterMoveCurve =
        AnimationCurve.EaseInOut(
            0f,
            0f,
            1f,
            1f
        );


    // =========================================================
    // Coroutine
    // =========================================================

    private Coroutine textCoroutine;
    private Coroutine backgroundCoroutine;

    private Coroutine ch01Coroutine;
    private Coroutine ch02Coroutine;
    private Coroutine ch03Coroutine;


    // =========================================================
    // 立繪原始位置
    // =========================================================
    //
    // 每次開始立繪進場動畫時，
    // 直接記錄動畫開始前的真正目標位置。
    //
    // 之後如果玩家在動畫途中點擊，
    // 不再透過 characterDropDistance 推算，
    // 而是直接回到這裡記錄的位置。
    //

    private Vector2 ch01OriginalPosition;
    private Vector2 ch02OriginalPosition;
    private Vector2 ch03OriginalPosition;

    private bool ch01HasOriginalPosition = false;
    private bool ch02HasOriginalPosition = false;
    private bool ch03HasOriginalPosition = false;


    // =========================================================
    // 目前背景
    // =========================================================

    private string currentBackgroundID = "";


    // =========================================================
    // Awake
    // =========================================================

    void Awake()
    {
        childObjects = new GameObject[]
        {
            gameObject
        };
    }


    // =========================================================
    // OnEnable
    // =========================================================

    public void OnEnable()
    {
        StartDialogue(
            talkFileName,
            childObjects
        );
    }


    // =========================================================
    // OnDisable
    // =========================================================

    void OnDisable()
    {
        StopAllAnimations();

        isTextAnimating = false;
    }


    // =========================================================
    // Update
    // =========================================================

    void Update()
    {
        if (!talking)
            return;


        // 滑鼠 / 觸控
        if (Input.GetMouseButtonDown(0))
        {
            // =========================================
            // 動畫還沒完成
            // =========================================

            if (isTextAnimating)
            {
                CompleteAnimation();

                // 這次點擊不進下一句
                return;
            }


            // =========================================
            // 所有動畫都完成
            // =========================================

            NextDialogue();
        }
    }


    // =========================================================
    // 開始對話
    // =========================================================

    public void StartDialogue(
        string fileName,
        GameObject[] objects
    )
    {
        StartStory();

        LoadJSON(fileName);

        if (dialogueFile == null)
            return;


        currentIndex = 0;

        talking = true;


        // 重置目前背景 ID
        currentBackgroundID = "";


        // 確保立繪動畫狀態清空
        ch01Animating = false;
        ch02Animating = false;
        ch03Animating = false;


        // 清除立繪原始位置記錄
        ch01HasOriginalPosition = false;
        ch02HasOriginalPosition = false;
        ch03HasOriginalPosition = false;


        dialogueUI.SetActive(true);


        // 確保第二層背景一開始透明
        SetImageAlpha(
            backgroundTransitionImage,
            0f
        );


        // 顯示第一句
        ShowDialogue();
    }


    // =========================================================
    // 讀取 JSON
    // =========================================================

    void LoadJSON(string fileName)
    {
        string path =
            "Talk/" + fileName;


        TextAsset json =
            Resources.Load<TextAsset>(path);


        if (json == null)
        {
            Debug.LogError(
                "找不到JSON : Resources/"
                + path
                + ".json"
            );


            dialogueFile = null;

            return;
        }


        dialogueFile =
            JsonUtility.FromJson<DialogueFile>(
                json.text
            );
    }


    // =========================================================
    // 顯示一句話
    // =========================================================

    void ShowDialogue()
    {
        DialogueLine data =
            dialogueFile.dialogues[
                currentIndex
            ];


        // =====================================================
        // 姓名
        // =====================================================

        nameText.text =
            data.name;


        // =====================================================
        // 背景
        // =====================================================

        ChangeBackground(
            data.background
        );


        // =====================================================
        // 角色立繪
        // =====================================================

        ChangeCharacter(
            ch01Image,
            data.ch01,
            1
        );


        ChangeCharacter(
            ch02Image,
            data.ch02,
            2
        );


        ChangeCharacter(
            ch03Image,
            data.ch03,
            3
        );


        // =====================================================
        // 文字
        // =====================================================

        StartTextAnimation(
            data.talk
        );


        // 更新總動畫狀態
        UpdateAnimationState();
    }


    // =========================================================
    // 文字動畫
    // =========================================================

    void StartTextAnimation(
        string text
    )
    {
        if (textCoroutine != null)
        {
            StopCoroutine(
                textCoroutine
            );
        }


        textCoroutine =
            StartCoroutine(
                TypeText(text)
            );
    }


    IEnumerator TypeText(
        string text
    )
    {
        textAnimating = true;

        UpdateAnimationState();


        talkText.text = "";


        foreach (char c in text)
        {
            talkText.text += c;


            yield return new WaitForSeconds(
                textSpeed
            );
        }


        textAnimating = false;

        textCoroutine = null;


        UpdateAnimationState();
    }


    // =========================================================
    // 完成文字動畫
    // =========================================================

    void CompleteText()
    {
        if (!textAnimating)
            return;


        if (textCoroutine != null)
        {
            StopCoroutine(
                textCoroutine
            );

            textCoroutine = null;
        }


        talkText.text =
            dialogueFile
                .dialogues[currentIndex]
                .talk;


        textAnimating = false;


        UpdateAnimationState();
    }


    // =========================================================
    // 背景切換
    // =========================================================

    void ChangeBackground(
        string backgroundID
    )
    {
        // 沒有指定背景
        if (string.IsNullOrEmpty(
            backgroundID
        ))
        {
            backgroundAnimating = false;

            UpdateAnimationState();

            return;
        }


        // 和上一句相同
        if (
            backgroundID ==
            currentBackgroundID
        )
        {
            backgroundAnimating = false;

            UpdateAnimationState();

            return;
        }


        // 取得新背景
        Sprite newSprite =
            imageDatabase.GetBackgroundImage(
                backgroundID
            );


        if (newSprite == null)
        {
            backgroundAnimating = false;

            UpdateAnimationState();

            return;
        }


        // 記錄新的背景 ID
        currentBackgroundID =
            backgroundID;


        // 停止上一個背景動畫
        if (backgroundCoroutine != null)
        {
            StopCoroutine(
                backgroundCoroutine
            );

            backgroundCoroutine = null;
        }


        // 開始背景淡入
        backgroundCoroutine =
            StartCoroutine(
                FadeBackground(newSprite)
            );
    }


    // =========================================================
    // 背景淡入
    // =========================================================

    IEnumerator FadeBackground(
        Sprite newSprite
    )
    {
        backgroundAnimating = true;

        UpdateAnimationState();


        // 第二層設定新背景
        backgroundTransitionImage.sprite =
            newSprite;


        // 第二層完全透明
        SetImageAlpha(
            backgroundTransitionImage,
            0f
        );


        float timer = 0f;


        // 背景淡入
        while (
            timer <
            backgroundFadeTime
        )
        {
            timer +=
                Time.deltaTime;


            float progress =
                timer /
                backgroundFadeTime;


            progress =
                Mathf.Clamp01(
                    progress
                );


            SetImageAlpha(
                backgroundTransitionImage,
                progress
            );


            yield return null;
        }


        // 完全不透明
        SetImageAlpha(
            backgroundTransitionImage,
            1f
        );


        // 第一層換成新背景
        backgroundImage.sprite =
            newSprite;


        // 第一層保持完全不透明
        SetImageAlpha(
            backgroundImage,
            1f
        );


        // 第二層恢復透明
        SetImageAlpha(
            backgroundTransitionImage,
            0f
        );


        backgroundAnimating = false;

        backgroundCoroutine = null;


        UpdateAnimationState();
    }


    // =========================================================
    // 立即完成背景
    // =========================================================

    void CompleteBackground()
    {
        if (!backgroundAnimating)
            return;


        if (backgroundCoroutine != null)
        {
            StopCoroutine(
                backgroundCoroutine
            );

            backgroundCoroutine = null;
        }


        DialogueLine data =
            dialogueFile
                .dialogues[currentIndex];


        Sprite newSprite =
            imageDatabase.GetBackgroundImage(
                data.background
            );


        if (newSprite != null)
        {
            // 第一層直接換成新背景
            backgroundImage.sprite =
                newSprite;


            // 完全不透明
            SetImageAlpha(
                backgroundImage,
                1f
            );
        }


        // 第二層透明
        SetImageAlpha(
            backgroundTransitionImage,
            0f
        );


        backgroundAnimating = false;


        UpdateAnimationState();
    }


    // =========================================================
    // 角色立繪切換
    // =========================================================

    void ChangeCharacter(
        Image image,
        CharacterSlot data,
        int slot
    )
    {
        // =====================================================
        // 沒有角色
        // =====================================================

        if (data == null)
        {
            // 如果原本有圖片
            // 直接消失
            image.color =
                new Color(
                    1f,
                    1f,
                    1f,
                    0f
                );


            // 停止該角色動畫
            StopCharacterCoroutine(
                slot
            );


            SetCharacterAnimating(
                slot,
                false
            );


            ClearCharacterOriginalPosition(
                slot
            );


            return;
        }


        // =====================================================
        // 取得新的 Sprite
        // =====================================================

        Sprite newSprite =
            imageDatabase.GetCharacterImage(
                data.id
            );


        if (newSprite == null)
        {
            image.color =
                new Color(
                    1f,
                    1f,
                    1f,
                    0f
                );


            SetCharacterAnimating(
                slot,
                false
            );


            ClearCharacterOriginalPosition(
                slot
            );


            return;
        }


        // =====================================================
        // 判斷圖片是否相同
        // =====================================================

        if (image.sprite == newSprite)
        {
            // 圖片相同
            // 不播放動畫
            SetCharacter(
                image,
                data
            );


            SetCharacterAnimating(
                slot,
                false
            );


            ClearCharacterOriginalPosition(
                slot
            );


            return;
        }


        // =====================================================
        // 圖片不同
        // 開始進場動畫
        // =====================================================

        StartCharacterAnimation(
            image,
            newSprite,
            data,
            slot
        );
    }


    // =========================================================
    // 開始角色動畫
    // =========================================================

    void StartCharacterAnimation(
        Image image,
        Sprite newSprite,
        CharacterSlot data,
        int slot
    )
    {
        // 停止這個角色原本的動畫
        StopCharacterCoroutine(
            slot
        );


        // -----------------------------------------------------
        // 記錄原本位置
        // -----------------------------------------------------

        RectTransform rect =
            image.rectTransform;


        Vector2 targetPosition =
            rect.anchoredPosition;


        // =====================================================
        // 重要：
        // 直接記錄真正的原始目標位置
        // =====================================================

        SetCharacterOriginalPosition(
            slot,
            targetPosition
        );


        // -----------------------------------------------------
        // 設定新圖片
        // -----------------------------------------------------

        image.sprite =
            newSprite;


        // 設定角色顏色 / 聚焦
        SetCharacterColor(
            image,
            data
        );


        // -----------------------------------------------------
        // 設定起始位置
        // -----------------------------------------------------

        Vector2 startPosition =
            targetPosition;


        startPosition.y -=
            characterDropDistance;


        rect.anchoredPosition =
            startPosition;


        // -----------------------------------------------------
        // 開始動畫
        // -----------------------------------------------------

        SetCharacterAnimating(
            slot,
            true
        );


        Coroutine coroutine =
            StartCoroutine(
                MoveCharacter(
                    image,
                    startPosition,
                    targetPosition,
                    slot
                )
            );


        SetCharacterCoroutine(
            slot,
            coroutine
        );
    }


    // =========================================================
    // 角色往上移動
    // =========================================================

    IEnumerator MoveCharacter(
        Image image,
        Vector2 startPosition,
        Vector2 targetPosition,
        int slot
    )
    {
        RectTransform rect =
            image.rectTransform;


        float timer = 0f;


        // 防止設定為 0 導致除以 0
        if (characterMoveTime <= 0f)
        {
            rect.anchoredPosition =
                targetPosition;


            SetCharacterAnimating(
                slot,
                false
            );


            SetCharacterCoroutine(
                slot,
                null
            );


            ClearCharacterOriginalPosition(
                slot
            );


            UpdateAnimationState();

            yield break;
        }


        // -----------------------------------------------------
        // 移動
        // -----------------------------------------------------

        while (
            timer <
            characterMoveTime
        )
        {
            timer +=
                Time.deltaTime;


            float progress =
                timer /
                characterMoveTime;


            progress =
                Mathf.Clamp01(
                    progress
                );


            // 使用自訂 AnimationCurve
            float curveValue =
                characterMoveCurve.Evaluate(
                    progress
                );


            rect.anchoredPosition =
                Vector2.LerpUnclamped(
                    startPosition,
                    targetPosition,
                    curveValue
                );


            yield return null;
        }


        // -----------------------------------------------------
        // 確保最後位置正確
        // -----------------------------------------------------

        rect.anchoredPosition =
            targetPosition;


        SetCharacterAnimating(
            slot,
            false
        );


        SetCharacterCoroutine(
            slot,
            null
        );


        // 動畫正常完成
        // 原始位置記錄可以清除
        ClearCharacterOriginalPosition(
            slot
        );


        UpdateAnimationState();
    }


    // =========================================================
    // 立即完成角色動畫
    // =========================================================

    void CompleteCharacter(
        Image image,
        int slot
    )
    {
        if (!GetCharacterAnimating(slot))
            return;


        // -----------------------------------------------------
        // 停止 Coroutine
        // -----------------------------------------------------

        StopCharacterCoroutine(
            slot
        );


        // -----------------------------------------------------
        // 直接回到動畫開始前記錄的原始位置
        // -----------------------------------------------------

        RectTransform rect =
            image.rectTransform;


        if (HasCharacterOriginalPosition(slot))
        {
            rect.anchoredPosition =
                GetCharacterOriginalPosition(slot);
        }


        // -----------------------------------------------------
        // 動畫結束
        // -----------------------------------------------------

        SetCharacterAnimating(
            slot,
            false
        );


        // 清除原始位置記錄
        ClearCharacterOriginalPosition(
            slot
        );


        UpdateAnimationState();
    }


    // =========================================================
    // 記錄角色原始位置
    // =========================================================

    void SetCharacterOriginalPosition(
        int slot,
        Vector2 position
    )
    {
        if (slot == 1)
        {
            ch01OriginalPosition = position;
            ch01HasOriginalPosition = true;
        }
        else if (slot == 2)
        {
            ch02OriginalPosition = position;
            ch02HasOriginalPosition = true;
        }
        else if (slot == 3)
        {
            ch03OriginalPosition = position;
            ch03HasOriginalPosition = true;
        }
    }


    // =========================================================
    // 取得角色原始位置
    // =========================================================

    Vector2 GetCharacterOriginalPosition(
        int slot
    )
    {
        if (slot == 1)
        {
            return ch01OriginalPosition;
        }


        if (slot == 2)
        {
            return ch02OriginalPosition;
        }


        if (slot == 3)
        {
            return ch03OriginalPosition;
        }


        return Vector2.zero;
    }


    // =========================================================
    // 是否有角色原始位置
    // =========================================================

    bool HasCharacterOriginalPosition(
        int slot
    )
    {
        if (slot == 1)
        {
            return ch01HasOriginalPosition;
        }


        if (slot == 2)
        {
            return ch02HasOriginalPosition;
        }


        if (slot == 3)
        {
            return ch03HasOriginalPosition;
        }


        return false;
    }


    // =========================================================
    // 清除角色原始位置
    // =========================================================

    void ClearCharacterOriginalPosition(
        int slot
    )
    {
        if (slot == 1)
        {
            ch01HasOriginalPosition = false;
        }
        else if (slot == 2)
        {
            ch02HasOriginalPosition = false;
        }
        else if (slot == 3)
        {
            ch03HasOriginalPosition = false;
        }
    }


    // =========================================================
    // 停止角色 Coroutine
    // =========================================================

    void StopCharacterCoroutine(
        int slot
    )
    {
        Coroutine coroutine =
            GetCharacterCoroutine(
                slot
            );


        if (coroutine == null)
            return;


        StopCoroutine(
            coroutine
        );


        SetCharacterCoroutine(
            slot,
            null
        );
    }


    // =========================================================
    // 設定角色動畫狀態
    // =========================================================

    void SetCharacterAnimating(
        int slot,
        bool value
    )
    {
        if (slot == 1)
        {
            ch01Animating = value;
        }
        else if (slot == 2)
        {
            ch02Animating = value;
        }
        else if (slot == 3)
        {
            ch03Animating = value;
        }
    }


    // =========================================================
    // 取得角色動畫狀態
    // =========================================================

    bool GetCharacterAnimating(
        int slot
    )
    {
        if (slot == 1)
        {
            return ch01Animating;
        }


        if (slot == 2)
        {
            return ch02Animating;
        }


        if (slot == 3)
        {
            return ch03Animating;
        }


        return false;
    }


    // =========================================================
    // 設定角色 Coroutine
    // =========================================================

    void SetCharacterCoroutine(
        int slot,
        Coroutine coroutine
    )
    {
        if (slot == 1)
        {
            ch01Coroutine = coroutine;
        }
        else if (slot == 2)
        {
            ch02Coroutine = coroutine;
        }
        else if (slot == 3)
        {
            ch03Coroutine = coroutine;
        }
    }


    // =========================================================
    // 取得角色 Coroutine
    // =========================================================

    Coroutine GetCharacterCoroutine(
        int slot
    )
    {
        if (slot == 1)
        {
            return ch01Coroutine;
        }


        if (slot == 2)
        {
            return ch02Coroutine;
        }


        if (slot == 3)
        {
            return ch03Coroutine;
        }


        return null;
    }


    // =========================================================
    // 設定角色立繪
    // =========================================================

    void SetCharacter(
        Image image,
        CharacterSlot data
    )
    {
        // 沒有角色
        if (data == null)
        {
            image.color =
                new Color(
                    1f,
                    1f,
                    1f,
                    0f
                );

            return;
        }


        Sprite sprite =
            imageDatabase.GetCharacterImage(
                data.id
            );


        image.sprite =
            sprite;


        SetCharacterColor(
            image,
            data
        );
    }


    // =========================================================
    // 設定角色顏色 / 聚焦
    // =========================================================

    void SetCharacterColor(
        Image image,
        CharacterSlot data
    )
    {
        if (data == null)
        {
            image.color =
                new Color(
                    1f,
                    1f,
                    1f,
                    0f
                );

            return;
        }


        // 完全顯示
        image.color =
            new Color(
                1f,
                1f,
                1f,
                1f
            );


        // 聚焦
        if (data.focus)
        {
            image.color =
                Color.white;
        }
        else
        {
            image.color =
                new Color(
                    0.5f,
                    0.5f,
                    0.5f,
                    1f
                );
        }
    }


    // =========================================================
    // 設定 Image Alpha
    // =========================================================

    void SetImageAlpha(
        Image image,
        float alpha
    )
    {
        if (image == null)
            return;


        Color color =
            image.color;


        color.a =
            alpha;


        image.color =
            color;
    }


    // =========================================================
    // 完成所有動畫
    // =========================================================

    void CompleteAnimation()
    {
        // -----------------------------------------------------
        // 文字
        // -----------------------------------------------------

        if (textAnimating)
        {
            CompleteText();
        }


        // -----------------------------------------------------
        // 背景
        // -----------------------------------------------------

        if (backgroundAnimating)
        {
            CompleteBackground();
        }


        // -----------------------------------------------------
        // ch01
        // -----------------------------------------------------

        if (ch01Animating)
        {
            CompleteCharacter(
                ch01Image,
                1
            );
        }


        // -----------------------------------------------------
        // ch02
        // -----------------------------------------------------

        if (ch02Animating)
        {
            CompleteCharacter(
                ch02Image,
                2
            );
        }


        // -----------------------------------------------------
        // ch03
        // -----------------------------------------------------

        if (ch03Animating)
        {
            CompleteCharacter(
                ch03Image,
                3
            );
        }


        // 最後統一更新
        UpdateAnimationState();
    }


    // =========================================================
    // 更新總動畫狀態
    // =========================================================

    void UpdateAnimationState()
    {
        isTextAnimating =
            textAnimating ||
            backgroundAnimating ||
            ch01Animating ||
            ch02Animating ||
            ch03Animating;
    }


    // =========================================================
    // 停止所有動畫
    // =========================================================

    void StopAllAnimations()
    {
        // -----------------------------------------------------
        // 文字
        // -----------------------------------------------------

        if (textCoroutine != null)
        {
            StopCoroutine(
                textCoroutine
            );

            textCoroutine = null;
        }


        // -----------------------------------------------------
        // 背景
        // -----------------------------------------------------

        if (backgroundCoroutine != null)
        {
            StopCoroutine(
                backgroundCoroutine
            );

            backgroundCoroutine = null;
        }


        // -----------------------------------------------------
        // ch01
        // -----------------------------------------------------

        if (ch01Coroutine != null)
        {
            StopCoroutine(
                ch01Coroutine
            );

            ch01Coroutine = null;
        }


        // -----------------------------------------------------
        // ch02
        // -----------------------------------------------------

        if (ch02Coroutine != null)
        {
            StopCoroutine(
                ch02Coroutine
            );

            ch02Coroutine = null;
        }


        // -----------------------------------------------------
        // ch03
        // -----------------------------------------------------

        if (ch03Coroutine != null)
        {
            StopCoroutine(
                ch03Coroutine
            );

            ch03Coroutine = null;
        }


        // -----------------------------------------------------
        // 第二層背景恢復透明
        // -----------------------------------------------------

        SetImageAlpha(
            backgroundTransitionImage,
            0f
        );


        // -----------------------------------------------------
        // 立繪動畫狀態
        // -----------------------------------------------------

        ch01Animating = false;
        ch02Animating = false;
        ch03Animating = false;


        // -----------------------------------------------------
        // 清除立繪原始位置
        // -----------------------------------------------------

        ch01HasOriginalPosition = false;
        ch02HasOriginalPosition = false;
        ch03HasOriginalPosition = false;


        // -----------------------------------------------------
        // 其他動畫狀態
        // -----------------------------------------------------

        textAnimating = false;
        backgroundAnimating = false;

        isTextAnimating = false;
    }


    // =========================================================
    // 下一句
    // =========================================================

    void NextDialogue()
    {
        currentIndex++;


        if (
            currentIndex >=
            dialogueFile.dialogues.Count
        )
        {
            EndDialogue();

            return;
        }


        ShowDialogue();
    }


    // =========================================================
    // 結束對話
    // =========================================================

    void EndDialogue()
    {
        talking = false;


        StopAllAnimations();


        dialogueUI.SetActive(false);


        foreach (GameObject obj in enableObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }


        StopStory();


        gameObject.SetActive(false);
    }


    // =========================================================
    // Story
    // =========================================================

    void StartStory()
    {

    }


    void StopStory()
    {

    }
}