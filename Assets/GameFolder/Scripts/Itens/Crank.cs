using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite spr_crank_down;
    public Transform gate;
    private bool isActive;
    public Transform wayPoint;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    IEnumerator MoveGate()
    {
        if (Vector2.Distance(gate.position, wayPoint.position) > 0.1f)
        {
            gate.position = Vector2.MoveTowards(gate.position, wayPoint.position, 10f * Time.deltaTime);
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(MoveGate());
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6 && !isActive)
        {
            isActive = true;
            spriteRenderer.sprite = spr_crank_down;
            StartCoroutine(MoveGate());
        }
    }
}
