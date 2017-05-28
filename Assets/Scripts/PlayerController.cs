using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Math;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public delegate void StateChanged(PlayerState oldState, PlayerState newState);
	public delegate void Dash();

	public event StateChanged playerStateChangeEvent;
	public event Dash dashEvent;

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
	[SerializeField]
	private float crushVelocity = 3f;
	[SerializeField]
	private float cameraShakeDuration = 0.3f;
	[SerializeField]
	private Vector2 distanceForBoost = new Vector2(2f, 4f);
    [Range(0, 1)]
    [SerializeField]
    private float boostValue = .2f;
	[SerializeField]
	private float attackCooldownTime = 0.6f;
	[SerializeField]
	private float dashSpeedBonus = 5f;
	[SerializeField]
	private float dashDuration = 0.4f;
	[SerializeField]
	private float upwardsTimeWindow = 0.4f;

	[HideInInspector]
    public bool CanGetBoost = false;

    private bool isBodyTaken = false;
    private bool isWaitingForGhostForm = false;
	private bool onGound = false;
    private float distanceAtAttack = Mathf.Infinity;
    private float currentTime = 0;
    private float lastAddedDistance = 0;
	private Coroutine attackTimerCoroutine = null;
	private Color defaultColor;
	private SpriteRenderer bodyRenderer;
	private Coroutine attackCooldownCoroutine;
	private float dashAttackSpeedValue = 0f;
	private bool canDoUpwardAttack = false;
	private Coroutine upwardsTimerCoroutine = null;

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
	                GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>().enabled = false;
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

		GetComponent<PlSoundManager>().playSound(SoundType.Attack, 0.05f);
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

	IEnumerator DoAttackCooldown()
	{
		yield return new WaitForSeconds(attackCooldownTime);

		attackCooldownCoroutine = null;
	}

	IEnumerator DecreaseSpeedBonus()
	{
		yield return new WaitForSeconds(dashDuration);

		const float easingTime = 0.1f;
		float timer = easingTime;

		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			dashAttackSpeedValue = Mathf.Lerp(0f, dashSpeedBonus, Mathf.Pow(timer / easingTime, 2));

			yield return new WaitForEndOfFrame();
		}

		dashAttackSpeedValue = 0f;
	}

	IEnumerator StartUpwardsAttackTimer()
	{
		canDoUpwardAttack = true;

		yield return new WaitForSeconds(upwardsTimeWindow);

		canDoUpwardAttack = false;
		upwardsTimerCoroutine = null;
	}

	void upwardsAttackTimeWindowStart()
	{
		if (upwardsTimerCoroutine != null)
		{
			StopCoroutine(upwardsTimerCoroutine);
		}

		upwardsTimerCoroutine = StartCoroutine(StartUpwardsAttackTimer());
	}

	// Update is called once per frame
	void Update ()
	{
        if (GameController.instance.IsPaused)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= 1)
        {
            currentTime = 0;
            GameController.instance.BoostValue -= .01f;
        }

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

				if (!canDoUpwardAttack)
				{
					GetComponent<PlSoundManager>().playSound(SoundType.Jump, 0.05f);
				}
				else
				{
					attackType = AttackType.UPWARDS;
					attack();
				}

				Collider2D col = this.GetComponentInChildren<Collider2D>();
                RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, new Vector2(1, col.bounds.extents.y * 2), 0, Vector2.right * (int)playerState, Mathf.Infinity, enemyLayer);

                if (hit)
                {
                    distanceAtAttack = Mathf.Abs(hit.point.x - this.transform.position.x) - col.bounds.extents.x;
                    if (distanceAtAttack <= distanceForBoost.y && distanceAtAttack > 0.05f)
                        CanGetBoost = true;

				}

				if (attackCooldownCoroutine != null)
				{
					StopCoroutine(attackCooldownCoroutine);
					attackCooldownCoroutine = null;
				}
			}

			if (Input.GetButtonDown("Duck"))
			{
				playerAnimator.SetTrigger(_animDuck);

				if (!onGound)
				{
					attackType = AttackType.DOWNWARDS;
					attack();
					velocity.y = -crushVelocity;
				}
				else
				{
					upwardsAttackTimeWindowStart();
				}

				if (attackCooldownCoroutine != null)
				{
					StopCoroutine(attackCooldownCoroutine);
					attackCooldownCoroutine = null;
				}
			}

			if ((PlayerState == PlayerState.Alive && Input.GetButtonDown("PunchRight") ||
			    PlayerState == PlayerState.Ghost && Input.GetButtonDown("PunchLeft")) &&
			    attackCooldownCoroutine == null)
			{
				playerAnimator.SetTrigger(_animPunch);

                attackType = AttackType.FORWARD;
				attack();

				attackCooldownCoroutine = StartCoroutine(DoAttackCooldown());

				if (!onGound)
				{
					dashAttackSpeedValue = dashSpeedBonus;
					StartCoroutine(DecreaseSpeedBonus());

					if (dashEvent != null)
					{
						dashEvent();
					}
				}
			}

			physicsBody.velocity = velocity;

			if (playerState == PlayerState.Alive)
			{
				totalDistancePassed += (lastPos - transform.position).magnitude;

                if (Mathf.RoundToInt(totalDistancePassed) - lastAddedDistance >= 1)
                {
                    GameController.instance.Score += 1;
                    lastAddedDistance = totalDistancePassed;
                }
            }
			else if (playerState == PlayerState.Ghost)
			{
				totalDistancePassed -= (lastPos - transform.position).magnitude;

                if (Mathf.RoundToInt(totalDistancePassed) - lastAddedDistance <= -1)
                {
                    GameController.instance.Score -= 1;
                    lastAddedDistance = totalDistancePassed;
                }
			}

			lastPos = transform.position;
        }
	}

	void FixedUpdate()
	{
        if (isWaitingForGhostForm || GameController.instance.IsPaused)
            return;

		Collider2D col = GetComponentInChildren<Collider2D>();
		Rigidbody2D physicsBody = GetComponent<Rigidbody2D>();
		Vector2 velocity = physicsBody.velocity;
		int direction = PlayerState == PlayerState.Alive ? 1 : -1;

		velocity.x = (baseSpeed + dashAttackSpeedValue) *direction * GameController.instance.BoostValue;

		physicsBody.velocity = velocity;

		onGound = Physics2D.Raycast(new Vector2(0f, -col.bounds.extents.y) + (Vector2) transform.position,
			Vector2.down, groundRaycastLength, groundLayer);

		playerAnimator.SetBool(_animOnGround, onGound);

		//Check for enemyHits here
		if (Attacking)
		{
			RaycastHit2D hit;


			if (attackType == AttackType.FORWARD)
            {
                hit = Physics2D.BoxCast(
                    this.transform.position + new Vector3((col.bounds.extents.x + (raycastLenght / 2)) * (int)playerState, 0),
                    new Vector2(raycastLenght, col.bounds.extents.y * 1.9f),
                    0,
                    Vector2.right * (int)playerState,
                    raycastLenght,
                    enemyLayer
                    );
            }
            else if (attackType == AttackType.DOWNWARDS)
            {
                hit = Physics2D.BoxCast(
                    this.transform.position + new Vector3(0, col.bounds.extents.y + raycastLenght / 2),
                    new Vector2(col.bounds.extents.x * 1.9f, raycastLenght + .4f),
                    0,
                    Vector2.down,
                    raycastLenght + .4f,
                    enemyLayer
                    );
            }
			else if (attackType == AttackType.UPWARDS)
			{
				hit = Physics2D.BoxCast(
					this.transform.position - new Vector3(0, col.bounds.extents.y + raycastLenght / 2),
					new Vector2(col.bounds.extents.x * 1.9f, raycastLenght + .4f),
					0,
					Vector2.up,
					raycastLenght + .4f,
					enemyLayer
				);
			}
			else
                throw new System.Exception("No attack type assigned!");

			if (hit)
			{
                if (hit.rigidbody.GetComponent<EnemyControl>().hasKilledPlayer)
                    TakeTheBodyBack();

				hit.rigidbody.GetComponent<EnemyControl>().Death(hit.point);
				Camera.main.GetComponent<CameraController>().ShakeCamera(cameraShakeDuration);
				Camera.main.GetComponent<CameraController>().ScreenFlash();
				if (CanGetBoost)
				{
					float boostModMod = 1f - Mathf.Clamp01((distanceAtAttack - distanceForBoost.x) /
					                                  (distanceForBoost.y - distanceForBoost.x));
					float boostMod = MathUtil.quadraticEasingInOut(boostModMod);

					//Debug.Log(string.Format("distanceAtAttack: {0}, boostMod: {1}", distanceAtAttack, boostMod));

					GameController.instance.BoostValue += boostValue*boostMod;
					CanGetBoost = false;
					distanceAtAttack = Mathf.Infinity;
				}
			}
		}
	}

    private void TurnPlayerIntoGhost()
    {
        this.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.gameObject.GetComponentInChildren<Collider2D>().enabled = true;
        isWaitingForGhostForm = false;
	    GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>().enabled = true;
		GameController.instance.IsSecondChance = true;
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
