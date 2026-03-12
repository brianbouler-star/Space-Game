using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float speed = 80f;
    [SerializeField] private float lifetime = 0.8f;

    public void SetText(string content)
    {
        if (label != null) label.text = content;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f) Destroy(gameObject);
    }
}
