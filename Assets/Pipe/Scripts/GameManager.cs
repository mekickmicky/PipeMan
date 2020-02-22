using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRanking
{
    internal string playerName = "";
    internal int score = 0;
    public PlayerRanking(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public bool isGameStart = false;

    [Header("Pipe")]
    public GameObject pipePrefab;
    public Transform groupPipe;
    public int rangePipe = 20;
    public int limitPipe = 40;

    public int lv = 1;
    public List<Pipe> listPipe = new List<Pipe>();

    [Header("Player")]
    private string playerName = "";
    public Transform player;
    public Ball ball;
    public Transform startPoint;
    public float speed = 30;
    public float currentRange = 0;
    private int score = 0;
    public InputPad inputControl;
    public float speedCtrl = 2;


    [Header("UI")]
    public GameObject mainPanel;
    public GameObject gameoverPanel;
    public GameObject rankingPanel;
    public GameObject tutorialPanel;
    [SerializeField]
    internal InputField playerNameInput;
    public Text scoreText;
    public Text highScoreText;

    public GameObject loading;


    [Header("Popup")]
    public GameObject popupPanel;
    public Text popupText;

    [Header("Ranking")]
    public GameObject boxPlayerRankPrefab;
    public Transform content;
    public List<GameObject> listBox = new List<GameObject>();

    public PlayfabController pfc;

    public AudioSource audioS;
    public AudioClip clickAudio;

    private bool firstTime = false;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
            playerNameInput.text = "PlayerName";
        else
            playerNameInput.text = PlayerPrefs.GetString("PlayerName");
        highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        if(PlayerPrefs.GetInt("HighScore") == 0)
        {
            PlayerPrefs.SetInt("Lv", 1);
            PlayerPrefs.SetInt("HighScore", 0);
            firstTime = true;
        }
        ball = player.GetComponentInChildren<Ball>();
        SoundSetup();
        Init();
        BackToMain();
    }
    private void SoundSetup()
    {
        var btns = FindObjectsOfType<Button>();
        foreach (var item in btns)
        {
            item.onClick.AddListener(OnClickBtn);
        }
    }
    public void OnClickBtn()
    {
        audioS.PlayOneShot(clickAudio);
    }

    private void Init()
    {
        score = 0;
        currentRange = 0;
        lv = 1;
        groupPipe.rotation = Quaternion.identity;
        inputControl.currentRotX = 0;

        ClearPipe();
        listPipe.Add(Instantiate(pipePrefab.GetComponent<Pipe>(), groupPipe));
        for (int i = 0; i < rangePipe; i++)
        {
            SetPipeChain(listPipe[listPipe.Count - 1].chainPoint);
        }
        player.position = startPoint.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("RESET");
            PlayerPrefs.SetInt("Lv", 1);
            PlayerPrefs.SetInt("HighScore", 0);
        }

        if (!isGameStart)
            return;

        player.transform.Translate(transform.forward * speed * Time.deltaTime);
        if (player.transform.position.z > currentRange+(pipePrefab.GetComponent<Pipe>().plane[0].localScale.z))
        {
            currentRange = (int)player.transform.position.z;
            SetPipeChain(listPipe[listPipe.Count - 1].chainPoint);
            score += 20;
            scoreText.text = score + "";
            if (score % 800 == 0)
                lv++;
        }
        speed += Time.deltaTime / 2;

    }
    public void SetPipeChain(Transform chainPoint)
    {
        var pipe = Instantiate(pipePrefab.GetComponent<Pipe>(), chainPoint.position, chainPoint.rotation, groupPipe);
        listPipe.Add(pipe);
        pipe.SetPipe();
        if (listPipe.Count > limitPipe)
        {
            Destroy(listPipe[0].gameObject);
            listPipe.RemoveAt(0);
        }
    }

    public void GameOver()
    {
        speed = 0;
        if(PlayerPrefs.GetInt("HighScore")< score)
        {
            PlayerPrefs.SetInt("Lv", lv);
            PlayerPrefs.SetInt("HighScore", score);
            pfc.SetStatis(score);
        }
        gameoverPanel.SetActive(true);
        isGameStart = false;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        ball.audioS.PlayOneShot(ball.gameoverAudio);
    }
    public void StartGame()
    {
        speed = 30 * (PlayerPrefs.GetInt("Lv") / 1.8f);
        scoreText.gameObject.SetActive(true);
        scoreText.text = "0";
        isGameStart = true;

        Init();
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.GetComponent<Animator>().enabled = false;
        ball.audioS.loop = false;
        ball.audioS.PlayOneShot(ball.spinAudio);
        mainPanel.SetActive(false);
        gameoverPanel.SetActive(false);

        if (firstTime)
        {
            StartCoroutine(ShowTutorial());
        }
    }
    IEnumerator ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        firstTime = false;
        yield return new WaitForSeconds(10f);
        tutorialPanel.SetActive(false);
    }
    public void BackToMain()
    {
        highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        scoreText.gameObject.SetActive(false);
        mainPanel.SetActive(true);
        gameoverPanel.SetActive(false);
        rankingPanel.SetActive(false);
        popupPanel.SetActive(false);
        loading.SetActive(false);
        ball.GetComponent<Animator>().enabled = true;
        ball.audioS.loop = true;
        ball.audioS.clip = ball.mainMenuAudio;
        ball.audioS.Play();
    }

    public void SetPopup(string msg)
    {
        popupText.text = msg;
        popupPanel.SetActive(true);
    }

    private void ClearPipe()
    {
        listPipe.ForEach(v => Destroy(v.gameObject));
        listPipe.Clear();
    }
    public void SetDisplayname()
    {
        if (playerNameInput.text.Length > 20)
        {
            SetPopup("Your name have letter more 20 character.");
            return;
        }


        loading.SetActive(true);
        pfc.SetDisplayName(playerNameInput.text);
    }
    public void Ranking()
    {
        listBox.ForEach(v => { Destroy(v); });
        listBox.Clear();
        rankingPanel.SetActive(true);
        loading.SetActive(true);
        pfc.GetRank();
    }


    public void SetRanking(List<PlayerRanking> listPlayer)
    {
        int count = 1;
        foreach (var player in listPlayer)
        {
            var item = Instantiate(boxPlayerRankPrefab.GetComponent<ItemPlayerRank>(), content);
            item.SetItem(count,player.playerName, player.score);
            listBox.Add(item.gameObject);
            count++;
        }
        loading.SetActive(false);
    }
}
