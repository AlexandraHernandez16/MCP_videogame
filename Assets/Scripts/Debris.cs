using UnityEngine;

// Partícula simple de explosión.
public class Debris : MonoBehaviour
{
    public Vector2 vel;
    float life = 0.8f, t;
    SpriteRenderer sr;

    void Start() { sr = GetComponent<SpriteRenderer>(); }

    void Update()
    {
        t += Time.deltaTime;
        vel *= 1f - 2f * Time.deltaTime;
        transform.position += (Vector3)(vel * Time.deltaTime);
        var c = sr.color; c.a = 1f - t / life; sr.color = c;
        transform.localScale *= 1f - 1.5f * Time.deltaTime;
        if (t >= life) Destroy(gameObject);
    }
}
