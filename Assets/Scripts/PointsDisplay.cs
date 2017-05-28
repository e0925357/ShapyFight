using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PointsDisplay : MonoBehaviour
{
    public static PointsDisplay instance;

	[SerializeField]
	private RectTransform scoreTextRect;
	[SerializeField]
	private AnimationCurve scaleEffect;

	private Coroutine lastCoroutine = null;

    private void Awake()
    {
        instance = this;
    }

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
		Vector3 scale = scoreTextRect.localScale;
		float timer = 0;

		do
		{
			scale.x = scaleEffect.Evaluate(timer);
			scale.y = scaleEffect.Evaluate(timer);

			scoreTextRect.localScale = scale;

			timer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		} while (timer < maxTime);

		scale = Vector3.one;
		scoreTextRect.localScale = scale;
		lastCoroutine = null;
	}
}

