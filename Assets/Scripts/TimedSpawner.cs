using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawner : MonoBehaviour
{
    [SerializeField]
    private bool isEnemySpawner = false;
	[SerializeField]
	private GameObject[] prefabsToSpawn;
	[SerializeField]
	private Vector2 spawnTimeFrame = new Vector2(1.5f, 2f);

	private Coroutine spawnCoroutine = null;

	void OnEnable()
	{
		if (spawnCoroutine != null)
		{
			StopCoroutine(spawnCoroutine);
		}

		spawnCoroutine = StartCoroutine(spawn());
	}

	IEnumerator spawn()
	{
        if (isEnemySpawner)
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(spawnTimeFrame.x, spawnTimeFrame.y) + ((GameController.instance != null) ? (GameController.instance.BoostValue - 1) / 4 : 0));
                Instantiate(prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)], transform.position, Quaternion.identity);
            }
        }
        else
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(spawnTimeFrame.x, spawnTimeFrame.y));
                Instantiate(prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)], transform.position, Quaternion.identity);
            }
        }
	}

	void OnDisable()
	{
		if (spawnCoroutine != null)
		{
			StopCoroutine(spawnCoroutine);
			spawnCoroutine = null;
		}
	}
}
