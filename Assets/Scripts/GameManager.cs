using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Puntaje, velocidad creciente, generador de obstáculos y pantalla de Game Over.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Sprite square, circle, triangle;
    public float Speed = 6f;
    public bool GameOver;

    static readonly Color[] palette =
    {
        new Color(1f, 0.30f, 0.55f),
        new Color(1f, 0.55f, 0.20f),
        new Color(0.65f, 0.40f, 1f),
        new Color(0.30f, 1f, 0.60f),
    };

    float score, spawnTimer, best;
    const float FloorY = -3.5f, CeilY = 3.5f;
    GUIStyle big, center;

    void Awake()
    {
        Instance = this;
        best = PlayerPrefs.GetFloat("best", 0);
    }

    void Update()
    {
        if (GameOver)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        score += Time.deltaTime * 10f;
        Speed = 6f + score * 0.03f;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            Spawn();
            spawnTimer = Random.Range(1.0f, 1.8f) * (6f / Speed) + 0.4f;
        }
    }

    void Spawn()
    {
        bool onFloor = Random.value > 0.5f;
        float h = Random.Range(1.2f, 2.6f);
        float y = onFloor ? FloorY + h / 2f : CeilY - h / 2f;
        Color col = palette[Random.Range(0, palette.Length)];

        var go = new GameObject("Obstacle");
        go.transform.position = new Vector3(12f, y, 0);
        go.transform.localScale = new Vector3(0.8f, h, 1);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = square;
        sr.color = col * 0.85f;
        sr.sortingOrder = 1;
        go.AddComponent<BoxCollider2D>();
        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        go.AddComponent<Obstacle>();

        // Punta triangular brillante en el extremo libre
        var tip = new GameObject("Tip");
        tip.transform.SetParent(go.transform, false);
        float tipH = 0.5f / h; // alto local para que mida 0.5 unidades
        float sign = onFloor ? 1f : -1f;
        tip.transform.localScale = new Vector3(1.3f, tipH, 1);
        tip.transform.localPosition = new Vector3(0, sign * (0.5f + 0.5f * tipH), 0);
        var tsr = tip.AddComponent<SpriteRenderer>();
        tsr.sprite = triangle;
        tsr.color = col;
        tsr.sortingOrder = 2;
        tsr.flipY = !onFloor;
    }

    public void Die(Vector3 pos)
    {
        if (GameOver) return;
        GameOver = true;
        if (score > best) { best = score; PlayerPrefs.SetFloat("best", best); }

        var player = GameObject.Find("Player");
        if (player != null) player.SetActive(false);

        for (int i = 0; i < 24; i++)
        {
            var g = new GameObject("Debris");
            g.transform.position = pos;
            g.transform.localScale = Vector3.one * Random.Range(0.1f, 0.3f);
            var sr = g.AddComponent<SpriteRenderer>();
            sr.sprite = circle;
            sr.sortingOrder = 5;
            sr.color = Color.Lerp(new Color(1f, 0.9f, 0.2f), Color.white, Random.value);
            var d = g.AddComponent<Debris>();
            d.vel = Random.insideUnitCircle.normalized * Random.Range(2f, 7f);
        }
    }

    void Label(Rect r, string text, GUIStyle s, Color c)
    {
        s.normal.textColor = new Color(0, 0, 0, 0.6f);
        GUI.Label(new Rect(r.x + 2, r.y + 2, r.width, r.height), text, s);
        s.normal.textColor = c;
        GUI.Label(r, text, s);
    }

    void OnGUI()
    {
        if (big == null)
        {
            big = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold };
            center = new GUIStyle(big) { alignment = TextAnchor.MiddleCenter };
        }

        Label(new Rect(24, 18, 400, 40), "PUNTOS  " + (int)score, big, Color.white);
        Label(new Rect(24, 54, 400, 40), "MEJOR  " + (int)best, big, new Color(1f, 0.9f, 0.3f));

        if (GameOver)
        {
            GUI.color = new Color(0, 0, 0, 0.45f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            center.fontSize = 64;
            Label(new Rect(0, Screen.height / 2f - 90, Screen.width, 80), "GAME OVER", center, new Color(1f, 0.35f, 0.55f));
            center.fontSize = 30;
            Label(new Rect(0, Screen.height / 2f, Screen.width, 40), "Puntos: " + (int)score, center, Color.white);
            center.fontSize = 22;
            Label(new Rect(0, Screen.height / 2f + 50, Screen.width, 40), "Presiona R para reiniciar", center, new Color(0.7f, 0.9f, 1f));
        }
        else if (score < 3f)
        {
            center.fontSize = 22;
            Label(new Rect(0, Screen.height - 70, Screen.width, 40), "ESPACIO / CLIC = invertir gravedad", center, new Color(0.7f, 0.9f, 1f));
        }
    }
}
