using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericEnemyAI : GroundEnemy 
{  
    public Transform[] patrolpoints;
	int destPoint = 0;

    public int idleAnimations;
	public float patrolTime = 10f;
	public float idleTime = 5f;
    public float stunTime = 5f;

	enum EnemyState 
    {
		_Idle,
		_Patrolling,
		_SpottedPlayer,
		_ChasingPlayer,
        _Stunned,
        _Taunting,
		_Dead
	}

	EnemyState currentState;

	public bool isDoingSomething = false;
	public bool playerSpotted = false;
	public bool allowWait;
	public bool waiting;
    public bool currentlyStunned;

	public bool playerInVisionRange;
    public bool currentlyDying = false;

	public float chaseSpeed = 6.5f;
	public float patrolSpeed = 3.0f;
    public float outOfVisionTime = 2.0f;

    public bool isAlwaysIdle;
    public bool canChasePlayer;

    DamageTrigger dTrigger;
    BounceTrigger bTrigger;

	void Start () 
    {
		allowWait = true;
		agent = GetComponent<NavMeshAgent> ();
		GetNextDestination ();
		currentState = EnemyState._Patrolling;
        player = GameObject.FindObjectOfType<CharacterControl>();
		anim = GetComponent<Animator>();
        dTrigger = GetComponentInChildren<DamageTrigger>();
        bTrigger = GetComponentInChildren<BounceTrigger>();
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad7))
        {
            currentState = EnemyState._Dead;
        }
    }

	void FixedUpdate() 
    {
        if (bTrigger.bouncedOn)
        {
            EnemyStun();
            bTrigger.bouncedOn = false;
        }

		switch (currentState)
		{
		case EnemyState._Idle:
			Idle();
			break;
		case EnemyState._Patrolling:
			Patrol ();
			break;
		case EnemyState._SpottedPlayer:
			SpottedPlayer ();
			break;
		case EnemyState._ChasingPlayer:
			ChasingPlayer ();
			break;
        case EnemyState._Stunned:
            Stunned();
            break;
        case EnemyState._Taunting:
            TauntingPlayer();
            break;
		case EnemyState._Dead:
			Squish ();
			break;
		default:
			Idle();
			break;
		}
	}

	void GetNextDestination()
    {
        if (patrolpoints.Length == 0)
        {
            agent.SetDestination(RandomNavmeshLocation(Random.Range(2, 15)));
            return;
        }
        else
        {
            agent.destination = patrolpoints[destPoint].position;
        }

		destPoint = (destPoint + 1) % patrolpoints.Length;
	}

	void Idle()
	{
        anim.SetBool("isChasing", false);
        anim.SetBool("isMoving", false);

        if (playerSpotted && canChasePlayer)
        {
            currentState = EnemyState._SpottedPlayer;
            return;
        }

        if (dTrigger.damagedPlayer == true)
        {
            currentState = EnemyState._Taunting;
            return;
        }

        if (isAlwaysIdle)
        {
            agent.isStopped = true;
            return;
        }

        if (allowWait)
        {
            allowWait = false;
            isDoingSomething = true;

            anim.SetInteger("idleAlt", Random.Range(0, idleAnimations + 1));
            StartCoroutine(WaitTime(idleTime));
        }
            
        if (isDoingSomething)
        {
            agent.isStopped = true;
        }

        if (!isDoingSomething)
        {
            currentState = EnemyState._Patrolling;
        }
        
        if (currentlyStunned == true)
        {
            currentState = EnemyState._Stunned; 
        }
	}

	void Patrol()
	{
        agent.enabled = true;

        ReturnToDefaultIdle();
        anim.SetBool("isChasing", false);
        anim.SetBool("isMoving", true);

        if (isAlwaysIdle)
        {
            currentState = EnemyState._Idle;
        }
		agent.isStopped = false;
		agent.speed = patrolSpeed;

		if (allowWait)
		{
			allowWait = false;
            isDoingSomething = true;
			StartCoroutine (WaitTime (patrolTime));
		}


		if (isDoingSomething) 
        {		
			if (!agent.pathPending && agent.remainingDistance < 1f) 
            {
				GetNextDestination ();
			}
		}
        else 
        {
			currentState = EnemyState._Idle;
		}

        if (playerSpotted && canChasePlayer) 
        {
			currentState = EnemyState._SpottedPlayer;
		}

        if (currentlyStunned == true)
        {
            currentState = EnemyState._Stunned; 
        }

        if (dTrigger.damagedPlayer == true)
        {
            currentState = EnemyState._Taunting;
            return;
        }
	}

    Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) 
        {
            finalPosition = hit.position;            
        }

        return finalPosition;
    }

	void SpottedPlayer()
	{	
		agent.isStopped = true;
        anim.Play("ChargeUp");

        if (currentlyStunned == true)
        {
            currentState = EnemyState._Stunned; 
        }

        if (dTrigger.damagedPlayer == true)
        {
            currentState = EnemyState._Taunting;
            return;
        }
	}

    // Called from an animation event on the attack wind-up
    public void BeginChase()
    {
        anim.SetBool("isChasing", true);
        StopAllCoroutines();
        currentState = EnemyState._ChasingPlayer;
    }

	void ChasingPlayer()
    {
        
		agent.isStopped = false;

		agent.destination = player.transform.position;
		agent.speed = chaseSpeed;

        if (currentlyStunned == true)
        {
            currentState = EnemyState._Stunned; 
        }

        if (playerSpotted == false)
        {
            if (isAlwaysIdle)
            {
                currentState = EnemyState._Idle;
            } else {
                currentState = EnemyState._Patrolling;
             
            }
        }

        if (dTrigger.damagedPlayer == true)
        {
            currentState = EnemyState._Taunting;
            return;
        }
	}

    void Stunned()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
            isDoingSomething = true;
            StartCoroutine(WaitTime(stunTime));
            dTrigger.GetComponent<BoxCollider>().enabled = false;

            anim.SetBool("isStunned", true);
            anim.Play("Stunned");
        }

        if (!isDoingSomething)
        {
            dTrigger.GetComponent<BoxCollider>().enabled = true;
            currentlyStunned = false;
            currentState = EnemyState._Patrolling;
            anim.SetBool("isStunned", false);
        }

        if (currentlyDying)
        {
            // go to dying state
            currentlyDying = false;
            Squish();

        }
    }

    // Has damaged the player 
    void TauntingPlayer()
    {
        if (currentlyStunned == true)
        {
            currentState = EnemyState._Stunned; 
        }
            
        if (agent.enabled)
        {
            if (!agent.isStopped)
            {
                anim.Play("Taunting");
                agent.isStopped = true;
                isDoingSomething = true;
                StartCoroutine(WaitTime(2f));
                agent.enabled = false;
            }
        }
       
        if (!isDoingSomething) 
        {
            agent.enabled = true;
            currentState = EnemyState._Patrolling;
        }
    }
 
    public void EnemyDead()
    {
        currentlyDying = true;
    }

    void EnemyStun()
    {
        currentlyStunned = true;
    }

	void Squish()
	{
        anim.Play("Squish");
        dTrigger.enabled = false;
        agent.isStopped = true;
	}

    public void IsDead()
    {
        print("Squished!");
        gameObject.SetActive(false);
        GameManager.instance.DropChi(transform.position, chiToDrop, false);
        dTrigger.enabled = true;
        agent.isStopped = false;
        currentState = EnemyState._Patrolling;
    }

	IEnumerator WaitTime(float seconds)
	{
		yield return new WaitForSeconds (seconds * 0.75f);

        // If the enemy is in an alternate idle, return it to its original idle
        if (currentState == EnemyState._Idle)
        {
            print("Returning to default idle");
            ReturnToDefaultIdle();
        }
        yield return new WaitForSeconds(seconds * 0.25f);

		isDoingSomething = false;
		yield return new WaitForFixedUpdate();
		allowWait = true;
	}

	void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
            print("Player in radius");
			Vector3 targetDir = other.transform.position - transform.position;
			float angle = Vector3.Angle(targetDir, transform.forward);

			if (angle < 30)
			{
                print("Spotted player");
				playerSpotted = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
        for (int i = 0; i < 10 * Time.deltaTime; i++)
        {
            playerSpotted = true;
        }

        playerSpotted = false;
	}

//    IEnumerator PlayerOutOfVision(float seconds)
//    {
//        yield return new WaitForSeconds(seconds);
//        playerSpotted = false;
//        yield return null;
//    }	

    // Called from certain Idle Animation Varients to return back to the original idle
    void ReturnToDefaultIdle()
    {
        anim.SetInteger("idleAlt", 0);
    }
}
