using UnityEngine;

namespace Assets.Scripts
{
	public class CameraController : MonoBehaviour
	{
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

			Camera.main.transform.position = Camera.main.transform.position + new Vector3(delta + mod, 0, 0);
		}
	}
}