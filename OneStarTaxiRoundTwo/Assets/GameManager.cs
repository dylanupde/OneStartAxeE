using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Text p1Text;
    [SerializeField] Text p2Text;
    [SerializeField] Text countdownText;
    [SerializeField] GameObject winnerTextObj;
    [SerializeField] int scoreNeededToWin = 5;

    [HideInInspector] public List<PassengerCollider> pickups;
    [HideInInspector] public List<PassengerDropoff> dropoffs;
    [HideInInspector] public bool gameHasStarted;

    int player1Score = 0;
    int player2Score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pickups = new List<PassengerCollider>();
        dropoffs = new List<PassengerDropoff>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("TestReqs", 0.5f);
        gameHasStarted = false;
        StartCoroutine(StartCountdown());

        StartCoroutine(ActivateRandomPickupDropoffNextFrame());
    }

    IEnumerator ActivateRandomPickupDropoffNextFrame()
    {
        yield return null;
        ActivateRandomPickupDropoff();
    }

    private void TestReqs()
    {
        int number = (scoreNeededToWin * 2) - 1;
        if (number > dropoffs.Count + 1)
        {
            Debug.LogError("HEY FUCKFACE, the gay manager's scoreNeededToWin needs to be less than half the number dropoffs!");
        }
        else
        {
            Debug.Log("scoreneeded: " + number);
            Debug.Log("dropoffs: " + dropoffs.Count);
        }
    }


    public void AddToScore(bool player1Scored)
    {
        if (player1Scored)
        {
            player1Score++;
            p1Text.text = "P1: " + player1Score.ToString();
        }
        else
        {
            player2Score++;
            p2Text.text = "P2: " + player2Score.ToString();
        }


        if (player1Score >= scoreNeededToWin)
        {
            Win(true);
        }
        else if (player1Score >= scoreNeededToWin)
        {
            Win(false);
        }

        ActivateRandomPickupDropoff();
    }


    void Win(bool player1Won)
    {
        winnerTextObj.SetActive(true);
        gameHasStarted = false;

        if (player1Won)
        {
            winnerTextObj.GetComponent<Text>().text = "PLAYER 1 WINS!";
        }
        else
        {
            winnerTextObj.GetComponent<Text>().text = "PLAYER 2 WINS!";
        }

        Invoke("ReloadScene", 4f);
    }


    void ActivateRandomPickupDropoff()
    {
        if (pickups.Count > 0)
        {
            int randomPickup = Random.Range(0, pickups.Count - 1);
            int randomDropoff = Random.Range(0, dropoffs.Count - 1);
            pickups[randomPickup].transform.parent.gameObject.SetActive(true);
            dropoffs[randomDropoff].gameObject.SetActive(true);
            pickups[randomPickup].myDropoff = dropoffs[randomDropoff];

            pickups.RemoveAt(randomPickup);
            dropoffs.RemoveAt(randomDropoff);
        }
        else
        {
            Debug.Log("NO MORE PICKUPS/DROPOFFS LEFT!!!!");
        }
    }


    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }




    IEnumerator StartCountdown()
    {
        countdownText.text = "5";
        yield return new WaitForSeconds(1f);
        countdownText.text = "4";
        yield return new WaitForSeconds(1f);
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.text = "GO!";
        gameHasStarted = true;
        yield return new WaitForSeconds(1.5f);
        countdownText.gameObject.SetActive(false);
    }
}
