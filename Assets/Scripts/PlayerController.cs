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

	private bool onGound = false;
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
		}
	}

	// Update is called once per frame
	void Update ()
	{
		Rigidbody2D physicsBody = GetComponent<Rigidbody2D>();
		Vector2 velocity = physicsBody.velocity;

		if (Input.GetButtonDown("Jump") && onGound)
		{
			velocity.y = jumpVelocity;
		}

		physicsBody.velocity = velocity;
	}

	void FixedUpdate()
	{
		Rigidbody2D physicsBody = GetComponent<Rigidbody2D>();
		Vector2 velocity = physicsBody.velocity;

		velocity.x = baseSpeed;

		physicsBody.velocity = velocity;

		onGound = Physics2D.Raycast(new Vector2(0f, -0.5f) + (Vector2) transform.position,
			Vector2.down, groundRaycastLength);
	}
}

public enum PlayerState
{
	Alive, Ghost, Dead
}
