using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionToScoreGOBehavior : MonoBehaviour
{
	[SerializeField]
	private Vector2 accelerationRange = new Vector2(.2f, .4f);
	[SerializeField]
	private Vector2 speedRange = new Vector2(.1f, 1.5f);

    private float lerpSpeed;

    private void Start()
    {
        lerpSpeed = Random.Range(accelerationRange.x, accelerationRange.y);
    }

    private void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, UIController.instance.scoreAdditionGOTran.position, Mathf.Lerp(speedRange.x, speedRange.y, lerpSpeed));
    }
}
