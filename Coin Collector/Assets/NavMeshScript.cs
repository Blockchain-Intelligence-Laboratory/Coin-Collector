using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;

public class NavMeshScript : MonoBehaviour
{
    private Transform target;
    private Transform thisOB;
    private NavMeshAgent agent;
    private Text Score;
    public int coinCounter = 0;

    void Start() {
        target = GameObject.Find("NavCoin").GetComponent<Transform>();
        thisOB = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        Score = GameObject.Find("NavScore").GetComponent<Text>();
        SetCountText();
    }

    void Update() {
        agent.destination = target.position;
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        if (distanceToTarget < 1.42f)
        {
            coinCounter += 1;
            SetCountText();
            resetCoin();
        }
    }

    void resetCoin() {
        target.localPosition = new Vector3(Random.value * 8 - 4,
                                   0.5f, Random.value * 8 - 4); // Å¸°ÙÀÇ ÁÂÇ¥¸¦ ·£´ýÇÏ°Ô º¯°æ
    }

    void SetCountText()
    {
        Score.text = "Coins: " + coinCounter.ToString();
    }
}
