using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BlockAgent : Agent
{
    private Rigidbody agentRb;
    private EscapeEnvController controller;
    private float runSpeed = 2.5f;

    public float DecisionWaitingTime = 0.01f;
    float currentTime = 0f;

    public override void Initialize()
    {
        controller = GetComponentInParent<EscapeEnvController>();
        agentRb = GetComponent<Rigidbody>();

        Academy.Instance.AgentPreStep += WaitTimeinference;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(controller.numberOfRemainPlayers);
        sensor.AddObservation(agentRb.velocity);
    }

    private void MoveAgnet(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                dirToGo = transform.right * -1f;
                break;
            case 4:
                dirToGo = transform.right * 1f;
                break;

        }
        agentRb.AddForce(dirToGo * runSpeed, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgnet(actions.DiscreteActions);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            controller.GoalReached();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trap"))
        {
            controller.KilledByTrap(this);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 4;
        }
    }

    public void WaitTimeinference(int action)
    {
        // mlagents-learn과 연결되어 있는지 확인하는 과정임.
        if (Academy.Instance.IsCommunicatorOn)
        {
            RequestDecision();
        }
        else
        {
            if (currentTime >= DecisionWaitingTime)
            {
                currentTime = 0;
                RequestDecision();
            }
            else
            {
                currentTime += Time.fixedDeltaTime;
            }
        }
    }
}