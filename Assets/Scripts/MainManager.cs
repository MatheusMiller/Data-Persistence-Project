using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;

// create a new scene "Game Over" that will show the game over text and
// the HS game over text, in this you will input your name, after that
// will show the HS in both cases, quit and restart/start button on it.
// Continue in game over section.
public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;
    private bool m_HSGameOver = false;

    private List<HSData> hSDatas = new List<HSData>();
    private Data data = new Data();


    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        data.LoadData();
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {

        if (data.listHSData.Count < 5 || data.listHSData[4].playerPoints < m_Points)
        {
            m_HSGameOver = true;
            Debug.Log("HS");
        }
            
        else
        {
            m_GameOver = true;
            Debug.Log("no HS");
            //GameOverText.SetActive(true);
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
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(json, this);
                
                foreach (HSData data in listHSData)
                {
                    Debug.Log(data);
                }
                Debug.Log("------");
                OrderList();
                foreach (HSData data in listHSData)
                {
                    Debug.Log(data);
                }
            }
        }

        public void OrderList()
        {
            listHSData = listHSData.OrderByDescending(player => player.playerPoints).ToList();
        }



    }
    


}
