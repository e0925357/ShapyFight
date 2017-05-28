using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
	[Serializable]
	public struct EnemySpawn
	{
		public GameObject prefab;
		public int minScoreForSpawning;
	}

	public class EnemySpawner : MonoBehaviour
	{
		[SerializeField]
		private EnemySpawn[] enemies;
		[SerializeField]
		private Vector2 spawnTimeFrame = new Vector2(1.5f, 2f);

		private Coroutine spawnCoroutine = null;

		void Start()
		{
			enemies = enemies.OrderBy(s => s.minScoreForSpawning).ToArray();
		}

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
				yield return new WaitForSeconds(Random.Range(spawnTimeFrame.x, spawnTimeFrame.y) + ((GameController.instance != null) ? (GameController.instance.BoostValue - 1) / 4 : 0));

				int highestValidIndex = -1;

				for (int e = 0; e < enemies.Length; ++e)
				{
					if (enemies[e].minScoreForSpawning <= GameController.instance.Score)
					{
						highestValidIndex = e;
					}
					else
					{
						break;
					}
				}

				Instantiate(enemies[Random.Range(0, highestValidIndex + 1)].prefab, transform.position, Quaternion.identity);
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
}