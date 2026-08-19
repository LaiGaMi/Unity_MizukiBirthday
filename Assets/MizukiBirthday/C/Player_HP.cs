using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_HP : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [Header("Damage")]

    // 可以造成玩家傷害的 Tag
    [SerializeField] private List<string> damageTags = new List<string>()
    {
        "Enemy",
        "EnemyBullet",
        "EnemyAttack"
    };

    [SerializeField] private float damageAmount = 5f;
    [SerializeField] private float damageCooldown = 1f;


    [Header("Card Defense")]
    [SerializeField] private float cardDefenseCooldown = 1f;


    [Header("HP Bar")]
    [SerializeField] private Image hpBar;


    [Header("Player Image")]
    [SerializeField] private SpriteRenderer playerSprite;
	
	[Header("Death Object")]
	[SerializeField] private GameObject deathObject;

    // =========================================================
    // 玩家自身碰撞箱
    // =========================================================

    [Header("Player Collider")]

    [Tooltip("只接受這個物件本身的 Collider2D，不包含子物件的 Collider2D。")]
    [SerializeField] private Collider2D playerCollider;


    // =========================================================
    // 回血道具
    // =========================================================

    [Header("Heal Item Tag")]

    [SerializeField] private string healItem01Tag = "HealItem01";
    [SerializeField] private string healItem02Tag = "HealItem02";
    [SerializeField] private string healItem03Tag = "HealItem03";


    [Header("Heal Amount")]

    [SerializeField] private float healAmount01 = 10f;
    [SerializeField] private float healAmount02 = 25f;
    [SerializeField] private float healAmount03 = 50f;


    // 一般受傷無敵
    private bool isInvincible = false;

    // 塔羅牌防禦冷卻
    private bool isCardDefenseCooldown = false;


    private void Awake()
    {
        currentHP = maxHP;

        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
        }

        // 如果沒有手動指定，
        // 自動取得「腳本所在物件」上的 Collider2D。
        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }

        UpdateHPBar();
    }


    // =========================================================
    // 玩家碰撞
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        // =====================================================
        // 只接受 Player_HP 所在 GameObject 自己的 Collider2D
        // =====================================================

        if (playerCollider == null)
        {
            return;
        }


        // 如果觸發這個事件的碰撞箱不是玩家自身指定的碰撞箱，
        // 直接忽略。
        //
        // 子物件上的 Collider2D 不會進行任何判定。
        if (playerCollider.gameObject != gameObject)
        {
            return;
        }


        // =====================================================
        // 回血道具
        // =====================================================

        if (other.CompareTag(healItem01Tag))
        {
            Heal(healAmount01, other.gameObject);
            return;
        }

        if (other.CompareTag(healItem02Tag))
        {
            Heal(healAmount02, other.gameObject);
            return;
        }

        if (other.CompareTag(healItem03Tag))
        {
            Heal(healAmount03, other.gameObject);
            return;
        }


        // =====================================================
        // 一般受傷
        // =====================================================

        if (isInvincible)
        {
            return;
        }

        if (isCardDefenseCooldown)
        {
            return;
        }


        // =====================================================
        // 判斷是否為可以造成傷害的 Tag
        // =====================================================

        if (damageTags.Contains(other.tag))
        {
            TakeDamage(damageAmount);
        }
    }


    // =========================================================
    // 回復生命值
    // =========================================================

    private void Heal(float amount, GameObject healItem)
    {
        if (currentHP >= maxHP)
        {
            return;
        }
		Audio.Instance.Play("SE_mizuki03");

        currentHP += amount;

        currentHP = Mathf.Min(currentHP, maxHP);

        UpdateHPBar();

        Debug.Log(
            "玩家回血 " +
            amount +
            "，目前 HP：" +
            currentHP +
            "/" +
            maxHP
        );

        Destroy(healItem);
    }


    // =========================================================
    // 受到傷害
    // =========================================================

    private void TakeDamage(float damage)
    {
        // =====================================================
        // 有塔羅牌
        // =====================================================

        if (mizuki.card > 0)
        {
			Audio.Instance.Play("SE_mizukiAtt02");
			
            mizuki.card--;

            Debug.Log(
                "塔羅牌防禦！剩餘塔羅牌：" +
                mizuki.card
            );

            StartCoroutine(CardDefenseCooldown());

            return;
        }


        // =====================================================
        // 沒有塔羅牌
        // =====================================================
		Audio.Instance.Play("SE_mizukiAtt");

        currentHP -= damage;

        currentHP = Mathf.Max(currentHP, 0f);

        UpdateHPBar();

        StartCoroutine(DamageCooldown());


        if (currentHP <= 0f)
        {
            Die();
        }
    }


    // =========================================================
    // 一般受傷無敵
    // =========================================================

    private IEnumerator DamageCooldown()
    {
        isInvincible = true;

        SetPlayerColor(1f, 1f, 1f, 0.5f);

        yield return new WaitForSeconds(damageCooldown);

        SetPlayerColor(1f, 1f, 1f, 1f);

        isInvincible = false;
    }


    // =========================================================
    // 塔羅牌防禦冷卻
    // =========================================================

    private IEnumerator CardDefenseCooldown()
    {
        isCardDefenseCooldown = true;

        SetPlayerColor(0.5f, 0.7f, 1f, 0.5f);

        yield return new WaitForSeconds(cardDefenseCooldown);

        SetPlayerColor(1f, 1f, 1f, 1f);

        isCardDefenseCooldown = false;
    }


    // =========================================================
    // 設定玩家顏色與透明度
    // =========================================================

    private void SetPlayerColor(
        float r,
        float g,
        float b,
        float a)
    {
        if (playerSprite == null)
        {
            return;
        }

        Color color = new Color(r, g, b, a);

        playerSprite.color = color;
    }


    // =========================================================
    // HP Bar
    // =========================================================

    private void UpdateHPBar()
    {
        if (hpBar == null)
        {
            return;
        }

        if (maxHP <= 0f)
        {
            hpBar.fillAmount = 0f;
            return;
        }

        hpBar.fillAmount = currentHP / maxHP;
    }


    // =========================================================
    // 死亡
    // =========================================================

    private void Die()
    {
        Debug.Log("Player HP = 0");

        deathObject.SetActive(true);
		Destroy(gameObject);
    }


    // =========================================================
    // 取得目前 HP
    // =========================================================

    public float GetCurrentHP()
    {
        return currentHP;
    }


    // =========================================================
    // 取得最大 HP
    // =========================================================

    public float GetMaxHP()
    {
        return maxHP;
    }
}