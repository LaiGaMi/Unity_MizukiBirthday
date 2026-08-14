using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Big : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private Vector3 targetScale = new Vector3(3f, 3f, 3f);
    [SerializeField] private float scaleDuration = 1f;

    [Header("Stay Settings")]
    [SerializeField] private float stayDuration = 2f;

    [Header("Scale Curve")]
    [SerializeField] private AnimationCurve scaleCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 startScale;
    private float timer;

    private void Start()
    {
        // 記錄物件生成時的大小
        startScale = transform.localScale;

        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // =========================
        // 放大階段
        // =========================

        if (timer < scaleDuration)
        {
            float progress = Mathf.Clamp01(timer / scaleDuration);

            // 使用曲線控制放大速度
            float curveValue = scaleCurve.Evaluate(progress);

            // 從起始大小放大到最大大小
            transform.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                curveValue
            );
        }
        else
        {
            // 確保維持最大大小
            transform.localScale = targetScale;

            // =========================
            // 停留階段
            // =========================

            float stayTimer = timer - scaleDuration;

            if (stayTimer >= stayDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}
