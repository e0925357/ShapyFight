using UnityEngine;

namespace Assets.Scripts
{
	public class DistanceDestroyer : MonoBehaviour
	{
		[SerializeField]
		private float deathDistance = 10f;

		private Transform targetTransform;

		void Start()
		{
			targetTransform = Camera.main.transform;
		}

		void FixedUpdate()
		{
			Vector2 delta = targetTransform.position - transform.position;

			if (delta.sqrMagnitude > deathDistance * deathDistance)
			{
				Destroy(gameObject);
			}
		}
	}
}