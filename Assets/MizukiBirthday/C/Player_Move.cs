using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player_Move : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Camera mainCamera;

    private Vector2 moveDirection = Vector2.zero;

    // 記錄角色最後面向
    private bool facingRight = true;


    // =========================================================
    // 滑鼠操作狀態
    // =========================================================

    // 滑鼠這一次按下是否允許移動
    private bool mouseCanMove = false;


    // =========================================================
    // 觸控操作狀態
    // =========================================================

    // 目前這根手指是否允許移動
    private bool touchCanMove = false;

    // 目前追蹤的手指 ID
    private int activeFingerId = -1;


    // =========================================================
    // 初始化
    // =========================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // 如果沒有手動綁定 Animator，就自動尋找
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        // =====================================================
        // 如果遊戲暫停
        // =====================================================

        if (Time.timeScale == 0f)
        {
            StopMoving();
            UpdateAnimation();
            return;
        }


        // =====================================================
        // 滑鼠
        // =====================================================

        HandleMouseInput();


        // =====================================================
        // 觸控
        // =====================================================

        HandleTouchInput();


        // =====================================================
        // 更新動畫
        // =====================================================

        UpdateAnimation();
    }


    // =========================================================
    // 滑鼠操作
    // =========================================================

    private void HandleMouseInput()
    {
        // -----------------------------------------------------
        // 滑鼠剛按下
        // -----------------------------------------------------

        if (Input.GetMouseButtonDown(0))
        {
            // -------------------------------------------------
            // 只在按下的瞬間判斷是不是 Button
            // -------------------------------------------------

            if (IsPointerOverButton())
            {
                // 按到 Button
                // 這一次滑鼠操作不允許移動

                mouseCanMove = false;

                StopMoving();
            }
            else
            {
                // 沒有按到 Button
                // 允許這次操作移動

                mouseCanMove = true;

                UpdateMoveDirection(
                    Input.mousePosition
                );
            }
        }


        // -----------------------------------------------------
        // 滑鼠持續按住
        // -----------------------------------------------------

        if (Input.GetMouseButton(0))
        {
            // -------------------------------------------------
            // 只看「按下時」是否允許移動
            //
            // 這裡不再判斷 UI
            //
            // 所以如果：
            //
            // 空白開始
            // ↓
            // 移動到 UI
            //
            // 仍然可以繼續移動
            // -------------------------------------------------

            if (mouseCanMove)
            {
                UpdateMoveDirection(
                    Input.mousePosition
                );
            }
            else
            {
                StopMoving();
            }
        }


        // -----------------------------------------------------
        // 滑鼠放開
        // -----------------------------------------------------

        if (Input.GetMouseButtonUp(0))
        {
            mouseCanMove = false;

            StopMoving();
        }
    }


    // =========================================================
    // 觸控操作
    // =========================================================

    private void HandleTouchInput()
    {
        // -----------------------------------------------------
        // 沒有觸控
        // -----------------------------------------------------

        if (Input.touchCount == 0)
        {
            return;
        }


        // =====================================================
        // 如果目前沒有追蹤手指
        // =====================================================

        if (activeFingerId == -1)
        {
            Touch touch = Input.GetTouch(0);


            // -------------------------------------------------
            // 只在手指剛開始接觸時判斷 Button
            // -------------------------------------------------

            if (touch.phase == TouchPhase.Began)
            {
                activeFingerId =
                    touch.fingerId;


                // -------------------------------------------------
                // 判斷按下位置是不是 Button
                // -------------------------------------------------

                if (IsTouchOverButton(
                    touch.fingerId))
                {
                    // 按到 Button
                    // 這一次觸控不允許移動

                    touchCanMove = false;

                    StopMoving();
                }
                else
                {
                    // 沒有按到 Button
                    // 允許這次觸控移動

                    touchCanMove = true;

                    UpdateMoveDirection(
                        touch.position
                    );
                }
            }

            return;
        }


        // =====================================================
        // 找到目前正在控制的手指
        // =====================================================

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch =
                Input.GetTouch(i);


            // -------------------------------------------------
            // 不是目前追蹤的手指
            // -------------------------------------------------

            if (touch.fingerId != activeFingerId)
            {
                continue;
            }


            // =================================================
            // 手指移動 / 持續按住
            // =================================================

            if (
                touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary
            )
            {
                // -------------------------------------------------
                // 這裡不判斷 UI
                //
                // 因此：
                //
                // 空白開始
                // ↓
                // 移到 UI
                // ↓
                // 仍然可以移動
                // -------------------------------------------------

                if (touchCanMove)
                {
                    UpdateMoveDirection(
                        touch.position
                    );
                }
                else
                {
                    StopMoving();
                }
            }


            // =================================================
            // 手指離開
            // =================================================

            else if (
                touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled
            )
            {
                touchCanMove = false;
                activeFingerId = -1;

                StopMoving();
            }

            break;
        }
    }


    // =========================================================
    // 判斷滑鼠按下位置是不是 Button
    // =========================================================

    private bool IsPointerOverButton()
    {
        if (EventSystem.current == null)
        {
            return false;
        }


        PointerEventData pointerData =
            new PointerEventData(
                EventSystem.current
            );


        pointerData.position =
            Input.mousePosition;


        List<RaycastResult> results =
            new List<RaycastResult>();


        EventSystem.current.RaycastAll(
            pointerData,
            results
        );


        // -----------------------------------------------------
        // 檢查所有被滑鼠碰到的 UI
        // -----------------------------------------------------

        foreach (RaycastResult result in results)
        {
            // -------------------------------------------------
            // 直接是 Button
            // -------------------------------------------------

            Button button =
                result.gameObject.GetComponent<Button>();


            if (button != null)
            {
                return true;
            }


            // -------------------------------------------------
            // 如果點到 Button 的子物件
            // 往父物件尋找 Button
            // -------------------------------------------------

            button =
                result.gameObject.GetComponentInParent<Button>();


            if (button != null)
            {
                return true;
            }
        }


        return false;
    }


    // =========================================================
    // 判斷觸控位置是不是 Button
    // =========================================================

    private bool IsTouchOverButton(int fingerId)
    {
        if (EventSystem.current == null)
        {
            return false;
        }


        PointerEventData pointerData =
            new PointerEventData(
                EventSystem.current
            );


        pointerData.position =
            GetTouchPosition(fingerId);


        List<RaycastResult> results =
            new List<RaycastResult>();


        EventSystem.current.RaycastAll(
            pointerData,
            results
        );


        // -----------------------------------------------------
        // 檢查所有被觸控到的 UI
        // -----------------------------------------------------

        foreach (RaycastResult result in results)
        {
            // -------------------------------------------------
            // 直接是 Button
            // -------------------------------------------------

            Button button =
                result.gameObject.GetComponent<Button>();


            if (button != null)
            {
                return true;
            }


            // -------------------------------------------------
            // 如果點到 Button 的子物件
            // 往父物件尋找 Button
            // -------------------------------------------------

            button =
                result.gameObject.GetComponentInParent<Button>();


            if (button != null)
            {
                return true;
            }
        }


        return false;
    }


    // =========================================================
    // 取得指定手指的位置
    // =========================================================

    private Vector2 GetTouchPosition(int fingerId)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch =
                Input.GetTouch(i);


            if (touch.fingerId == fingerId)
            {
                return touch.position;
            }
        }


        return Vector2.zero;
    }


    // =========================================================
    // FixedUpdate
    // =========================================================

    private void FixedUpdate()
    {
        // -----------------------------------------------------
        // 遊戲暫停時停止玩家
        // -----------------------------------------------------

        if (Time.timeScale == 0f)
        {
            rb.velocity = Vector2.zero;
            return;
        }


        // -----------------------------------------------------
        // 移動速度
        // -----------------------------------------------------

        float currentSpeed =
            moveSpeed + mizuki.att04;


        // -----------------------------------------------------
        // 套用速度
        // -----------------------------------------------------

        rb.velocity =
            moveDirection * currentSpeed;
    }


    // =========================================================
    // 更新移動方向
    // =========================================================

    private void UpdateMoveDirection(
        Vector2 screenPosition
    )
    {
        // =====================================================
        // 滑鼠 / 手指位置
        // 螢幕座標 → 世界座標
        // =====================================================

        Vector3 inputWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    -mainCamera.transform.position.z
                )
            );


        // =====================================================
        // 玩家自身位置
        // =====================================================

        Vector3 playerWorldPosition =
            transform.position;


        // =====================================================
        // 玩家 → 滑鼠 / 手指
        // =====================================================

        Vector2 direction =
            (Vector2)(
                inputWorldPosition -
                playerWorldPosition
            );


        // =====================================================
        // 判斷方向
        // =====================================================

        if (direction.sqrMagnitude > 0.01f)
        {
            moveDirection =
                direction.normalized;


            // -------------------------------------------------
            // 判斷左右方向
            // -------------------------------------------------

            if (moveDirection.x > 0.01f)
            {
                facingRight = true;
            }
            else if (moveDirection.x < -0.01f)
            {
                facingRight = false;
            }
        }
        else
        {
            moveDirection =
                Vector2.zero;
        }
    }


    // =========================================================
    // 停止移動
    // =========================================================

    private void StopMoving()
    {
        moveDirection =
            Vector2.zero;
    }


    // =========================================================
    // 更新動畫
    // =========================================================

    private void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }


        bool isMoving =
            moveDirection.sqrMagnitude > 0.01f;


        // -----------------------------------------------------
        // Move
        // -----------------------------------------------------

        animator.SetBool(
            "move",
            isMoving
        );


        // -----------------------------------------------------
        // 左右方向
        // -----------------------------------------------------

        animator.SetBool(
            "R",
            facingRight
        );

        animator.SetBool(
            "L",
            !facingRight
        );
    }


    // =========================================================
    // 取得目前移動方向
    // =========================================================

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }
}