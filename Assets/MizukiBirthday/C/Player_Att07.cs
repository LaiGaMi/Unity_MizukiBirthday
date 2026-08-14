using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Att07 : MonoBehaviour
{
    [Header("技能預置物")]
    public GameObject prefab01;
    public GameObject prefab02;
    public GameObject prefab03;

    [Header("敵人 Tag")]
    public string enemyTag = "Enemy";

    [Header("攻擊間隔")]
    public float attackInterval = 1f;

    private float attackTimer = 0f;


    private void Update()
    {
        // 沒有技能，不攻擊
        if (mizuki.att07 <= 0)
        {
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Attack();

            attackTimer = 0f;
        }
    }


    private void Attack()
    {
        // 找出場上所有敵人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        // 場上沒有敵人
        if (enemies.Length == 0)
        {
            return;
        }


        // =====================================================
        // Level 1
        // =====================================================

        if (mizuki.att07 == 1)
        {
            GameObject target = enemies[Random.Range(0, enemies.Length)];

            if (prefab01 != null)
            {
                Instantiate(
                    prefab01,
                    target.transform.position,
                    Quaternion.identity
                );
            }
        }


        // =====================================================
        // Level 2
        // =====================================================

        else if (mizuki.att07 == 2)
        {
            GameObject target = enemies[Random.Range(0, enemies.Length)];

            if (prefab02 != null)
            {
                Instantiate(
                    prefab02,
                    target.transform.position,
                    Quaternion.identity
                );
            }
        }


        // =====================================================
        // Level 3
        // 鎖定兩個不同敵人
        // =====================================================

        else if (mizuki.att07 == 3)
        {
            // 只有一個敵人時，只能生成一個
            if (enemies.Length == 1)
            {
                if (prefab03 != null)
                {
                    Instantiate(
                        prefab03,
                        enemies[0].transform.position,
                        Quaternion.identity
                    );
                }

                return;
            }


            // 第一個目標
            int firstIndex = Random.Range(0, enemies.Length);

            // 第二個目標
            int secondIndex = Random.Range(0, enemies.Length);

            // 確保兩個目標不同
            while (secondIndex == firstIndex)
            {
                secondIndex = Random.Range(0, enemies.Length);
            }


            // 生成第一個
            if (prefab03 != null)
            {
                Instantiate(
                    prefab03,
                    enemies[firstIndex].transform.position,
                    Quaternion.identity
                );


                // 生成第二個
                Instantiate(
                    prefab03,
                    enemies[secondIndex].transform.position,
                    Quaternion.identity
                );
            }
        }
    }
}