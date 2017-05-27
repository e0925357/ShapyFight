using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsHandler : MonoBehaviour
{
    public static PlayerCollisionsHandler instance;
    [HideInInspector]
    public EnemyControl enemyWithBody;


    private void Awake()
    {
        instance = this;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag.Equals("Body"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().TakeTheBodyBack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Death"))
        {
            if (GameController.instance.IsSecondChance)
            {
                // this also changes the player's state
                GameController.instance.IsFailed = true;
            }
            else
            {
                this.gameObject.GetComponent<PlayerController>().PlayerState = PlayerState.Ghost;
                enemyWithBody = collision.gameObject.GetComponentInParent<EnemyControl>();
            }
        }
    }
}
