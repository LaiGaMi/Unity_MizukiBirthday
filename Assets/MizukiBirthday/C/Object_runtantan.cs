using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_runtantan : MonoBehaviour
{
    [Header("敵人設定")]
    public string enemyTag = "Enemy";


    [Header("移動速度")]
    public float moveSpeed = 8f;


    [Header("下一個目標距離")]
    public float minTargetDistance = 0f;
    public float maxTargetDistance = 5f;


    // 還剩多少次彈跳
    private int bounceCount;


    // 目前鎖定的敵人
    private GameObject currentTarget;


    // 已經攻擊過的敵人
    private List<GameObject> attackedEnemies =
        new List<GameObject>();


    // =========================================
    // 設定彈跳次數
    // =========================================

    public void SetBounceCount(int count)
    {
        bounceCount = count;

        FindTarget();
    }


    // =========================================
    // 更新
    // =========================================

    private void Update()
    {
        // 沒有目標
        if (currentTarget == null)
        {
            FindTarget();
            return;
        }


        // 目標已經被刪除
        if (!currentTarget.activeInHierarchy)
        {
            FindTarget();
            return;
        }


        // =====================================
        // 朝目標移動
        // =====================================

        Vector3 direction =
            currentTarget.transform.position -
            transform.position;

        direction.Normalize();


        transform.position +=
            direction * moveSpeed * Time.deltaTime;
    }


    // =========================================
    // 搜尋第一個目標
    // =========================================

    private void FindTarget()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(enemyTag);


        if (enemies.Length == 0)
        {
            Destroy(gameObject);
            return;
        }


        GameObject nearestEnemy = null;

        float nearestDistance = Mathf.Infinity;


        foreach (GameObject enemy in enemies)
        {
            // 不選已經攻擊過的
            if (attackedEnemies.Contains(enemy))
            {
                continue;
            }


            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );


            // 找最近
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }


        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // =========================================
    // 碰到敵人
    // =========================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag))
        {
            return;
        }


        // 確認是不是目前目標
        if (other.gameObject != currentTarget)
        {
            return;
        }


        // 記錄這個敵人已經被攻擊
        attackedEnemies.Add(other.gameObject);


        // 彈跳次數 -1
        bounceCount--;


        // =====================================
        // 彈跳次數結束
        // =====================================

        if (bounceCount <= 0)
        {
            Destroy(gameObject);
            return;
        }


        // =====================================
        // 尋找下一個目標
        // =====================================

        currentTarget = FindNextTarget();
    }


    // =========================================
    // 找下一個目標
    // =========================================

    private GameObject FindNextTarget()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(enemyTag);


        GameObject nearestEnemy = null;

        float nearestDistance = Mathf.Infinity;


        foreach (GameObject enemy in enemies)
        {
            // 不選已經攻擊過的
            if (attackedEnemies.Contains(enemy))
            {
                continue;
            }


            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );


            // 距離不符合
            if (distance < minTargetDistance)
            {
                continue;
            }

            if (distance > maxTargetDistance)
            {
                continue;
            }


            // 找最近
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }


        return nearestEnemy;
    }
}