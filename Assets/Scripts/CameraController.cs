using UnityEngine;

namespace Assets.Scripts
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private float cameraSpeed = 1f;

		private Transform playerTransform;

		void Start()
		{
			GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
			playerTransform = playerGo.transform;
		}

		void Update()
		{
			int mod = playerTransform.GetComponent<PlayerController>().PlayerState == PlayerState.Alive ? -1 : 1;
			float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
			float delta = playerTransform.position.x - (Camera.main.transform.position.x + cameraWidth*mod);
			float scaledDelta = Mathf.Sign(delta) * cameraSpeed * Time.deltaTime;
			//Prevent overshooting
			if (delta > 0 && scaledDelta > delta || delta < 0 && scaledDelta < delta)
			{
				scaledDelta = delta;
			}
            //Debug.Log(scaledDelta);
            //Debug.Log(delta);

            Camera.main.transform.position = Camera.main.transform.position + new Vector3(scaledDelta + mod, 0, 0);
		}
	}
}