using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private GameObject playerBodyPrefab;
    [SerializeField]
    private float extraForce;
    [SerializeField]
    private float jumpForwardForce;
    [SerializeField]
    private float jumpUpForce;
    [SerializeField]
    private Collider2D enemyCollider;
    [SerializeField]
    private LayerMask groundMask;

    [HideInInspector]
    public GameObject PlayerBody;
    [HideInInspector]
    public bool isDead = false;
    private bool isPlayerConnected = false;
    [HideInInspector]
    public bool hasKilledPlayer = false;
    private Collider2D[] enemyColliders;
    private PlayerController player;
    private Rigidbody2D rigid;

    private bool jumped = false;

    private void Awake()
    {
        rigid = this.gameObject.GetComponent<Rigidbody2D>();
        enemyColliders = this.gameObject.GetComponentsInChildren<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        this.transform.localScale = new Vector3((int)player.PlayerState, 1, 1);
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

        if (!hasKilledPlayer)
        {
            if (!isPlayerConnected)
                rigid.AddForce(new Vector2(jumpForwardForce * (int)player.PlayerState, jumpUpForce), ForceMode2D.Impulse);
            else
                rigid.AddForce(new Vector2((jumpForwardForce + extraForce) * (int)player.PlayerState, jumpUpForce + extraForce * -1), ForceMode2D.Impulse);
        }
        else
        {
            if (!isPlayerConnected)
                rigid.AddForce(new Vector2(jumpForwardForce * (int)player.PlayerState * -1, jumpUpForce), ForceMode2D.Impulse);
            else
                rigid.AddForce(new Vector2((jumpForwardForce + extraForce) * (int)player.PlayerState * -1, jumpUpForce + extraForce * -1), ForceMode2D.Impulse);
        }

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

    public void TakeAndSpawnPlayerBody(Vector3 position)
    {
        PlayerBody = Instantiate(playerBodyPrefab, position, Quaternion.identity) as GameObject;

        isPlayerConnected = true;
        SpringJoint2D joint = this.gameObject.GetComponent<SpringJoint2D>();
        joint.enabled = true;
        joint.connectedBody = PlayerBody.GetComponent<Rigidbody2D>();
    }

    public void DefendYourself()
    {
        StartCoroutine(Defend());
    }

    private IEnumerator Defend()
    {
        yield return new WaitForSeconds(.5f);
        if(this.gameObject.GetComponent<Animator>() != null)
            this.gameObject.GetComponent<Animator>().SetTrigger("Attack");
    }
}
