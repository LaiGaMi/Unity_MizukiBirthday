using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_hand : MonoBehaviour
{
    // =========================================================
    // 敵人設定
    // =========================================================

    [Header("敵人血量")]
    public float maxHP = 10f;

    public float currentHP;


    // =========================================================
    // 受傷閃爍設定
    // =========================================================

    [Header("受傷閃爍")]

    // 受傷時顯示的顏色
    public Color hitFlashColor = Color.red;

    // 閃爍持續時間
    public float hitFlashDuration = 0.1f;

    // Boss 手部的 SpriteRenderer
    private SpriteRenderer[] spriteRenderers;

    // 記錄原本的顏色
    private Color[] originalColors;

    // 閃爍計時器
    private float hitFlashTimer = 0f;

    // 是否正在閃爍
    private bool isHitFlashing = false;


    // =========================================================
    // EXP 設定
    // =========================================================

    [Header("擊殺 EXP")]
    public int expReward = 10;

    public GameObject DieItem;


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

    [Header("一般子彈 Tag")]

    // 可以設定多個一般子彈 Tag
    public List<string> normalBulletTags = new List<string>()
    {
        "Bullet"
    };


    // 碰到後刪除子彈
    [Header("刪除子彈 Tag")]
    public string destroyBulletTag = "DestroyBullet";


    // 持續接觸造成傷害
    [Header("持續傷害子彈 Tag")]
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
    // Boss 附屬物：小怪生成設定
    // =========================================================

    [Header("Boss 附屬物：小怪生成")]

    // 小怪預置物清單
    public List<GameObject> minionPrefabs = new List<GameObject>();

    // 生成冷卻時間
    public float minionSpawnCooldown = 5f;

    // 最大小怪數量
    public int maxMinionCount = 5;

    // 目前生成的小怪數量
    private int currentMinionCount = 0;

    // 生成冷卻計時器
    private float minionSpawnTimer = 0f;


    // =========================================================
    // 初始化
    // =========================================================

    private void Start()
    {
        currentHP = maxHP;

        // 找自己以及所有子物件的 SpriteRenderer
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 建立原本顏色的陣列
        originalColors = new Color[spriteRenderers.Length];

        // 記錄所有 SpriteRenderer 原本的顏色
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                originalColors[i] = spriteRenderers[i].color;
            }
        }

        // 小怪生成計時器歸零
        minionSpawnTimer = 0f;
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        HandleMinionSpawn();

        // 處理受傷閃爍
        HandleHitFlash();
    }


    // =========================================================
    // 受傷閃爍處理
    // =========================================================

    private void HandleHitFlash()
    {
        // 沒有正在閃爍
        if (!isHitFlashing)
        {
            return;
        }

        // 倒數
        hitFlashTimer -= Time.deltaTime;

        // 時間到了
        if (hitFlashTimer <= 0f)
        {
            RestoreOriginalColor();
        }
    }


    // =========================================================
    // 開始受傷閃爍
    // =========================================================

    private void StartHitFlash()
    {
        // 如果 SpriteRenderer 還沒有找到
        if (spriteRenderers == null)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        // 如果原本顏色陣列還沒有建立
        if (originalColors == null ||
            originalColors.Length != spriteRenderers.Length)
        {
            originalColors =
                new Color[spriteRenderers.Length];

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null)
                {
                    originalColors[i] =
                        spriteRenderers[i].color;
                }
            }
        }

        // 開始閃爍
        isHitFlashing = true;

        // 每次受傷重新計時
        hitFlashTimer = hitFlashDuration;

        // 變成指定受傷顏色
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color =
                    hitFlashColor;
            }
        }
    }


    // =========================================================
    // 恢復原本顏色
    // =========================================================

    private void RestoreOriginalColor()
    {
        isHitFlashing = false;

        hitFlashTimer = 0f;

        if (spriteRenderers == null ||
            originalColors == null)
        {
            return;
        }

        // 恢復原本顏色
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null &&
                i < originalColors.Length)
            {
                spriteRenderers[i].color =
                    originalColors[i];
            }
        }
    }


    // =========================================================
    // Boss 附屬物：小怪生成處理
    // =========================================================

    private void HandleMinionSpawn()
    {
        // 沒有設定小怪預置物
        if (minionPrefabs == null || minionPrefabs.Count == 0)
        {
            return;
        }

        // 已經達到小怪生成上限
        if (currentMinionCount >= maxMinionCount)
        {
            return;
        }

        // 開始倒數
        minionSpawnTimer += Time.deltaTime;

        // 冷卻時間還沒到
        if (minionSpawnTimer < minionSpawnCooldown)
        {
            return;
        }

        // 冷卻完成，生成小怪
        SpawnRandomMinion();

        // 重置計時器
        minionSpawnTimer = 0f;
    }


    // =========================================================
    // 隨機生成小怪
    // =========================================================

    private void SpawnRandomMinion()
    {
        // 再次確認數量上限
        if (currentMinionCount >= maxMinionCount)
        {
            return;
        }

        // 從清單中隨機選擇
        int randomIndex =
            Random.Range(0, minionPrefabs.Count);

        GameObject selectedPrefab =
            minionPrefabs[randomIndex];

        // 清單中如果有空物件
        if (selectedPrefab == null)
        {
            return;
        }

        Audio.Instance.Play("SE_V05");

        // 在 Boss 附屬物目前的位置生成
        GameObject minion = Instantiate(
            selectedPrefab,
            transform.position,
            Quaternion.identity
        );

        // 小怪數量 +1
        currentMinionCount++;

        // 監控小怪死亡
        MinionSpawnTracker tracker =
            minion.AddComponent<MinionSpawnTracker>();

        tracker.owner = this;

        Debug.Log(
            gameObject.name +
            " 生成小怪：" +
            minion.name +
            "，目前數量：" +
            currentMinionCount +
            "/" +
            maxMinionCount
        );
    }


    // =========================================================
    // 小怪死亡後通知
    // =========================================================

    public void OnMinionDestroyed()
    {
        currentMinionCount--;

        // 防止數量變成負數
        if (currentMinionCount < 0)
        {
            currentMinionCount = 0;
        }

        // 小怪數量下降後重新開始倒數
        minionSpawnTimer = 0f;

        Debug.Log(
            gameObject.name +
            " 小怪消失，目前數量：" +
            currentMinionCount +
            "/" +
            maxMinionCount
        );
    }


    // =========================================================
    // Trigger
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        // -----------------------------------------------------
        // 一般子彈
        // -----------------------------------------------------

        if (HasNormalBulletTag(other))
        {
            TakeDamage(1f);

            if (mizuki.att02 == 3 &&
                mizuki.card == mizuki.cardMax)
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

            if (mizuki.att02 == 3 &&
                mizuki.card == mizuki.cardMax)
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
    // 判斷是否為一般子彈
    // =========================================================

    private bool HasNormalBulletTag(Collider2D other)
    {
        if (normalBulletTags == null)
        {
            return false;
        }

        foreach (string tag in normalBulletTags)
        {
            // Tag 沒有設定
            if (string.IsNullOrEmpty(tag))
            {
                continue;
            }

            if (other.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
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
        // -----------------------------------------------------
        // 播放受傷閃爍
        // -----------------------------------------------------

        StartHitFlash();

        Audio.Instance.Play("SE_Vatt");


        // -----------------------------------------------------
        // 扣血
        // -----------------------------------------------------

        currentHP -= damage;

        Debug.Log(
            gameObject.name +
            " HP：" +
            currentHP +
            "/" +
            maxHP
        );


        // -----------------------------------------------------
        // HP 歸零
        // -----------------------------------------------------

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
        // 死亡前先恢復原本顏色
        RestoreOriginalColor();

        Audio.Instance.Play("SE_V00");

        Instantiate(
            DieItem,
            transform.position,
            Quaternion.identity
        );

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

        float randomValue =
            Random.Range(0f, 100f);

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


// =============================================================
// 小怪生成監控器
// =============================================================

public class MinionSpawnTracker : MonoBehaviour
{
    public Boss_hand owner;

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.OnMinionDestroyed();
        }
    }
}