using UnityEngine;

namespace Assets.Scripts
{
	public class CameraWidthScaler : MonoBehaviour
	{
		[SerializeField]
		private Vector2 constantToAdd = Vector2.zero;
		[SerializeField]
		private Vector2 cameraDependetScale = Vector2.one;
		[SerializeField]
		private bool scaleX = false;
		[SerializeField]
		private bool scaleY = false;

		void OnEnable()
		{
			FixedUpdate();
		}

		void FixedUpdate()
		{
			Vector2 cameraSize = new Vector2(Camera.main.orthographicSize*Camera.main.aspect, Camera.main.orthographicSize);

			cameraSize.Scale(cameraDependetScale);
			cameraSize += constantToAdd;

			Vector3 scale = transform.localScale;

			if (scaleX)
			{
				scale.x = cameraSize.x;
			}
			if (scaleY)
			{
				scale.y = cameraSize.y;
			}

			transform.localScale = scale;
		}
	}
}