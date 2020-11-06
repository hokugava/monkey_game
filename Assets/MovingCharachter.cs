using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class MovingCharachter : MonoBehaviour
{
    public PlayerController controller;
    private float horizontaleMove = 0f;
    public float runSpeed = 40f;
    private bool jump = false;

    public GameObject gameOverBlock;
    private bool getTrouble = false;
    public Animator animator;
    public List<GameObject> hearts;
    private int heartsCount = 3;

    public GameObject explosion;
    
    //Mobile Movement
    private float x;
    public Text scoreInCoins;

    void Start()
    {
        explosion.SetActive(false);
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("score", 0);
        scoreInCoins.text = "0";
        gameOverBlock.SetActive(false);
    }

    void Update()
    {
        scoreInCoins.text = PlayerPrefs.GetInt("score", 0).ToString();
        x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        
        horizontaleMove = x * runSpeed;
        animator.SetFloat("Speed",Mathf.Abs(horizontaleMove));
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping",true);
        }

        if (getTrouble)
        {
            animator.SetBool("isTrouble",true);
        }
    }

    public void restartLevel()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("gameOvers",PlayerPrefs.GetInt("gameOvers",0)+1);
        if (PlayerPrefs.GetInt("gameOvers") == 3)
        {
            
            PlayerPrefs.SetInt("gameOvers", 0);
        }
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void OnLanding()
    {
        animator.SetBool("isJumping",false);
    }

    public void goHome()
    {
        SceneManager.LoadScene("MainScreen");
    }

    void FixedUpdate()
    {
        controller.Move(horizontaleMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    public void GameOver()
    {
        foreach (var heart in hearts)
        {
            heart.SetActive(false);
        }
        gameOverBlock.SetActive(true);
        // Time.timeScale = .000001f;
        
    }

    public void shareMe()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Trouble"))
        {
            heartsCount -= 1;
            switch (heartsCount)
            {
                case 2:
                    hearts[0].SetActive(false); 
                    break;
                case 1:
                    hearts[1].SetActive(false); 
                    break;
                case 0:
                    hearts[2].SetActive(false); 
                    break;
                    
            }
            if (heartsCount == 0)
            {
                getTrouble = true;
            }
        }

        if (col.gameObject.CompareTag("Loot"))
        {
            Destroy(col.gameObject);
            PlayerPrefs.SetInt("score",PlayerPrefs.GetInt("score",0)+1);
        }
        
        if (col.gameObject.CompareTag("Death"))
        {
            GameOver();
        }
        
        if (col.gameObject.CompareTag("Bomb"))
        {
            Destroy(col.gameObject);
            explosion.SetActive(true);
            getTrouble = true;
        }
    }

    public void explosionEnds()
    {
        explosion.SetActive(false);
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
        ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
        ss.Apply();
        string filePath = Path.Combine( Application.temporaryCachePath, "shared img.png" );
        File.WriteAllBytes( filePath, ss.EncodeToPNG() );
        Destroy( ss );
        new NativeShare().AddFile( filePath )
            .SetSubject( "Subject goes here" ).SetText( "Hello world!" )
            .SetCallback( ( result, shareTarget ) => Debug.Log( "Share result: " + result + ", selected app: " + shareTarget ) )
            .Share();

    }
}