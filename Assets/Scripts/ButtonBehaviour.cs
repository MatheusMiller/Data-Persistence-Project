using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private InputField nameField;
    [SerializeField] private Text[] hsTexts;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Text scoreText;


    private void Start()
    {
        if (GameObject.Find("HighScore") != null)
        {
            scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
            scoreText.text = $"Score: {MainManager.instance.m_Points}";
            quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
            quitButton.enabled = false;
            restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
            restartButton.enabled = false;
            hsTexts = GameObject.Find("HighScore").GetComponentsInChildren<Text>();
            if (MainManager.instance.data.listHSData != null)
            {
                ShowHS();
            }            
        }
    }
    public void OnStartButton()
    {
        SceneManager.LoadScene("main");
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void OnConfirmButton()
    {
        nameField = GameObject.Find("InputField").GetComponent<InputField>();
        
        if (nameField.text != "")
        {
            if (MainManager.instance.data.listHSData !=null && MainManager.instance.data.listHSData.Count >= 5)
            {
                MainManager.instance.data.listHSData[MainManager.instance.data.listHSData.Count - 1].playerName = nameField.text;
                MainManager.instance.data.listHSData[MainManager.instance.data.listHSData.Count - 1].playerPoints = MainManager.instance.m_Points;
            }
            else
            {
                if (MainManager.instance.data.listHSData == null)
                {
                    MainManager.instance.data.listHSData = new List<MainManager.HSData>();
                }
                MainManager.instance.data.listHSData.Add(new MainManager.HSData(nameField.text, MainManager.instance.m_Points));
            }
            MainManager.instance.data.SaveData();
            MainManager.instance.data.OrderList();
            MainManager.instance.data.PrintData();
            ShowHS();
            quitButton.enabled = true;
            restartButton.enabled = true;
        }
    }

    public void ShowHS()
    {
        for (int i = 0; i < MainManager.instance.data.listHSData.Count; i++)
        {
            hsTexts[i].text = MainManager.instance.data.listHSData[i].ToString();
        }
    }
}
