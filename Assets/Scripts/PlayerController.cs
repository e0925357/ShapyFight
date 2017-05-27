using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public delegate void StateChanged(PlayerState oldState, PlayerState newState);

	public event StateChanged playerStateChangeEvent;

	private static int _animJump = Animator.StringToHash("Jump");
	private static int _animDuck = Animator.StringToHash("Duck");
	private static int _animPunch = Animator.StringToHash("Punch");
	private static int _animOnGround = Animator.StringToHash("OnGround");
	private static int _animGoesRight = Animator.StringToHash("GoesRight");

	[SerializeField]
	private float baseSpeed = 10;
	[SerializeField]
	private float jumpVelocity = 30;
	[SerializeField]
	private LayerMask groundLayer;
	[SerializeField]
	private LayerMask enemyLayer;
	[SerializeField]
	private float groundRaycastLength = 0.03f;
	[SerializeField]
	private Animator playerAnimator;
	[SerializeField]
	private float attackTime = 0.5f;
	[SerializeField]
	private Color attackColor = Color.red;
    [SerializeField]
    private float timeForTurningIntoGhost = 3;
    [SerializeField]
    private float raycastLenght = .5f;

    private bool isBodyTaken = false;
    private bool isWaitingForGhostForm = false;
	private bool onGound = false;
	private Coroutine attackTimerCoroutine = null;
	private Color defaultColor;
	private SpriteRenderer bodyRenderer;

    private float totalDistancePassed = 0;
    private Vector3 lastPos = new Vector3();

	[ContextMenuItem("Toggle Player State", "ToggleState")]
	[SerializeField]
	[Tooltip("Do not change!")]
	private PlayerState playerState;

	public PlayerState PlayerState
	{
		get { return playerState; }
		set
		{
			PlayerState oldValue = playerState;
			playerState = value;

            switch (value)
            {
                case PlayerState.Ghost:

                    this.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    this.gameObject.GetComponentInChildren<Collider2D>().enabled = false;
                    isWaitingForGhostForm = true;
                    break;
            }
			if (oldValue != playerState && playerStateChangeEvent != null)
			{
				playerStateChangeEvent(oldValue, playerState);
			}

			playerAnimator.SetBool(_animGoesRight, PlayerState == PlayerState.Alive);
		}
	}

	public bool Attacking { get; set; }
    private AttackType attackType;

	void Start()
	{
		playerAnimator.SetBool(_animGoesRight, PlayerState == PlayerState.Alive);
		bodyRenderer = GetComponentInChildren<SpriteRenderer>();
		defaultColor = bodyRenderer.color;
		lastPos = this.transform.position;
	}

	void ToggleState()
	{
		if (PlayerState == PlayerState.Alive)
		{
			PlayerState = PlayerState.Ghost;
		}
		else
		{
			PlayerState = PlayerState.Alive;
		}
	}

	void attack()
	{
		if (attackTimerCoroutine != null)
		{
			StopCoroutine(attackTimerCoroutine);
		}

		attackTimerCoroutine = StartCoroutine(StartAttackTimer());
	}

	IEnumerator StartAttackTimer()
	{
		float colorTransitionTime = 0.1f;
		float attackWaitTime = attackTime - colorTransitionTime*2;
		float timer = 0;
		Color c;
		Attacking = true;

		while (timer < colorTransitionTime)
		{
			c = Color.Lerp(defaultColor, attackColor, timer / colorTransitionTime);
			c.a = bodyRenderer.color.a;

			bodyRenderer.color = c;
			timer += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
		c = attackColor;
		c.a = bodyRenderer.color.a;
		bodyRenderer.color = c;

		yield return new WaitForSeconds(attackWaitTime);
		timer = 0f;

		while (timer < colorTransitionTime)
		{
			c = Color.Lerp(attackColor, defaultColor, timer / colorTransitionTime);
			c.a = bodyRenderer.color.a;

			bodyRenderer.color = c;
			timer += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
		c = defaultColor;
		c.a = bodyRenderer.color.a;
		bodyRenderer.color = c;

		Attacking = false;
		attackTimerCoroutine = null;
	}

	// Update is called once per frame
	void Update ()
	{
		if (isWaitingForGhostForm)
		{
			if (isBodyTaken)
				return;
			// when the killed player is behind the enemy and is more than 2 units away from him then spawn the body and change into the ghost
			if (this.transform.position.x > PlayerCollisionsHandler.instance.enemyWithBody.gameObject.transform.position.x &&
			    (this.transform.position - PlayerCollisionsHandler.instance.enemyWithBody.gameObject.transform.position)
			    .magnitude >= 3)
			{
				PlayerCollisionsHandler.instance.enemyWithBody.TakeAndSpawnPlayerBody(this.transform.position);
				Color32 ghostCol = this.gameObject.GetComponentInChildren<SpriteRenderer>().color;
				ghostCol.a = 100;
				this.gameObject.GetComponentInChildren<SpriteRenderer>().color = ghostCol;
				isBodyTaken = true;
				Invoke("TurnPlayerIntoGhost", timeForTurningIntoGhost);
			}
		}
		else
		{
			Rigidbody2D physicsBody = GetComponent<Rigidbody2D>();
			Vector2 velocity = physicsBody.velocity;

			if (Input.GetButtonDown("Jump") && onGound)
			{
				velocity.y = jumpVelocity;
				playerAnimator.SetTrigger(_animJump);
			}

			if (Input.GetButtonDown("Duck"))
			{
				playerAnimator.SetTrigger(_animDuck);

				if (!onGound)
				{
                    attackType = AttackType.DOWNWARDS;
					attack();
				}
			}

			if (PlayerState == PlayerState.Alive && Input.GetButtonDown("PunchRight") ||
			    PlayerState == PlayerState.Ghost && Input.GetButtonDown("PunchLeft"))
			{
				playerAnimator.SetTrigger(_animPunch);

                attackType = AttackType.FORWARD;
				attack();
			}

			physicsBody.velocity = velocity;

			if (playerState == PlayerState.Alive)
			{
				totalDistancePassed += (lastPos - transform.position).magnitude;
			}
			else if (playerState == PlayerState.Ghost)
			{
				totalDistancePassed -= (lastPos - transform.position).magnitude;
			}

			lastPos = transform.position;
			GameController.instance.Score = Mathf.RoundToInt(totalDistancePassed);
		}
	}

	void FixedUpdate()
	{
        if (isWaitingForGhostForm)
            return;

		Rigidbody2D physicsBody = GetComponent<Rigidbody2D>();
		Vector2 velocity = physicsBody.velocity;
		int direction = PlayerState == PlayerState.Alive ? 1 : -1;

		velocity.x = baseSpeed*direction;

		physicsBody.velocity = velocity;

		onGound = Physics2D.Raycast(new Vector2(0f, -0.5f) + (Vector2) transform.position,
			Vector2.down, groundRaycastLength, groundLayer);

		playerAnimator.SetBool(_animOnGround, onGound);

		//Check for enemyHits here
		if (Attacking)
		{
            Collider2D col = this.gameObject.GetComponentInChildren<Collider2D>();

            if (attackType == AttackType.FORWARD)
            {
                RaycastHit2D hit = Physics2D.BoxCast(
                    this.transform.position + new Vector3(col.bounds.extents.x + (raycastLenght / 2), 0),
                    new Vector2(raycastLenght, col.bounds.extents.y * 2),
                    0,
                    Vector2.right * (int)playerState,
                    raycastLenght,
                    enemyLayer
                    );

                if (hit)
                {
                    Destroy(hit.transform.gameObject);
                }
            }
            else if (attackType == AttackType.DOWNWARDS)
            {
                RaycastHit2D hit = Physics2D.BoxCast(
                    this.transform.position + new Vector3(0, col.bounds.extents.y + raycastLenght / 2),
                    new Vector2(col.bounds.extents.y * 2, raycastLenght),
                    0,
                    Vector2.down,
                    raycastLenght,
                    enemyLayer
                    );

                if (hit)
                {
                    Destroy(hit.transform.gameObject);
                }
            }
            else
                throw new System.Exception("No attack type assigned!");
		}
	}

    private void TurnPlayerIntoGhost()
    {
        this.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.gameObject.GetComponentInChildren<Collider2D>().enabled = true;
        isWaitingForGhostForm = false;
    }

    public void TakeTheBodyBack()
    {
        Color32 ghostCol = this.gameObject.GetComponentInChildren<SpriteRenderer>().color;
        ghostCol.a = 255;
        this.gameObject.GetComponentInChildren<SpriteRenderer>().color = ghostCol;

        PlayerState = PlayerState.Alive;
        Destroy(PlayerCollisionsHandler.instance.enemyWithBody.PlayerBody);
        PlayerCollisionsHandler.instance.enemyWithBody.gameObject.GetComponent<SpringJoint2D>().enabled = false;
        PlayerCollisionsHandler.instance.enemyWithBody = null;
    }
}

public enum PlayerState
{
	Alive = 1, Ghost = -1, Dead
}
