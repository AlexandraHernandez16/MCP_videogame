using UnityEngine;

// Fondo de estrellas con parallax.
public class Stars : MonoBehaviour
{
    public Sprite dot;
    public int count = 70;
    Transform[] stars;
    float[] speeds;

    void Start()
    {
        stars = new Transform[count];
        speeds = new float[count];
        for (int i = 0; i < count; i++)
        {
            var g = new GameObject("Star");
            g.transform.SetParent(transform);
            float s = Random.Range(0.04f, 0.18f);
            g.transform.localScale = Vector3.one * s;
            g.transform.position = new Vector3(Random.Range(-12f, 12f), Random.Range(-4.5f, 4.5f), 5);
            var sr = g.AddComponent<SpriteRenderer>();
            sr.sprite = dot;
            sr.sortingOrder = -9;
            sr.color = new Color(1, 1, 1, Random.Range(0.25f, 0.9f));
            stars[i] = g.transform;
            speeds[i] = s * 8f + 0.3f;
        }
    }

    void Update()
    {
        float boost = (GameManager.Instance != null && !GameManager.Instance.GameOver) ? GameManager.Instance.Speed / 6f : 0.2f;
        for (int i = 0; i < count; i++)
        {
            var p = stars[i].position;
            p.x -= speeds[i] * boost * Time.deltaTime;
            if (p.x < -12f) { p.x = 12f; p.y = Random.Range(-4.5f, 4.5f); }
            stars[i].position = p;
        }
    }
}
