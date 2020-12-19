using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData _saveData;
    // Start is called before the first frame update
    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();
    }
    public void Start()
    {
        
    }
    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);
        SaveData data = new SaveData();
        data = _saveData;
        formatter.Serialize(file, data);
        file.Close();
        
    }
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            _saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
        }
        else
        {
            _saveData = new SaveData();
            _saveData.isActive = new bool[100];
            _saveData.stars = new int[100];
            _saveData.highScores = new int[100];
            _saveData.isActive[0] = true;
        }
    }
    /*private void OnApplicationQuit()
    {
        Save();
    }*/
    /*public void OnDisable()
    {
        Save();
    }*/
}
