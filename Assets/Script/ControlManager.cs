using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ControlManager : MonoBehaviour
{
    private int points = 0;
    private bool inPause = false;
    public Text highscore;
    public AudioManager audioManager;
    public GameObject playView;
    public GameObject goMenu;
    public GameObject creditsView;
    public GameObject controlsView;
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject leaderBoard;

    public GameObject spawner;
    public Text goPoints;
    public Text pointsText;
    public GameObject fallenPieces;
    public GameObject ground;
    public Text countDown;

    private IEnumerator harder_Coroutine;
    private float smoothTime = 1;
    private float convertedTime = 200;
    private float smooth;

    public float fallInterval = 1.25f;

    private void Start()
    {
        harder_Coroutine = harder();

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && playView.activeInHierarchy)
        {
            if (inPause)
                resume();
            else
                pause();
        }

        if (Input.GetKeyDown(KeyCode.A))
            rotateFieldL();
        else if (Input.GetKeyDown(KeyCode.D))
            rotateFieldR();
    }

    private IEnumerator harder()
    {
        do
        {
            yield return new WaitForSeconds(8f);
            fallInterval -= 0.025f;
            Debug.Log(fallInterval);
        }
        while (fallInterval > 0.47f);
    }

    //Change the active grid with the grid at its right
    public void rotateFieldL()
    {
        Piece piece = FindObjectOfType<Spawner>().GetPiece().GetComponent<Piece>();
        piece.setFreeze(true);
        int activeGrid = Grid.active_grid;

        switch (activeGrid)
        {
            case 1:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(1);
                }
                Grid.active_grid = 2;
                break;
            case 2:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(2);
                }
                Grid.active_grid = 3;
                break;
            case 3:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(3);
                }
                Grid.active_grid = 4;
                break;
            case 4:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(4);
                }
                Grid.active_grid = 1;
                break;
        }

        //TODO: Implementare
        ground.transform.rotation *= MyQuaternion.Euler(0, 90, 0);
        fallenPieces.transform.rotation *= MyQuaternion.Euler(0, 90, 0);

        if (!piece.checkPosition())
        {
            if (piece.transform.position.z == -3)
            {
                piece.transform.position += new Vector3(0, 0, -1);
                //piece.setTransparency(0.8f);
            }
        }
        else
        {
            if (piece.transform.position.z == -4)
            {
                piece.transform.position += new Vector3(0, 0, 1);
                //piece.setTransparency(1);
            }
            piece.updateGrid();
            piece.setFreeze(false);
        }
    }

    //Change the active grid with the grid at its left
    public void rotateFieldR()
    {
        Piece piece = FindObjectOfType<Spawner>().GetPiece().GetComponent<Piece>();
        int activeGrid = Grid.active_grid;
        piece.setFreeze(true);

        switch (activeGrid)
        {
            case 1:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(1);
                }
                Grid.active_grid = 4;
                break;
            case 2:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(2);
                }
                Grid.active_grid = 1;
                break;
            case 3:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(3);
                }
                Grid.active_grid = 2;
                break;
            case 4:
                if (piece.checkPosition())
                {
                    piece.removeForNextGrid(4);
                }
                Grid.active_grid = 3;
                break;
        }

        ground.transform.rotation *= MyQuaternion.Euler(0, -90, 0);
        fallenPieces.transform.rotation *= MyQuaternion.Euler(0, -90, 0);

        if (!piece.checkPosition())
        {
            if (piece.transform.position.z == -3)
            {
                piece.transform.position += new Vector3(0, 0, -1);
                //piece.setTransparency(0.8f);
            }
        }
        else
        {

            if (piece.transform.position.z == -4)
            {
                piece.transform.position += new Vector3(0, 0, 1);
                //piece.setTransparency(1);
            }
            piece.updateGrid();
            piece.setFreeze(false);
        }
    }

    public void givePoints(int fullRows)
    {
        points += (int)Mathf.Ceil(100 * (Mathf.Pow(fullRows, 1.5f)));
        pointsText.text = points + " ";
    }

    public void startPlaying()
    {
        StartCoroutine(harder_Coroutine);
        spawner.SetActive(true);
        playView.SetActive(true);
        startMenu.SetActive(false);
        audioManager.stop("MenuTheme");
        audioManager.play("Main");
    }

    public void showHelp()
    {
#if UNITY_ANDROID
        SceneManager.LoadScene("Tutorial");
#endif
#if UNITY_WEBGL
        startMenu.SetActive(false);
        helpMenu.SetActive(true);
#endif
    }

    public void showCredits()
    {
        startMenu.SetActive(false);
        creditsView.SetActive(true);
    }

    public void showControls()
    {
        startMenu.SetActive(false);
        controlsView.SetActive(true);
    }

    public void showLeaderBoard()
    {
        startMenu.SetActive(false);
        for (int i = 1; i <= 5; ++i)
            highscore.text += i + ") " + PlayerPrefs.GetInt("Score" + i) + "\n";
        leaderBoard.SetActive(true);
    }

    public void backToMenu()
    {
        startMenu.SetActive(true);
        goMenu.SetActive(false);
        leaderBoard.SetActive(false);
        controlsView.SetActive(false);
        creditsView.SetActive(false);
        playView.SetActive(false);
        highscore.text = "";
        points = 0;
        pointsText.text = 0 + "";
    }

    public void backFromGO()
    {
        foreach (Transform child in fallenPieces.transform)
        {
            Destroy(child.gameObject);
        }
        Grid.clean();
        startMenu.SetActive(true);
        goMenu.SetActive(false);
#if UNITY_WEBGL
        helpMenu.SetActive(false);
#endif
        playView.SetActive(false);
        audioManager.play("MenuTheme");
    }



    public void gameOver()
    {
        StopCoroutine(harder_Coroutine);
        spawner.SetActive(false);
        goMenu.SetActive(true);
        playView.SetActive(false);
        goPoints.text = points + " POINTS";
        setLeaderBoard();
        pointsText.text = 0 + " ";
        points = 0;
        fallInterval = 1f;
        audioManager.stop("Main");
        audioManager.play("Defeat");
    }

    public void replay()
    {
        StartCoroutine(harder_Coroutine);
        foreach (Transform child in fallenPieces.transform)
        {
            Destroy(child.gameObject);
        }

        Grid.clean();
        spawner.SetActive(true);
        playView.SetActive(true);
        goMenu.SetActive(false);
        audioManager.play("Main");
    }

    public void pause()
    {
        inPause = true;
        pauseMenu.SetActive(true);
        playView.SetActive(false);
        FindObjectOfType<Spawner>().GetPiece().GetComponent<Piece>().enabled = false;
        StopCoroutine(harder_Coroutine);
    }

    public void resume()
    {
        inPause = false;
        pauseMenu.SetActive(false);
        playView.SetActive(true);
        FindObjectOfType<Spawner>().GetPiece().GetComponent<Piece>().enabled = true;
        StartCoroutine(harder_Coroutine);
    }

    public void exit()
    {
        spawner.GetComponent<Spawner>().GetPiece().transform.SetParent(GameObject.FindGameObjectWithTag("FallenPieces").transform, true);
        foreach (Transform child in fallenPieces.transform)
        {
            Destroy(child.gameObject);
        }
        Grid.clean();

        playView.SetActive(false);
        spawner.SetActive(false);
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
        audioManager.stop("Main");
        audioManager.play("MenuTheme");
    }

    private void setLeaderBoard()
    {
        for (int i = 1; i <= 5; ++i)
            if (PlayerPrefs.GetInt("Score" + i) < points)
            {
                int j = 5;
                while (j > i)
                {
                    PlayerPrefs.SetInt("Score" + (j), PlayerPrefs.GetInt("Score" + (j - 1)));
                    j--;
                }
                PlayerPrefs.SetInt("Score" + i, points);
                break;
            }
    }

    public void movePieceR()
    {
        spawner.GetComponent<Spawner>().GetPiece().GetComponent<Piece>().translateRight();
    }

    public void movePieceL()
    {
        spawner.GetComponent<Spawner>().GetPiece().GetComponent<Piece>().translateLeft();
    }

    public void movePieceD()
    {
        spawner.GetComponent<Spawner>().GetPiece().GetComponent<Piece>().translateDown();
    }

    public void rotate()
    {
        spawner.GetComponent<Spawner>().GetPiece().GetComponent<Piece>().rotate();
    }

    public void CountDown(int n)
    {
        countDown.text = "" + n;
    }

    public void exitApp()
    {
        Application.Quit();
    }
}