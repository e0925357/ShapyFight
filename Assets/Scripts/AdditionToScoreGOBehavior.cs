using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionToScoreGOBehavior : MonoBehaviour
{
    private float lerpSpeed;

    private void Start()
    {
        lerpSpeed = Random.Range(.1f, .3f);
    }

    private void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, UIController.instance.scoreAdditionGOTran.position, Mathf.Lerp(.05f, 1f, lerpSpeed));
    }
}
