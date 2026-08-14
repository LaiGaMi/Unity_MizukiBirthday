using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Dam01 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 朝自身正前方移動
        rb.velocity = transform.right * speed;
    }
}
