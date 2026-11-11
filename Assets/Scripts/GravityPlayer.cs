using UnityEngine;
using UnityEngine.InputSystem;

// Bolita luminosa que corre sola. Espacio / clic = invertir la gravedad.
[RequireComponent(typeof(Rigidbody2D))]
public class GravityPlayer : MonoBehaviour
{
    public float gravity = 4f;
    Rigidbody2D rb;
    int dir = 1; // 1 = cae hacia abajo, -1 = cae hacia arriba
    float squash;
    const float Size = 0.9f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.GameOver) return;

        bool pressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                    || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);
        if (pressed)
        {
            dir = -dir;
            rb.gravityScale = gravity * dir;
            squash = 1f;
        }

        // Efecto de estiramiento al cambiar de gravedad
        squash = Mathf.MoveTowards(squash, 0f, Time.deltaTime * 4f);
        transform.localScale = new Vector3(Size * (1f - 0.3f * squash), Size * (1f + 0.4f * squash), 1);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.GetComponent<Obstacle>() != null)
            GameManager.Instance.Die(transform.position);
    }
}
