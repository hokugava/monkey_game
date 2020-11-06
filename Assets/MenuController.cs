using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private string ccode;
    private bool checkGeoApproval, checkAttribListApproval, isHandleOrganic = false;
    public GameObject menuBlock;
    private int filterIs = 0;

    private string sorosKey, sorosValue;
    private bool availableCountry = false;

    void Start()
    {
        menuBlock.SetActive(true);
    }

    public void startLevel()
    {
        SceneManager.LoadScene("GamePlatformer");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}