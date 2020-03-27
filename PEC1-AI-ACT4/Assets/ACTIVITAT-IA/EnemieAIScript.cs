using UnityEngine;
using UnityEngine.AI;

public class EnemieAIScript : MonoBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.

    public float searchingArea, foundArea, shootingArea, shootingTimer, shootingForce;

    [HideInInspector] public enum AIStates { Searching, GoingToPlayer, Shooting };
    [HideInInspector] public AIStates state;
    private NavMeshAgent navMesh;
    private GameObject player;
    private bool isMoving;
    private float timer;

    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        StateMachine();
    }

    private void StateMachine()
    {
        CheckDistance();
        switch (state)
        {
            case AIStates.Searching:
                SearchingPlayer();
                break;
            case AIStates.GoingToPlayer:
                GoingToPlayer();
                break;
            case AIStates.Shooting:
                ShootingPlayer();
                break;
            default:
                state = AIStates.Searching;
                break;
        }
    }

    private void SearchingPlayer()
    {
        if (!isMoving)
        {
            Vector3 pos = transform.position;
            Vector3 randomPos = new Vector3(pos.x + Random.Range(-searchingArea, searchingArea), pos.y, pos.z + Random.Range(-searchingArea, searchingArea));
            navMesh.destination = randomPos;
            isMoving = true;
        }
        if (navMesh.pathStatus == NavMeshPathStatus.PathComplete && Mathf.Approximately(navMesh.remainingDistance, 0))
        {
            isMoving = false;
        }
    }

    private void GoingToPlayer()
    {
        navMesh.destination = player.transform.position;
    }

    private void ShootingPlayer()
    {
        transform.LookAt(player.transform);
        timer += Time.deltaTime;
        if (timer <= 0.2f)
        {
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (timer >= shootingTimer)
        {
            Shot();
            timer = 0;
        }
    }

    private void Shot()
    {
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = shootingForce * m_FireTransform.forward;

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    private void CheckDistance()
    {
        if (!player) return;
        var distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= foundArea && distance <= shootingArea)
        {
            ChangeState(AIStates.Shooting);
        }
        else if (distance <= foundArea)
        {
            ChangeState(AIStates.GoingToPlayer);
        }
        else
        {
            ChangeState(AIStates.Searching);
        }
    }

    private void ChangeState(AIStates newState)
    {
        var currentState = state;
        if (currentState == newState) return;

        state = newState;
        if (newState == AIStates.GoingToPlayer)
        {
            navMesh.isStopped = false;
            navMesh.speed *= 2;
        }
        else if (newState == AIStates.Shooting)
        {
            navMesh.speed /= 2;
            navMesh.isStopped = true;
        }
        else if (newState == AIStates.Searching)
        {
            navMesh.speed /= 2;
            navMesh.isStopped = false;
            isMoving = false;
        }
    }
}
