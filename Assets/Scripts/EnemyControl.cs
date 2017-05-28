using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyControl : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject playerBodyPrefab;
    [SerializeField]
    private GameObject additionToScoreGOPrefab;

    [Space()]
    [SerializeField]
    private GameObject groundSpike;
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
	[SerializeField]
	private GameObject deathParticleGO;
	[SerializeField]
	private float dashForce;

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
	private GameObject playerGo;

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

    void OnEnable()
    {
        playerGo = GameObject.FindGameObjectWithTag("Player");
        playerGo.GetComponent<PlayerController>().dashEvent += OnPlayerDash;

        if (GameController.instance == null ||
            this.gameObject.GetComponent<Animator>() == null ||
            (from p in this.gameObject.GetComponent<Animator>().parameters where p.name.Equals("SpeedMulti") select p).ToArray().Length == 0)
            return;

        if (GameController.instance.Score >= 300)
            this.gameObject.GetComponent<Animator>().SetFloat("SpeedMulti", UnityEngine.Random.Range(.2f, .4f));
        else
            this.gameObject.GetComponent<Animator>().SetFloat("SpeedMulti", 0);
    }

    private void Update()
    {
        if (isDead || GameController.instance.IsPaused)
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
        if (groundSpike == null)
        {
            if (Physics2D.Raycast(this.transform.position + new Vector3(0, -enemyCollider.bounds.extents.y), Vector2.down, .05f, groundMask))
                return true;
        }
        else
        {
            if (Physics2D.Raycast(groundSpike.transform.position + new Vector3(0, -groundSpike.GetComponent<Collider2D>().bounds.extents.y), Vector2.down, .05f, groundMask))
                return true;
        }
        return false;
    }

    public void Death(Vector2 hitpoint)
    {
        isDead = true;

        this.gameObject.GetComponent<SpringJoint2D>().enabled = false;

        foreach (Collider2D c in enemyColliders)
            c.enabled = false;

        rigid.freezeRotation = false;
        rigid.AddTorque(-45);

		Destroy(gameObject, 2f);

	    if (deathParticleGO != null)
	    {
		    deathParticleGO.transform.position = hitpoint - new Vector2(0f, 0.8f);
		    deathParticleGO.GetComponent<ParticleSystem>().Play();
			deathParticleGO.GetComponent<AudioSource>().Play();
	    }

        int i;
        for (i = 0; i < 4; i++)
        {
            Instantiate(additionToScoreGOPrefab, this.transform.position, Quaternion.identity);
        }
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

	void OnDisable()
	{
		if (playerGo)
		{
			playerGo.GetComponent<PlayerController>().dashEvent -= OnPlayerDash;
		}
	}

	private void OnPlayerDash()
	{
		if (isPlayerConnected)
		{
			rigid.AddForce(Vector2.left*dashForce, ForceMode2D.Impulse);
		}
	}
}
