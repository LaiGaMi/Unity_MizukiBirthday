using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Att02 : MonoBehaviour
{
    // =========================================================
    // 塔羅牌設定
    // =========================================================

    [Header("塔羅牌預置物")]
    public GameObject cardPrefab;

    [Header("塔羅牌父物件")]
    public Transform cardParent;

    [Header("塔羅牌距離玩家的距離")]
    public float cardDistance = 1.5f;

    [Header("補充塔羅牌時間")]
    public float cardInterval = 5f;


    // =========================================================
    // 計時
    // =========================================================

    private float cardTimer = 0f;


    // =========================================================
    // 已生成的塔羅牌
    // =========================================================

    private List<GameObject> cards = new List<GameObject>();


    // =========================================================
    // 初始化
    // =========================================================

    private void Start()
    {
        UpdateCardMax();

        UpdateCards();
    }


    // =========================================================
    // 每幀
    // =========================================================

    private void Update()
    {
        // 更新最大數量
        UpdateCardMax();


        // 如果技能沒有啟用
        if (mizuki.att02 <= 0)
        {
            ClearCards();

            return;
        }


        // =====================================================
        // 如果目前數量小於最大數量
        // =====================================================

        if (mizuki.card < mizuki.cardMax)
        {
            cardTimer += Time.deltaTime;

            if (cardTimer >= cardInterval)
            {
                mizuki.card++;

                cardTimer = 0f;

                UpdateCards();
            }
        }
        else
        {
            cardTimer = 0f;
        }


        // =====================================================
        // 確認實際塔羅牌數量
        // =====================================================

        if (cards.Count != mizuki.card)
        {
            UpdateCards();
        }
    }


    // =========================================================
    // 根據 att02 設定最大塔羅牌數量
    // =========================================================

    private void UpdateCardMax()
    {
        switch (mizuki.att02)
        {
            case 0:
                mizuki.cardMax = 0;
                break;

            case 1:
                mizuki.cardMax = 1;
                break;

            case 2:
                mizuki.cardMax = 3;
                break;

            case 3:
                mizuki.cardMax = 5;
                break;
        }


        // 防止目前數量超過最大值
        if (mizuki.card > mizuki.cardMax)
        {
            mizuki.card = mizuki.cardMax;
        }
    }


    // =========================================================
    // 更新塔羅牌
    // =========================================================

    private void UpdateCards()
    {
        // 先清除舊塔羅牌
        ClearCards();


        // 沒有塔羅牌
        if (mizuki.card <= 0)
        {
            return;
        }


        // 沒有設定父物件
        if (cardParent == null)
        {
            Debug.LogWarning("Player_Att02 尚未設定 Card Parent！");
            return;
        }


        // =====================================================
        // 生成塔羅牌
        // =====================================================

        for (int i = 0; i < mizuki.card; i++)
        {
            // 計算角度
            float angle =
                (360f / mizuki.card) * i;


            // 轉換成圓周座標
            float radian =
                angle * Mathf.Deg2Rad;


            Vector3 position =
                new Vector3(
                    Mathf.Cos(radian),
                    Mathf.Sin(radian),
                    0f
                ) * cardDistance;


            // =================================================
            // 生成塔羅牌
            // =================================================

            GameObject newCard =
                Instantiate(
                    cardPrefab,
                    cardParent
                );


            // =================================================
            // 設定為父物件的子物件
            // =================================================

            newCard.transform.localPosition = position;


            // =================================================
            // 設定塔羅牌角度
            // =================================================

            newCard.transform.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle
                );


            // 記錄
            cards.Add(newCard);
        }
    }


    // =========================================================
    // 清除所有塔羅牌
    // =========================================================

    private void ClearCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
            {
                Destroy(cards[i]);
            }
        }

        cards.Clear();
    }
}