using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;           // ����Ƽ���� ��ũ��Ʈ ������ �ʱ� �������̵�� Monohavior,
using Unity.MLAgents.Sensors;   // �ʱ� �������̵忡���� ML-Agents�� ����� �� ���� ������
using Unity.MLAgents.Actuators; // MLAgnets �������� �������̵带 �ٲ���

public class SingleAgent : Agent // RollerAgent�� Ŭ����, ������Ʈ�� �Ӽ��� �������ش�.
{
    // Agent ����
    private Text singleScore;
    public int coinCounter = 0;
    Rigidbody rBody; // Rigidbody ������Ʈ, �־����� ������ ǥ���� ������� �ʾ� �ٴ��� �մ´�.
    void Start() // ������ ������ �� ����Ǵ� �޼ҵ带 ��Ƶδ� ����
    {
        rBody = GetComponent<Rigidbody>(); // ������Ʈ�� �ٵ� Rigidbody ������Ʈ ����
        singleScore = GameObject.Find("singleScore").GetComponent<Text>();
        SetCountText();
    }

    // ��ǥ�� ����
    public Transform Target; // Ÿ�� ������Ʈ ����

    /* ������Ʈ�� ��ǥ�� ������ �� ���� ���Ǽҵ� ���� ��,
	���ο� ���Ǽҵ尡 �����ʿ� ���� ȯ���� �������ֱ� ���� �޼ҵ� */
    public override void OnEpisodeBegin() // �� ���Ǽҵ尡 ���۵ɶ����� ����
    {
        // ������Ʈ�� �ٴ� ������ �������� ��ġ�� �ʱ�ȭ �����ִ� �޼ҵ�
        // �ٴ��� y�� ��ǥ�� 0�̱� ������ ������Ʈ�� y ��ǥ�� 0 �̸��̸� �������ٰ� �Ǵ� ����
        if (this.transform.localPosition.y < 0) // ���� ������Ʈ�� y�� ��ǥ�� 1���� �۾�����,
        {
            this.rBody.angularVelocity = Vector3.zero; // ������Ʈ�� ������ �ӵ� �ʱ�ȭ
            this.rBody.velocity = Vector3.zero;        // ������Ʈ�� �����̴� �ӵ� �ʱ�ȭ
            this.transform.localPosition = new Vector3(0, 1f, 0); // ������Ʈ�� ��ġ �ʱ�ȭ
        }

        // Ÿ�� ������Ʈ�� �����ϰ� �̵������ִ� �޼ҵ�
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f, Random.value * 8 - 4); // Ÿ���� ��ǥ�� �����ϰ� ����
    }

    /* ������Ʈ�� ȯ�濡 ���� �ൿ ������ ������ �޼ҵ� */
    public override void CollectObservations(VectorSensor sensor)
    {
        // ������ �̿��Ͽ� Ÿ�ٰ� ������Ʈ�� ��ġ�� �ľ�
        sensor.AddObservation(Target.localPosition); // Ÿ���� ��ġ�� �ľ�
        sensor.AddObservation(this.transform.localPosition); // ������Ʈ�� ��ġ�� �ľ�

        // ������ �̿��Ͽ� ������Ʈ�� �ӵ��� �ľ�
        sensor.AddObservation(rBody.velocity.x); // ������Ʈ�� x�� �̵� �ӵ� �ľ�
        sensor.AddObservation(rBody.velocity.z); // ������Ʈ�� z�� �̵� �ӵ� �ľ�
    }

    public float forceMultiplier = 10; // ������Ʈ�� �̵��� �� �޴� ���� ����. �������� ������ �����δ�.

    /* ������Ʈ�� �׼ǿ� ���� ������ �����ϴ� �޼ҵ�
	���� ȹ�� ����, ���Ǽҵ带 �����ϴ� �б⸦ ���� ���� */
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero; // ������Ʈ�� �����̴� ��ȣ����, 3�����̹Ƿ� Vector3 ���
        controlSignal.x = actionBuffers.ContinuousActions[0]; // x������ �̵��ϴ� ��ȣ
        controlSignal.z = actionBuffers.ContinuousActions[1]; // z������ �̵��ϴ� ��ȣ
        rBody.AddForce(controlSignal * forceMultiplier); // controlSignal�� ������� ��, ���� ���Ͽ� �̵��ϰ� ����

        // Ÿ�ٰ��� �Ÿ��� �����ϴ� ����, ������Ʈ, Ÿ���� ��ǥ�� �޾ƿͼ� �Ÿ��� ����Ͽ� ����.
        // �̶�, �Ÿ��� �� ������Ʈ�� �߽����κ����� �Ÿ��̴�.
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // ���� ����

        // Ÿ�ٰ��� �Ÿ��� 1.42f �̸��� ��� ��Ҵٰ� �Ǵ�,
        // �̶�, 1.42f�� (������Ʈ ������Ʈ�� ������) + (Ÿ�� ������Ʈ�� ���� ���� / 2)
        // ��, ������ ǥ���� ����� ����� �� �߽����κ����� �ִ�Ÿ�
        if (distanceToTarget < 1.42f) // Ÿ���� ����� ���,
        {
            SetReward(1.0f); // ������ 1�� ��´�.
            coinCounter += 1; // ������ ������ ������ +1 �Ѵ�.
            SetCountText();

            EndEpisode(); // ���� ���Ǽҵ带 ������.
        }

        // ������Ʈ�� y ��ǥ�� 0 �̸��� ��� �ٴڿ��� �������ٰ� �Ǵ�,
        // �̶�, 0�� �ٴ� ������Ʈ�� y ��ǥ
        // ��, ������Ʈ�� y ��ǥ�� 0 �̸��̶�� �ٴ� ������Ʈ���� �ؿ� �ִٴ� �ǹ��̹Ƿ� ������ ��.
        else if (this.transform.localPosition.y < 0) // ������Ʈ�� �ٴ� ������ �������� ���,
        {
            SetReward(-3.0f); // ������ 3�� �Ҵ´�.
            EndEpisode(); // ���� ���Ǽҵ带 ������.
        }
    }

    void SetCountText()
    {
        singleScore.text = "Coins: " + coinCounter.ToString();
    }
}
