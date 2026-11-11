using UnityEngine;

// Pincho/bloque que se mueve a la izquierda y se destruye fuera de pantalla.
public class Obstacle : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameOver) return;
        transform.position += Vector3.left * GameManager.Instance.Speed * Time.deltaTime;
        if (transform.position.x < -12f) Destroy(gameObject);
    }
}
