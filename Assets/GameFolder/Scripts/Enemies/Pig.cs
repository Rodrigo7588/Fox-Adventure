using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    public Transform a, b;
    private bool goRight;
    [Header("Velocidade Movimento")]
    public float speedMove = 7f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        followPoints();
    }

    private void followPoints(){
        if (goRight){
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (Vector2.Distance(transform.position, b.position) < 0.1f){
                goRight = false; // inverte pra esquerda
            }
            transform.position = Vector2.MoveTowards(transform.position, b.position, speedMove * Time.deltaTime);
        }else{
            transform.localScale = new Vector3(-1f, 1f, 1f);
            if (Vector2.Distance(transform.position, a.position) < 0.1f){
                goRight = true; // inverte pra direita
            }
            transform.position = Vector2.MoveTowards(transform.position, a.position, speedMove * Time.deltaTime);
        }
    }

    void Death()
    {
        Destroy(transform.parent.gameObject);
    }
}
