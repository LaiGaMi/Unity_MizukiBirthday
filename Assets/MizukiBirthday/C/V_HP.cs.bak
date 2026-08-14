using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V_HP : MonoBehaviour
{
    // =========================================================
    // 敵人設定
    // =========================================================

    [Header("敵人血量")]
    public float maxHP = 10f;

    private float currentHP;


    // =========================================================
    // EXP 設定
    // =========================================================

    [Header("擊殺 EXP")]
    public int expReward = 10;


    // =========================================================
    // att03 補血道具設定
    // =========================================================

    [Header("att03 補血道具")]

    // att03 = 1
    public GameObject healItem01;

    // att03 = 2
    public GameObject healItem02;

    // att03 = 3
    public GameObject healItem03;


    // 掉落機率
    [Header("掉落機率")]

    // 10%
    [Range(0f, 100f)]
    public float dropChance01 = 10f;

    // 10%
    [Range(0f, 100f)]
    public float dropChance02 = 10f;

    // 20%
    [Range(0f, 100f)]
    public float dropChance03 = 20f;


    // =========================================================
    // 子彈 Tag 設定
    // =========================================================

    [Header("子彈 Tag")]

    // 碰到後造成 1 點傷害
    public string normalBulletTag = "Bullet";

    // 碰到後刪除子彈
    public string destroyBulletTag = "DestroyBullet";

    // 持續接觸造成傷害
    public string continuousBulletTag = "ContinuousBullet";


    // =========================================================
    // 持續傷害設定
    // =========================================================

    [Header("持續傷害")]

    // 每次傷害
    public float continuousDamage = 0.1f;

    // 傷害間隔
    public float damageInterval = 0.1f;

    private float damageTimer = 0f;


    // =========================================================
    // 初始化
    // =========================================================

    private void Start()
    {
        currentHP = maxHP;
    }


    // =========================================================
    // Trigger
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        // -----------------------------------------------------
        // 一般子彈
        // -----------------------------------------------------

        if (other.CompareTag(normalBulletTag))
        {
            TakeDamage(1f);

            if (mizuki.att02 == 3 && mizuki.card == mizuki.cardMax)
            {
                TakeDamage(0.2f);
            }
        }


        // -----------------------------------------------------
        // 碰到後刪除子彈
        // -----------------------------------------------------

        if (other.CompareTag(destroyBulletTag))
        {
            TakeDamage(1f);

            if (mizuki.att02 == 3 && mizuki.card == mizuki.cardMax)
            {
                TakeDamage(0.2f);
            }

            Destroy(other.gameObject);
        }


        // -----------------------------------------------------
        // 持續傷害子彈
        // -----------------------------------------------------

        if (other.CompareTag(continuousBulletTag))
        {
            damageTimer = 0f;
        }
    }


    // =========================================================
    // 持續接觸
    // =========================================================

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(continuousBulletTag))
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                TakeDamage(continuousDamage);

                damageTimer = 0f;
            }
        }
    }


    // =========================================================
    // 離開碰撞
    // =========================================================

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(continuousBulletTag))
        {
            damageTimer = 0f;
        }
    }


    // =========================================================
    // 受到傷害
    // =========================================================

    private void TakeDamage(float damage)
    {
        currentHP -= damage;

        Debug.Log(
            gameObject.name +
            " HP：" +
            currentHP +
            "/" +
            maxHP
        );


        // HP 歸零
        if (currentHP <= 0)
        {
            Die();
        }
    }


    // =========================================================
    // 死亡
    // =========================================================

    private void Die()
    {
        // -----------------------------------------------------
        // 給玩家 EXP
        // -----------------------------------------------------

        mizuki.exp += expReward;

        Level level = FindObjectOfType<Level>();

        if (level != null)
        {
            level.CheckLevelUp();
        }


        // -----------------------------------------------------
        // att03 補血道具掉落
        // -----------------------------------------------------

        DropHealItem();


        // -----------------------------------------------------
        // 刪除敵人
        // -----------------------------------------------------

        Destroy(gameObject);
    }


    // =========================================================
    // 補血道具掉落
    // =========================================================

    private void DropHealItem()
    {
        GameObject item = null;
        float chance = 0f;


        // -----------------------------------------------------
        // 根據 att03 決定道具和機率
        // -----------------------------------------------------

        switch (mizuki.att03)
        {
            case 1:
                item = healItem01;
                chance = dropChance01;
                break;

            case 2:
                item = healItem02;
                chance = dropChance02;
                break;

            case 3:
                item = healItem03;
                chance = dropChance03;
                break;

            default:
                return;
        }


        // 沒有設定預置物
        if (item == null)
        {
            return;
        }


        // -----------------------------------------------------
        // 隨機判定
        // -----------------------------------------------------

        float randomValue = Random.Range(0f, 100f);

        if (randomValue < chance)
        {
            Instantiate(
                item,
                transform.position,
                Quaternion.identity
            );

            Debug.Log(
                "敵人掉落補血道具！ att03 = " +
                mizuki.att03
            );
        }
    }
}