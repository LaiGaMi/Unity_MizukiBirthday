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


    // =========================================================
    // 共用目標佔用清單
    // =========================================================
    //
    // 用來避免同一批人偶在選擇目標時全部鎖定同一隻敵人。
    //
    private static HashSet<GameObject> reservedTargets =
        new HashSet<GameObject>();


    // 還剩多少次攻擊
    private int bounceCount;


    // 目前鎖定的敵人
    private GameObject currentTarget;


    // 已經攻擊過的敵人
    private List<GameObject> attackedEnemies =
        new List<GameObject>();


    // =========================================================
    // 設定彈跳次數
    // =========================================================

    public void SetBounceCount(int count)
    {
        bounceCount = count;

        FindTarget();
    }


    // =========================================================
    // Update
    // =========================================================

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
            ReleaseCurrentTarget();

            FindTarget();
            return;
        }


        // =====================================================
        // 朝目標移動
        // =====================================================

        Vector3 direction =
            currentTarget.transform.position -
            transform.position;

        direction.Normalize();


        transform.position +=
            direction * moveSpeed * Time.deltaTime;
    }


    // =========================================================
    // 尋找第一個目標
    // =========================================================

    private void FindTarget()
    {
        // 已經有目標就不用找
        if (currentTarget != null)
        {
            return;
        }


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
            if (enemy == null)
            {
                continue;
            }


            // -------------------------------------------------
            // 已經攻擊過的敵人不能再次攻擊
            // -------------------------------------------------

            if (attackedEnemies.Contains(enemy))
            {
                continue;
            }


            // -------------------------------------------------
            // 已經被其他人偶鎖定的敵人先跳過
            // -------------------------------------------------

            if (reservedTargets.Contains(enemy))
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


        // =====================================================
        // 找到不同的目標
        // =====================================================

        if (nearestEnemy != null)
        {
            SetCurrentTarget(nearestEnemy);
            return;
        }


        // =====================================================
        // 沒有「未被其他人偶鎖定」的敵人
        // =====================================================
        //
        // 例如：
        //
        // 3 個人偶
        // 只有 2 個敵人
        //
        // 第 3 個人偶就不可能取得不同目標。
        //
        // 這時候再嘗試找一個沒有攻擊過的敵人。
        // =====================================================

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }


            if (attackedEnemies.Contains(enemy))
            {
                continue;
            }


            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );


            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }


        if (nearestEnemy != null)
        {
            SetCurrentTarget(nearestEnemy);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // =========================================================
    // 設定目前目標
    // =========================================================

    private void SetCurrentTarget(GameObject target)
    {
        currentTarget = target;

        if (currentTarget != null)
        {
            reservedTargets.Add(currentTarget);
        }
    }


    // =========================================================
    // 釋放目前目標
    // =========================================================

    private void ReleaseCurrentTarget()
    {
        if (currentTarget == null)
        {
            return;
        }

        reservedTargets.Remove(currentTarget);

        currentTarget = null;
    }


    // =========================================================
    // 碰到敵人
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag))
        {
            return;
        }


        // =====================================================
        // 確認是不是目前鎖定的目標
        // =====================================================

        if (other.gameObject != currentTarget)
        {
            return;
        }


        // =====================================================
        // 記錄已經攻擊過
        // =====================================================

        attackedEnemies.Add(other.gameObject);


        // =====================================================
        // 釋放目前目標
        // =====================================================

        ReleaseCurrentTarget();


        // =====================================================
        // 攻擊次數 -1
        // =====================================================

        bounceCount--;


        // =====================================================
        // 次數結束
        // =====================================================

        if (bounceCount <= 0)
        {
            Destroy(gameObject);
            return;
        }


        // =====================================================
        // 尋找下一個目標
        // =====================================================

        currentTarget = FindNextTarget();


        // =====================================================
        // 有找到下一個目標
        // =====================================================

        if (currentTarget != null)
        {
            reservedTargets.Add(currentTarget);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // =========================================================
    // 找下一個目標
    // =========================================================

    private GameObject FindNextTarget()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(enemyTag);


        GameObject nearestEnemy = null;

        float nearestDistance = Mathf.Infinity;


        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }


            // -------------------------------------------------
            // 不選已經攻擊過的
            // -------------------------------------------------

            if (attackedEnemies.Contains(enemy))
            {
                continue;
            }


            // -------------------------------------------------
            // 不選目前被其他人偶鎖定的
            // -------------------------------------------------

            if (reservedTargets.Contains(enemy))
            {
                continue;
            }


            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );


            // -------------------------------------------------
            // 距離限制
            // -------------------------------------------------

            if (distance < minTargetDistance)
            {
                continue;
            }


            if (distance > maxTargetDistance)
            {
                continue;
            }


            // -------------------------------------------------
            // 找最近
            // -------------------------------------------------

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }


        return nearestEnemy;
    }


    // =========================================================
    // 人偶被刪除
    // =========================================================

    private void OnDestroy()
    {
        // 防止人偶消失後，敵人還一直被佔用
        ReleaseCurrentTarget();
    }
}