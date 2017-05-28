using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class PointsDisplay : MonoBehaviour
	{
		[SerializeField]
		private Text pointsDisplay;
		[SerializeField]
		private AnimationCurve scaleEffect;
		
		private Coroutine lastCoroutine = null;

		public void DoScaleEffect() {
			lastCoroutine = StartCoroutine(doScaleEffect());
		}

		IEnumerator doScaleEffect()
		{
			if (lastCoroutine != null)
			{
				StopCoroutine(lastCoroutine);
			}

			float maxTime = scaleEffect.keys[scaleEffect.length - 1].time;
			Vector3 scale = transform.localScale;
			float timer = 0;

			do
			{
				scale.x = scaleEffect.Evaluate(timer);
				scale.y = scaleEffect.Evaluate(timer);

				transform.localScale = scale;

				timer += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			} while (timer < maxTime);

			scale = Vector3.one;
			transform.localScale = scale;
			lastCoroutine = null;
		}
	}
}
