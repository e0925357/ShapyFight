using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private float cameraSpeed = 1f;
		[SerializeField]
		private float shakeStrength = 0.7f;
		[SerializeField]
		private Color flashColor;
		[SerializeField]
		private float flashDuration = 0.3f;

		private Transform playerTransform;
		private float maxShakeTimer = 0;
		private float shakeTimer = 0;
		private float shakeMod = 1;
		private Vector3 lastCameraShakeOffset = Vector3.zero;
		private float lastShakeAngle;
		private Color originalColor;
		private Coroutine flashCoroutine;

		void Start()
		{
			GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
			playerTransform = playerGo.transform;
			originalColor = Camera.main.backgroundColor;
		}

		public void ScreenFlash()
		{
			if (flashCoroutine != null)
			{
				StopCoroutine(flashCoroutine);
			}

			flashCoroutine = StartCoroutine(StartFlashing());
		}

		IEnumerator StartFlashing()
		{
			float colorTransitionTime = 0.1f;
			float attackWaitTime = Mathf.Max(flashDuration - colorTransitionTime * 2, 0f);
			float timer = 0;
			Color c;

			while (timer < colorTransitionTime)
			{
				c = Color.Lerp(originalColor, flashColor, timer / colorTransitionTime);
				c.a = Camera.main.backgroundColor.a;

				Camera.main.backgroundColor = c;
				timer += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}
			c = flashColor;
			c.a = Camera.main.backgroundColor.a;
			Camera.main.backgroundColor = c;

			yield return new WaitForSeconds(attackWaitTime);
			timer = 0f;

			while (timer < colorTransitionTime)
			{
				c = Color.Lerp(flashColor, originalColor, timer / colorTransitionTime);
				c.a = Camera.main.backgroundColor.a;

				Camera.main.backgroundColor = c;
				timer += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}
			c = originalColor;
			c.a = Camera.main.backgroundColor.a;
			Camera.main.backgroundColor = c;

			flashCoroutine = null;
		}

		void Update()
		{
            if (GameController.instance.IsPaused)
                return;

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

            Vector3 newPos = Camera.main.transform.position + new Vector3(scaledDelta + mod, 0, 0) - lastCameraShakeOffset;

			if (shakeTimer > 0)
			{
				shakeTimer -= Time.deltaTime;

				if (shakeTimer < 0) shakeTimer = 0;

				float easing = (shakeTimer * shakeTimer) / (maxShakeTimer * maxShakeTimer);

				if (lastShakeAngle > 0)
				{
					lastShakeAngle -= Mathf.PI;
				}
				else
				{
					lastShakeAngle += Mathf.PI;
				}

				lastShakeAngle += Random.value * Mathf.PI / 2f - Mathf.PI / 4f;

				lastCameraShakeOffset = new Vector2(Mathf.Cos(lastShakeAngle), Mathf.Sin(lastShakeAngle)) * shakeStrength * shakeMod * easing;
				newPos += lastCameraShakeOffset;
			}
			else
			{
				lastCameraShakeOffset = Vector3.zero;
			}

			Camera.main.transform.position = newPos;
		}

		public void ShakeCamera(float duration, float strengthMod = 1f)
		{
			maxShakeTimer = shakeTimer = duration;
			shakeMod = strengthMod;
		}
	}
}