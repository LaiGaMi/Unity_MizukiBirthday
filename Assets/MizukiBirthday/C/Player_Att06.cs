using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Att06 : MonoBehaviour
{
    [Header("人偶架預置物")]
    public GameObject puppetPrefab;

    [Header("生成位置")]
    public Transform spawnPoint;

    [Header("生成間隔")]
    public float attackInterval = 2f;

    private float attackTimer = 0f;


    private void Update()
    {
        // 沒有技能
        if (mizuki.att06 <= 0)
        {
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            SpawnPuppets();

            attackTimer = 0f;
        }
    }


    // =========================================================
    // 生成技能人偶
    // =========================================================

    private void SpawnPuppets()
    {
        int puppetCount = 0;
        int bounceCount = 0;


        // =====================================================
        // 根據技能等級設定
        // =====================================================

        switch (mizuki.att06)
        {
            case 1:
                puppetCount = 1;
                bounceCount = 2;
                break;

            case 2:
                puppetCount = 2;
                bounceCount = 4;
                break;

            case 3:
                puppetCount = 3;
                bounceCount = 6;
                break;

            default:
                return;
        }


        // =====================================================
        // 生成指定數量的人偶
        // =====================================================
		
		Audio.Instance.Play("SE_mizuki06");

        for (int i = 0; i < puppetCount; i++)
        {
            if (puppetPrefab == null)
            {
                Debug.LogWarning("Player_Att06：沒有設定 puppetPrefab。");
                return;
            }

            if (spawnPoint == null)
            {
                Debug.LogWarning("Player_Att06：沒有設定 spawnPoint。");
                return;
            }


            GameObject puppet = Instantiate(
                puppetPrefab,
                spawnPoint.position,
                Quaternion.identity
            );


            // =================================================
            // 設定人偶彈射次數
            // =================================================

            Object_runtantan puppetScript =
                puppet.GetComponent<Object_runtantan>();

            if (puppetScript != null)
            {
                puppetScript.SetBounceCount(bounceCount);
            }
            else
            {
                Debug.LogWarning(
                    "Player_Att06：生成的人偶沒有 Object_runtantan。"
                );
            }
        }
    }
}