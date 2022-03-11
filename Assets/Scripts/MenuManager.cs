using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject recipesMenu;
    public GameObject tutorialMenu;
    public GameObject deadScreen;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Pause();
        }

        if(!FindObjectOfType<Player>() && SceneManager.GetActiveScene().name == "Game")
        {
            Dead();
        }
    }
    void Pause(){
        if(recipesMenu.activeSelf || tutorialMenu.activeSelf){
            recipesMenu.SetActive(false);
            tutorialMenu.SetActive(false);
            return;
        }
        if(!menu.activeSelf){
            Time.timeScale = 0f;
            menu.SetActive(true);
        }else if(menu.activeSelf){
            Time.timeScale = 1f;
            menu.SetActive(false);
        }
    }

    void Dead()
    {
        deadScreen.SetActive(true);       
    }

    public void ChangeScene(int scene){
        SceneManager.LoadSceneAsync(scene);
    }
    public void Quit(){
        Application.Quit();
    }
}
