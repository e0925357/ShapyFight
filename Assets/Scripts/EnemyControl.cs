using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private Direction enemyDirection;
    [SerializeField]
    private float extraForce;
    [SerializeField]
    private Rigidbody2D playerRigid;
    [SerializeField]
    private float jumpForwardForce;
    [SerializeField]
    private float jumpUpForce;
    [SerializeField]
    private Collider2D enemyCollider;
    [SerializeField]
    private LayerMask groundMask;

    private bool isDead = false;
    private bool isPlayerConnected = false;
    private Collider2D[] enemyColliders;

    private Rigidbody2D rigid;

    private bool jumped = false;

    private void Awake()
    {
        rigid = this.gameObject.GetComponent<Rigidbody2D>();
        enemyColliders = this.gameObject.GetComponentsInChildren<Collider2D>();
    }

    private void Update()
    {
        if (isDead)
            return;

        if (jumped)
        {
            if (isGrounded())
                jumped = false;

        } else if (isGrounded() && !jumped)
        {
            Move();
        }
    }

    private void Move()
    {
        rigid.velocity = new Vector2(0, 0);

        if (!isPlayerConnected)
            rigid.AddForce(new Vector2(jumpForwardForce * (int)enemyDirection, jumpUpForce), ForceMode2D.Impulse);
        else
            rigid.AddForce(new Vector2((jumpForwardForce + extraForce) * (int)enemyDirection, jumpUpForce + extraForce), ForceMode2D.Impulse);

        jumped = true;

    }

    private bool isGrounded()
    {
        if (Physics2D.Raycast(this.transform.position + new Vector3(0, -enemyCollider.bounds.extents.y), Vector2.down, .05f, groundMask))
            return true;
        return false;
    }

    public void Death()
    {
        isDead = true;

        this.gameObject.GetComponent<SpringJoint2D>().enabled = false;

        foreach (Collider2D c in enemyColliders)
            c.enabled = false;

        rigid.freezeRotation = false;
        rigid.AddTorque(-45);
    }

    public void TakePlayer()
    {
        isPlayerConnected = true;
        SpringJoint2D joint = this.gameObject.GetComponent<SpringJoint2D>();
        joint.enabled = true;
        joint.connectedBody = playerRigid;
    }
}
