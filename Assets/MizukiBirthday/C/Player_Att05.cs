using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Att05 : MonoBehaviour
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
        // 沒有技能
        if (mizuki.att05 <= 0)
        {
            return;
        }

        // 攻擊計時
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Attack();

            attackTimer = 0f;
        }
    }


    // =========================================================
    // 攻擊
    // =========================================================

    private void Attack()
    {
        // 場上所有敵人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        // 沒有敵人
        if (enemies.Length == 0)
        {
            return;
        }
		
		Audio.Instance.Play("SE_mizuki05");

        // =====================================================
        // 決定這次攻擊要鎖定幾個敵人
        // =====================================================

        int targetCount = mizuki.att05;

        // 如果敵人數量不足
        // 就只能鎖定場上實際存在的敵人數量
        targetCount = Mathf.Min(targetCount, enemies.Length);


        // =====================================================
        // 建立可選擇的敵人清單
        // =====================================================

        List<GameObject> availableEnemies =
            new List<GameObject>(enemies);


        // =====================================================
        // 根據技能等級選擇預置物
        // =====================================================

        GameObject attackPrefab = null;

        switch (mizuki.att05)
        {
            case 1:
                attackPrefab = prefab01;
                break;

            case 2:
                attackPrefab = prefab02;
                break;

            case 3:
                attackPrefab = prefab03;
                break;
        }


        if (attackPrefab == null)
        {
            return;
        }


        // =====================================================
        // 隨機選擇敵人
        // =====================================================

        for (int i = 0; i < targetCount; i++)
        {
            // 隨機選一個敵人
            int randomIndex =
                Random.Range(0, availableEnemies.Count);

            GameObject target =
                availableEnemies[randomIndex];


            // =================================================
            // 在敵人位置生成預置物
            // =================================================
			
            Instantiate(
                attackPrefab,
                target.transform.position,
                Quaternion.identity
            );


            // =================================================
            // 從可選清單移除
            // 避免同一次攻擊重複鎖定
            // =================================================

            availableEnemies.RemoveAt(randomIndex);
        }
    }
}