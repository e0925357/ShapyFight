using UnityEngine;

namespace Assets.Scripts
{
	public class CameraSizePositioner : MonoBehaviour
	{
		[SerializeField]
		private Vector2 normalizedScreenPosition;
		[SerializeField]
		private Vector3 worldOffset;

		void OnEnable()
		{
			FixedUpdate();
		}

		void FixedUpdate()
		{
			Vector2 screenOffset = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
			screenOffset.Scale(normalizedScreenPosition);

			transform.position = Camera.main.transform.position + (Vector3)screenOffset + worldOffset;
		}
	}
}