using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject playerTank, enemiePrefab, spawnPoints;
    public TextMeshProUGUI timer, scoreTxt;
    private int score;
    private float time;
    void Start()
    {
        InvokeRepeating("RespawnEnemie", 3f, 5f);
    }

    private void Update()
    {
        CheckPlayer();
        if (playerTank.activeSelf)
        {
            time += Time.deltaTime;
            timer.text = "Time: " + time.ToString("F2");
        }
    }

    void FixedUpdate()
    {
        mainCamera.transform.position = new Vector3(playerTank.transform.position.x, playerTank.transform.position.y + 70, playerTank.transform.position.z);
    }

    void RespawnEnemie()
    {
        var enemie = Instantiate(enemiePrefab);
        var randomPosition = Random.Range(0, spawnPoints.transform.childCount);
        enemie.transform.position = spawnPoints.transform.GetChild(randomPosition).position;
    }

    void CheckPlayer()
    {
        if (!playerTank.gameObject.activeSelf)
        {
            Invoke("RestartGame", 3f);
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene("ActivitatScene");
    }

    public void DeadTank()
    {
        score += 1;
        scoreTxt.text = score.ToString();
    }
}
