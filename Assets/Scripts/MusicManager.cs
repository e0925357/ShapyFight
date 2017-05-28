using UnityEngine;

namespace Assets.Scripts
{
	public class MusicManager : MonoBehaviour
	{
		void Start()
		{
			if (GameObject.FindGameObjectsWithTag("Music").Length > 1)
			{
				Destroy(gameObject);
			}
			else
			{
				DontDestroyOnLoad(gameObject);
			}
		}
	}
}