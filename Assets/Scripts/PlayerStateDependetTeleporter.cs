using UnityEngine;

public class PlayerStateDependetTeleporter : MonoBehaviour
{
	[SerializeField]
	private Transform alivePosition;
	[SerializeField]
	private Transform otherPosition;

	[Space]
	[SerializeField]
	private string aliveTag;
	[SerializeField]
	private string otherTag;

	private PlayerController playerController;

	void Start()
	{
		if (playerController != null)
		{
			OnPlayerStateChange(playerController.PlayerState, playerController.PlayerState);
		}
	}

	void OnEnable()
	{
		if (alivePosition == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag(aliveTag);

			if (go != null)
			{
				alivePosition = go.transform;
			}
		}

		if (otherPosition == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag(otherTag);

			if (go != null)
			{
				otherPosition = go.transform;
			}
		}

		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		playerController.playerStateChangeEvent += OnPlayerStateChange;
	}

	private void OnPlayerStateChange(PlayerState oldState, PlayerState newState)
	{
		if (newState == PlayerState.Alive)
		{
			transform.position = alivePosition.position;
		}
		else
		{
			transform.position = otherPosition.position;
		}
	}

	void OnDisable()
	{
		if (playerController != null)
		{
			playerController.playerStateChangeEvent -= OnPlayerStateChange;
		}
	}
}
