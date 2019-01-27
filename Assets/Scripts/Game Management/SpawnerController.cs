	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour 
{
	[Header("SpawnPoint")]
	[SerializeField] GameObject spawnPoint = null;

	[Header("Spawnable Prefabs")]
	[SerializeField] GameObject[] pickUpItems = null;
	[Space(30)]
	[SerializeField] GameObject[] friendlyNPCs = null;
	[Space(30)]
	[SerializeField] GameObject[] EnemyNPCs = null;

	public void SpawnItem()
	{
		Spawn(pickUpItems[Random.Range(0, pickUpItems.Length)]);
	}

	public void SpawnFriendlyNPC()
	{
		Spawn(friendlyNPCs[Random.Range(0, friendlyNPCs.Length)]);
	}

	public GameObject SpawnEnemyNPC()
	{
		return Spawn(EnemyNPCs[Random.Range(0, EnemyNPCs.Length)]);
	}

	GameObject Spawn(GameObject _objectToSpawn)
	{
		gameObject.transform.rotation = Random.rotation;

		RaycastHit hit;
		
		if(Physics.Raycast(spawnPoint.transform.position, gameObject.transform.position - spawnPoint.transform.position, out hit))
		{
			if(hit.collider.tag == "Planet")
			{
				Vector3 Spawnpos = hit.point + Vector3.Normalize(spawnPoint.transform.position - hit.point) * 0.284f;

				GameObject temp = Instantiate(_objectToSpawn, Spawnpos, Quaternion.identity);
				temp.transform.up = hit.normal;

				return temp;
			}
		}

		return Spawn(_objectToSpawn);
	}
}
