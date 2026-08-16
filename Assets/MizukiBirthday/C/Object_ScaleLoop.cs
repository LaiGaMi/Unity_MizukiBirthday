using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_ScaleLoop : MonoBehaviour
{
    // =========================================================
    // 縮放設定
    // =========================================================

    [Header("目標縮放 XYZ")]
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);

    [Header("縮放時間")]
    public float scaleTime = 1f;


    // =========================================================
    // 原始縮放
    // =========================================================

    private Vector3 originalScale;


    // =========================================================
    // 初始化
    // =========================================================

    private void Start()
    {
        // 記錄物件一開始的縮放
        originalScale = transform.localScale;

        // 開始循環縮放
        StartCoroutine(ScaleLoopCoroutine());
    }


    // =========================================================
    // 縮放循環
    // =========================================================

    private IEnumerator ScaleLoopCoroutine()
    {
        while (true)
        {
            // -------------------------------------------------
            // 原始縮放 → 目標縮放
            // -------------------------------------------------

            yield return StartCoroutine(
                ScaleTo(originalScale, targetScale)
            );


            // -------------------------------------------------
            // 目標縮放 → 原始縮放
            // -------------------------------------------------

            yield return StartCoroutine(
                ScaleTo(targetScale, originalScale)
            );
        }
    }


    // =========================================================
    // 執行縮放
    // =========================================================

    private IEnumerator ScaleTo(Vector3 startScale, Vector3 endScale)
    {
        float timer = 0f;

        while (timer < scaleTime)
        {
            timer += Time.deltaTime;

            float t = timer / scaleTime;

            // 防止超過 0 ~ 1
            t = Mathf.Clamp01(t);

            // SmoothStep 曲線
            t = Mathf.SmoothStep(0f, 1f, t);

            // 計算目前縮放
            transform.localScale = Vector3.Lerp(
                startScale,
                endScale,
                t
            );

            yield return null;
        }

        // 確保最後完全到達目標
        transform.localScale = endScale;
    }
}
