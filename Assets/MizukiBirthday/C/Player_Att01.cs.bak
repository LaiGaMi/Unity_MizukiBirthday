using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Att01 : MonoBehaviour
{
    [Header("子彈預置物")]
    public GameObject bullet01;
    public GameObject bullet02;
    public GameObject bullet03;

    [Header("子彈生成位置")]
    public Transform firePoint;

    [Header("攻擊間隔")]
    public float attackInterval = 0.5f;

    private float attackTimer = 0f;

    private Player_Move playerMove;

    // 玩家最後一次的移動方向
    private Vector2 lastDirection = Vector2.right;


    private void Awake()
    {
        playerMove = GetComponent<Player_Move>();
    }


    private void Update()
    {
        // =========================================
        // 取得玩家目前移動方向
        // =========================================

        Vector2 direction = playerMove.GetMoveDirection();

        // 玩家正在移動
        if (direction.sqrMagnitude > 0.01f)
        {
            lastDirection = direction.normalized;
        }


        // =========================================
        // 沒有技能
        // =========================================

        if (mizuki.att01 <= 0)
        {
            return;
        }


        // =========================================
        // 攻擊計時
        // =========================================

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Attack();

            attackTimer = 0f;
        }
    }


    private void Attack()
    {
        GameObject bullet = null;


        // =========================================
        // 根據 att01 決定子彈
        // =========================================

        switch (mizuki.att01)
        {
            case 1:
                bullet = bullet01;
                break;

            case 2:
                bullet = bullet02;
                break;

            case 3:
                bullet = bullet03;
                break;
        }


        if (bullet == null)
        {
            return;
        }


        // =========================================
        // 使用最後移動方向
        // =========================================

        float angle = Mathf.Atan2(
            lastDirection.y,
            lastDirection.x
        ) * Mathf.Rad2Deg;


        // =========================================
        // 生成子彈
        // =========================================

        Instantiate(
            bullet,
            firePoint.position,
            Quaternion.Euler(0f, 0f, angle)
        );
    }
}