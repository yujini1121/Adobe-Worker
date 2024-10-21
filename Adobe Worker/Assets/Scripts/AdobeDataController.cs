using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
	public static PlayerData instance;

	[Header("Status")]
	public bool isDead;
	public string name;
	public int gold;

	[Space(30)]
	[Header("Attributes")]
	public int health;
	public int hunger;
	public int sanity;

	[Space(30)]
	[Header("Inventory")]
	public string[] items;


	public PlayerData()
	{
		items = new string[0];
		instance = this;
	}
}


public class AdobeDataController : MonoBehaviour
{
	[SerializeField] private PlayerData playerData;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		LoadData();
	}

	public void SaveData()
	{
		string json = JsonUtility.ToJson(playerData, true);
		string path = Application.dataPath + "/Resources/Json Files/PlayerData.json";
		File.WriteAllText(path, json);
		Debug.Log("µÇ³Ä");
	}

	public void LoadData()
	{
		string path = Application.dataPath + "/Resources/Json Files/PlayerData.json";

		if (File.Exists(path))
		{
			string data = File.ReadAllText(path);
			playerData = JsonUtility.FromJson<PlayerData>(data);
		}
		else
		{
			Debug.LogError("Not Found 'PlayerData.json' File.");
		}
	}
}