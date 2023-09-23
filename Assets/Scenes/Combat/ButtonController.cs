using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public TMP_Text difficultyIndicator;
    private static string fileName="SaveFiles/saveFile1";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startANewGame(){
        SceneManager.LoadScene("Overworld#1");
    }

    public void loadAGame(){
        Overlord.Instance.loadSaveData(fileName);
        SceneManager.LoadScene(Overlord.Instance.sceneName);
    }

    public void openOptions(){
        SceneManager.LoadScene("MainOptions");
        Debug.Log("HI");
    }

    public void setEasyMode(){
        Overlord.Instance.difficulty=-1;
        difficultyIndicator.text="easy";
    }

    public void setMediumMode(){
        Overlord.Instance.difficulty=0;
        difficultyIndicator.text="medium";
    }

    public void setHardMode(){
        Overlord.Instance.difficulty=1;
        difficultyIndicator.text="hard";
    }

}
