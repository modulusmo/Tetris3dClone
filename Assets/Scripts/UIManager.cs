using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public delegate void ResetGame();
    public static event ResetGame OnResetGame;

    public delegate void ToggleMusic();
    public static event ToggleMusic OnToggleMusic;
    // Start is called before the first frame update
    public Text txtLevel;
    public Text txtScore;

    void OnEnable (){
        GameManager.OnLevelChange += GameManager_OnLevelChange;
        GameManager.OnScoreChange += GameManager_OnScoreChange;
    }
    void OnDisable(){
        GameManager.OnLevelChange -= GameManager_OnLevelChange;
        GameManager.OnScoreChange -= GameManager_OnScoreChange;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBtnClick(Button b)
    {
        Debug.Log("A button was pressed");
        switch(b.name){
            case "btn_Start":
                SceneManager.LoadScene("GameBoard");
                break;
            case "btn_Main" :
                SceneManager.LoadScene("GameStart");
                break;
            case "btn_Reset":
                if(OnResetGame != null)
                    OnResetGame();
                break;
            case "btn_Music":
                if(OnToggleMusic!=null)
                    OnToggleMusic();
                break;
            case "btn_Exit" :
                Application.Quit();
                break;
        }
    }
    
    void GameManager_OnLevelChange(int level)
    {
        txtLevel.text = "Level:\n" + level.ToString();
    }
    void GameManager_OnScoreChange(int score)
    {
        txtScore.text = "Score\n" + score.ToString();
    }
}
