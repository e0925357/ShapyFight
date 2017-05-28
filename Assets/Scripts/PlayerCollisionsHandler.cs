using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
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
        else if (collision.collider.gameObject.tag.Equals("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyControl>().DefendYourself();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Death"))
        {
            if (!collision.gameObject.GetComponentInParent<EnemyControl>().isDead)
            {
	            PlayerController pc = this.gameObject.GetComponent<PlayerController>();
				pc.GetComponent<PlSoundManager>().playSound(SoundType.Hurt, 0.02f);

                if (GameController.instance.IsSecondChance)
                {
                    // this also changes the player's state
                    GameController.instance.IsFailed = true;
                }
                else
                {
                    this.gameObject.GetComponent<PlayerController>().PlayerState = PlayerState.Ghost;
                    enemyWithBody = collision.gameObject.GetComponentInParent<EnemyControl>();
                    enemyWithBody.hasKilledPlayer = true;

	                GameController.instance.BoostValue *= 0.4f;

                }
            }
        }
    }
}
