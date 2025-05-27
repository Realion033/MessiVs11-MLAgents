using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;

public class EscapeEnvController : MonoBehaviour
{
    [Serializable]
    public class PlayerInfo
    {
        public BlockAgent Agent;
        [HideInInspector] public Vector3 StartingPos;
        [HideInInspector] public Rigidbody RbAgent;
    }

    public List<PlayerInfo> AgentList = new List<PlayerInfo>();
    public int MaxEnvironmentSteps = 2000;
    public GameObject Ground = null;
    public GameObject Door = null;
    public Transform TrapTr = null;

    //트랩 좌이동방향
    private int trap_dir = 1;
    //스텝을 기록해서 Max~Step을 넘어가면 새로운 에피소드를 시작하려고
    private int resetTimer;
    //그라운드 범위에 대한 변수
    private Bounds areaBounds;

    //그룹별로 리워드를 주기 위해 그룹 설정함.
    private SimpleMultiAgentGroup agentGroup;
    //남아있는 에이전트 수
    public int numberOfRemainPlayers;
    //완전 구석에 생성되지 않고 안쪽에 생성될 수 있도록 여
    private float spawnAreaMarginMultiplier = 0.8f;

    void Start()
    {
        areaBounds = Ground.GetComponent<Collider>().bounds;
        agentGroup = new SimpleMultiAgentGroup();
        foreach (var block in AgentList)
        {
            block.StartingPos = block.Agent.transform.localPosition;
            block.RbAgent = block.Agent.GetComponent<Rigidbody>();
            //그룹에 등
            agentGroup.RegisterAgent(block.Agent);
        }
        numberOfRemainPlayers = AgentList.Count;

        ResetScene();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        resetTimer += 1;
        if (resetTimer >= MaxEnvironmentSteps)
        {
            agentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }

        MoveTrap();
    }

    public void GoalReached()
    {
        agentGroup.AddGroupReward(1f);
        agentGroup.EndGroupEpisode();
        ResetScene();
    }

    public void OpenDoor()
    {
        Door.gameObject.SetActive(false);
    }
    public void CloseDoor()
    {
        Door.gameObject.SetActive(true);
    }

    public void KilledByTrap(BlockAgent agent)
    {
        numberOfRemainPlayers--;
        if (numberOfRemainPlayers == 0)
        {
            agentGroup.EndGroupEpisode();
            ResetScene();
        }
        else
        {
            agent.gameObject.SetActive(false);
            OpenDoor();
        }
    }

    public void MoveTrap()
    {
        if (trap_dir == 1 && TrapTr.localPosition.x >= 12f)
        {
            trap_dir = -1;
        }
        if (trap_dir == -1 && TrapTr.localPosition.x <= -12f)
        {
            trap_dir = 1;
        }
        TrapTr.localPosition = new Vector3(TrapTr.localPosition.x + trap_dir * 0.1f, TrapTr.localPosition.y, TrapTr.localPosition.z);
    }

    private List<Vector2> GetRandomSpawnPos()
    {
        List<Vector2> randPosList = new List<Vector2>();
        //Agent랑 Trap 위치를 랜덤하게 결정하려고 총 4번 포문 돌리기
        // 각각의 거리가 5이상이 될 수 있도록 설정하는 과
        for (int i = 0; i < 4; i++)
        {
            Vector2 randPos = new Vector2();
            while (true)
            {
                randPos = new Vector2(Ground.transform.localPosition.x, Ground.transform.localPosition.z) + new Vector2(
                    UnityEngine.Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier),
                    UnityEngine.Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier));

                bool again = false;
                foreach (Vector2 tmpPos in randPosList)
                {
                    if (Vector2.Distance(tmpPos, randPos) <= 5.0)
                    {
                        again = true;
                        break;
                    }
                }
                if (!again) { break; }
            }
            randPosList.Add(randPos);
        }
        return randPosList;
    }
    private void ResetScene()
    {
        List<Vector2> randPosList = GetRandomSpawnPos();

        TrapTr.localPosition = new Vector3(randPosList[0].x, 0.01f, randPosList[0].y);

        int index = 1;
        foreach (var agent in AgentList)
        {
            var pos = new Vector3(randPosList[index].x, 0.5f, randPosList[index].y);

            agent.Agent.transform.localPosition = pos;

            agent.RbAgent.velocity = Vector3.zero;
            agent.RbAgent.angularVelocity = Vector3.zero;
            agent.Agent.gameObject.SetActive(true);

            agentGroup.RegisterAgent(agent.Agent);
            index++;
        }

        resetTimer = 0;

        numberOfRemainPlayers = AgentList.Count;
        CloseDoor();
    }
}