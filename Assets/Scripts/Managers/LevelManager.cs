using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<GameObject> levelList = new List<GameObject>();
    public Transform levelParent;
    int levelIndex, maxLevel;

    const string LEVEL_INDEX = "LevelIndex";
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        maxLevel = levelList.Count;
        levelIndex = PlayerPrefs.GetInt(LEVEL_INDEX, 0);

        LoadLevel(levelIndex);
        UIManager.Instance.UpdateLevelText(levelIndex + 1);
    }

    public void NextLevel()
    {
        levelIndex++;
        PlayerPrefs.SetInt(LEVEL_INDEX, levelIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        Instantiate(levelList[levelIndex % maxLevel], levelParent);
    }
}
