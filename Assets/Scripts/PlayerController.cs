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

	private bool onGound = false;
	private Coroutine attackTimerCoroutine = null;
	private Color defaultColor;
	private SpriteRenderer bodyRenderer;

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

			if (oldValue != playerState && playerStateChangeEvent != null)
			{
				playerStateChangeEvent(oldValue, playerState);
			}

			playerAnimator.SetBool(_animGoesRight, PlayerState == PlayerState.Alive);
		}
	}

	public bool Attacking { get; set; }

	void Start()
	{
		playerAnimator.SetBool(_animGoesRight, PlayerState == PlayerState.Alive);
		bodyRenderer = GetComponentInChildren<SpriteRenderer>();
		defaultColor = bodyRenderer.color;
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
				attack();
			}
		}

		if (PlayerState == PlayerState.Alive && Input.GetButtonDown("PunchRight") ||
		    PlayerState == PlayerState.Ghost && Input.GetButtonDown("PunchLeft"))
		{
			playerAnimator.SetTrigger(_animPunch);

			attack();
		}

		physicsBody.velocity = velocity;
	}

	void FixedUpdate()
	{
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
		}
	}
}

public enum PlayerState
{
	Alive, Ghost, Dead
}
