using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using UnityEditor.Events;

// create a new scene "Game Over" that will show the game over text and
// the HS game over text, in this you will input your name, after that
// will show the HS in both cases, quit and restart/start button on it.
// Continue in game over section.
public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    [SerializeField] private Brick BrickPrefab;
    public int LineCount = 6;
    [SerializeField] private Rigidbody Ball;
    [SerializeField] private Brick brickAdded;

    [SerializeField] private Text ScoreText;
    [SerializeField] private Text BestScoreText;
    [SerializeField] private GameObject GameOverText;

    [SerializeField] private bool m_Started = false;
    public int m_Points;

    [SerializeField] private bool m_GameOver = false;
    [SerializeField] private bool m_HSGameOver = false;
    public Data data = new Data();


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);     
        }

        data.LoadData();
        
        instance.Ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
        instance.ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        instance.BrickPrefab = Resources.Load<Brick>("BrickPrefab").GetComponent<Brick>();
        instance.BestScoreText = GameObject.Find("BestScoreText").GetComponent<Text>();
        instance.GameOverText = GameObject.Find("GameOverText");
        instance.GameOverText.SetActive(false);
        instance.m_Points = 0;
        // falta a Game Over Scene e verificar se o json está funcionado

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                instance.brickAdded = Instantiate(instance.BrickPrefab, position, Quaternion.identity);
                instance.brickAdded.PointValue = pointCountArray[i];
            }
        }
        
        if (data.listHSData != null && data.listHSData.Count > 0)
        {
            BestScoreText.text = $"Best Score: " + data.listHSData[0].playerName + " => " + data.listHSData[0].playerPoints;
        }
        else
        {
            BestScoreText.text = "No best score yet";
        }
        
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            instance.GameOverText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = false;
                m_GameOver = false;
                instance.GameOverText.SetActive(false);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }
        }

        else if (m_HSGameOver)
        {
            m_Started = false;
            m_HSGameOver = false;
            SceneManager.LoadScene("GameOver");
        }
    }

    public void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {

        if (data.listHSData == null || (data.listHSData.Count < 5 || data.listHSData[4].playerPoints < m_Points))
        {
            m_HSGameOver = true;

        }
            
        else
        {
            m_GameOver = true;
        }

    }

    [Serializable]
    public class HSData
    {
        public string playerName;
        public int playerPoints;

        public HSData(string name, int points)
        {
            playerName = name;
            playerPoints = points;
        }

        public override string ToString()
        {
            return "Name: " + playerName + "     Points: " + playerPoints;
        }
    }

    [Serializable]
    public class Data
    {
        public List<HSData> listHSData;

        public void SaveData()
        {
           
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(Application.persistentDataPath + "/savedata.json", json);
        }

        public void LoadData()
        {
            string path = Application.persistentDataPath + "/savedata.json";
            Debug.Log(path);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(json, this);
                OrderList();

            }
        }

        public void OrderList()
        {
            listHSData = listHSData.OrderByDescending(player => player.playerPoints).ToList();
        }

        public void PrintData()
        {
            foreach(HSData data in listHSData)
            {
                Debug.Log(data.ToString());
            }
        }

    }
    


}
