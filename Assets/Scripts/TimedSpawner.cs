using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject[] prefabsToSpawn;
	[SerializeField]
	private Vector2 spawnTimeFrame = new Vector2(1f, 4f);

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
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(spawnTimeFrame.x, spawnTimeFrame.y));
			Instantiate(prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)], transform.position, Quaternion.identity);
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
