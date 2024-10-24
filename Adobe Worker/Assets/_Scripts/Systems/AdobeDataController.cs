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
	public List<ItemEntry> items = new List<ItemEntry>();

	public PlayerData()
	{
		instance = this;
	}
}

/// <summary>
/// 단순히 인벤토리를 2차원 리스트로 바꾸었을 때, 직렬화가 되지 않아 새로 정의하는 클래스일뿐. 큰 뜻은 없음.
/// </summary>
[System.Serializable]
public class ItemEntry
{
	public int id;
	public int amount;  

	public ItemEntry(int id, int amount)
	{
		this.id = id;
		this.amount = amount;
	}
}



public class AdobeDataController : MonoBehaviour
{
	[SerializeField] private PlayerData playerData;
	[SerializeField] private ItemEntry itemEntry;

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