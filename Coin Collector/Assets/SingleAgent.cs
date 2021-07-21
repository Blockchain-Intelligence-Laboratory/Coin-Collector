using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;           // 유니티에서 스크립트 생성시 초기 오버라이드는 Monohavior,
using Unity.MLAgents.Sensors;   // 초기 오버라이드에서는 ML-Agents를 사용할 수 없기 때문에
using Unity.MLAgents.Actuators; // MLAgnets 형식으로 오버라이드를 바꿔줌

public class SingleAgent : Agent // RollerAgent의 클래스, 에이전트의 속성을 정의해준다.
{
    // Agent 설정
    private Text singleScore;
    public int coinCounter = 0;
    Rigidbody rBody; // Rigidbody 컴포넌트, 넣어주지 않으면 표면이 적용되지 않아 바닥을 뚫는다.
    void Start() // 게임을 실행할 시 실행되는 메소드를 담아두는 공간
    {
        rBody = GetComponent<Rigidbody>(); // 에이전트의 바디에 Rigidbody 컴포넌트 적용
        singleScore = GameObject.Find("singleScore").GetComponent<Text>();
        SetCountText();
    }

    // 목표물 설정
    public Transform Target; // 타겟 오브젝트 선언

    /* 에이전트가 목표에 도달할 때 마다 에피소드 종료 후,
	새로운 에피소드가 생성됨에 따른 환경을 설정해주기 위한 메소드 */
    public override void OnEpisodeBegin() // 각 에피소드가 시작될때마다 실행
    {
        // 에이전트가 바닥 밑으로 떨어지면 위치를 초기화 시켜주는 메소드
        // 바닥의 y축 좌표가 0이기 때문에 에이전트의 y 좌표가 0 미만이면 떨어졌다고 판단 가능
        if (this.transform.localPosition.y < 0) // 만약 에이전트의 y축 좌표가 1보다 작아지면,
        {
            this.rBody.angularVelocity = Vector3.zero; // 에이전트의 구르는 속도 초기화
            this.rBody.velocity = Vector3.zero;        // 에이전트의 움직이는 속도 초기화
            this.transform.localPosition = new Vector3(0, 1f, 0); // 에이전트의 위치 초기화
        }

        // 타겟 오브젝트를 랜덤하게 이동시켜주는 메소드
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f, Random.value * 8 - 4); // 타겟의 좌표를 랜덤하게 변경
    }

    /* 에이전트가 환경에 대한 행동 결정을 내리는 메소드 */
    public override void CollectObservations(VectorSensor sensor)
    {
        // 센서를 이용하여 타겟과 에이전트의 위치를 파악
        sensor.AddObservation(Target.localPosition); // 타겟의 위치를 파악
        sensor.AddObservation(this.transform.localPosition); // 에이전트의 위치를 파악

        // 센서를 이용하여 에이전트의 속도를 파악
        sensor.AddObservation(rBody.velocity.x); // 에이전트의 x축 이동 속도 파악
        sensor.AddObservation(rBody.velocity.z); // 에이전트의 z축 이동 속도 파악
    }

    public float forceMultiplier = 10; // 에이전트가 이동할 때 받는 힘의 정도. 높을수록 빠르게 움직인다.

    /* 에이전트의 액션에 대한 보상을 결정하는 메소드
	보상 획득 이후, 에피소드를 종료하는 분기를 설정 가능 */
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero; // 에이전트를 움직이는 신호정의, 3차원이므로 Vector3 사용
        controlSignal.x = actionBuffers.ContinuousActions[0]; // x축으로 이동하는 신호
        controlSignal.z = actionBuffers.ContinuousActions[1]; // z축으로 이동하는 신호
        rBody.AddForce(controlSignal * forceMultiplier); // controlSignal로 방향결정 후, 힘을 곱하여 이동하게 만듦

        // 타겟과의 거리를 저장하는 변수, 에이전트, 타겟의 좌표를 받아와서 거리를 계산하여 저장.
        // 이때, 거리는 각 오브젝트의 중심으로부터의 거리이다.
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // 보상 정의

        // 타겟과의 거리가 1.42f 미만일 경우 잡았다고 판단,
        // 이때, 1.42f는 (에이전트 오브젝트의 반지름) + (타겟 오브젝트의 면의 길이 / 2)
        // 즉, 서로의 표면이 닿았을 경우의 각 중심으로부터의 최대거리
        if (distanceToTarget < 1.42f) // 타겟을 잡았을 경우,
        {
            SetReward(1.0f); // 보상을 1점 얻는다.
            coinCounter += 1; // 수집한 동전의 개수를 +1 한다.
            SetCountText();

            EndEpisode(); // 현재 에피소드를 끝낸다.
        }

        // 에이전트의 y 좌표가 0 미만일 경우 바닥에서 떨어졌다고 판단,
        // 이때, 0은 바닥 오브젝트의 y 좌표
        // 즉, 에이전트의 y 좌표가 0 미만이라면 바닥 오브젝트보다 밑에 있다는 의미이므로 떨어진 것.
        else if (this.transform.localPosition.y < 0) // 에이전트가 바닥 밖으로 떨어졌을 경우,
        {
            SetReward(-3.0f); // 보상을 3점 잃는다.
            EndEpisode(); // 현재 에피소드를 끝낸다.
        }
    }

    void SetCountText()
    {
        singleScore.text = "Coins: " + coinCounter.ToString();
    }
}
