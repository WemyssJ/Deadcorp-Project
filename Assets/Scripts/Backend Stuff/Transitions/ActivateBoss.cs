using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBoss : MonoBehaviour
{
    public GameObject _boss;
    public GameObject _player;


    private void Awake()
    {
        _boss.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(_player.GetComponent<Player>().isDead)
        {
            StartCoroutine(Delay(1));
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            
            _boss.GetComponent<Enemy>().CanAttack();
            _boss.GetComponent<Enemy>().standardAttackNo = 1;
            _boss.GetComponent<Enemy>().closeToEnemy = false;
            _boss.SetActive(true);
        }
    }
    IEnumerator Delay(float duration)
    {
        
        yield return new WaitForSeconds(duration);
        _boss.SetActive(false);

    }
}
