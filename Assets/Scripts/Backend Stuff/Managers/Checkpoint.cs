using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    Player _player;
    bool isActive;
    bool wasNeverActivated;
    string currentAnimatorState;
    Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();


        wasNeverActivated = true;
        isActive = false;

    }
    private void Update()
    {
        if (_player.checkpointPos == this.transform.position)
        {
            wasNeverActivated = false;
            isActive = true;
        }
        if (!wasNeverActivated)
        {
            playerIsDead();
            if (isActive)
            {
                ChangeAnimationState("Active");
            }else
                ChangeAnimationState("Inactive");
        }else
        ChangeAnimationState("Off");
    }
    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && InputManager.AttackIsPressed && _player.CurrentCoins >= 1 && !isActive)
        {
            _player.CurrentCoins--;
            SaveSystem.Save();
            GameObject[] AllCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

            foreach(GameObject _checkpoint in AllCheckpoints)
            {
                _checkpoint.gameObject.GetComponent<Checkpoint>().isActive = false;
            }

            _player.UpdateCheckpoint(this.transform.position);
            isActive = true;
            wasNeverActivated = false;

        }
    }

    public void ChangeAnimationState(string newAnimationState)
    {
        animator.Play(newAnimationState);
        currentAnimatorState = newAnimationState;
    }
    public bool playerIsDead()
    {
        if (_player.GetComponent<Player>().isDead)
        {
            GameObject[] AllCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

            foreach (GameObject _checkpoint in AllCheckpoints)
            {
                _checkpoint.gameObject.GetComponent<Checkpoint>().isActive = false;
            }

            isActive = false;
            return true;


        }
        return false;
    }
}
