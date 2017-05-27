using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public delegate void StateChanged(PlayerState oldState, PlayerState newState);

	public event StateChanged playerStateChangeEvent;

	[SerializeField]
	private float baseSpeed = 10;
	[SerializeField]
	private float jumpVelocity = 30;
	[SerializeField]
	private LayerMask groundLayer;
	[SerializeField]
	private float groundRaycastLength = 0.03f;
    [SerializeField]
    private float timeForTurningIntoGhost = 3;

    private bool isBodyTaken = false;
    private bool isWaitingForGhostForm = false;
	private bool onGound = false;

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
                    this.gameObject.GetComponent<Collider2D>().enabled = false;
                    isWaitingForGhostForm = true;
                    break;
            }
			if (oldValue != playerState && playerStateChangeEvent != null)
			{
				playerStateChangeEvent(oldValue, playerState);
			}
		}
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

    private void Start()
    {
        lastPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaitingForGhostForm)
        {
            if (isBodyTaken)
                return;
            // when the killed player is behind the enemy and is more than 2 units away from him then spawn the body and change into the ghost
            if (this.transform.position.x > PlayerCollisionsHandler.instance.enemyWithBody.gameObject.transform.position.x &&
                (this.transform.position - PlayerCollisionsHandler.instance.enemyWithBody.gameObject.transform.position).magnitude >= 3)
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
            }

            physicsBody.velocity = velocity;

            if (playerState == PlayerState.Alive)
                totalDistancePassed += (lastPos - this.transform.position).magnitude;
            else if (playerState == PlayerState.Ghost)
                totalDistancePassed -= (lastPos - this.transform.position).magnitude;

            lastPos = this.transform.position;

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
	}

    private void TurnPlayerIntoGhost()
    {
        this.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.gameObject.GetComponent<Collider2D>().enabled = true;
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
	Alive, Ghost, Dead
}
