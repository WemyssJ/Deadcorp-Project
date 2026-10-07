using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GhostTrail : MonoBehaviour
{
    [SerializeField] private GameObject GhostPreFab;
    [SerializeField] private int _numberOfTrails = 3;
    [SerializeField] private float _fadeTime = 1f;

    private GameObject _ghosts;

    private int _matAlpha = Shader.PropertyToID("_GhostAlpha");

    private void Awake()
    {
    }

    public void LeaveGhostTrail(float time)
    {

        if (GetComponent<PlayerUnlocks>().AirDashUnlocked == true || GetComponent<PlayerUnlocks>().DashUnlocked == true)
            {

            StartCoroutine(LeaveTrail(time));
            if (this.transform.rotation.y == 0)
            {
                GhostPreFab.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (this.transform.rotation.y == 1)
            {
                GhostPreFab.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

    }

    private IEnumerator LeaveTrail(float time)
    {
        int numberSpawned = 0;
        while (numberSpawned < _numberOfTrails)
        {
            for (int i = 0; i < _numberOfTrails; i++)
            {
                Spawn();
                yield return new WaitForSeconds(time / _numberOfTrails);
                numberSpawned++;
            }
            
        }
    }

    private void Spawn()
    {


        GameObject go = Instantiate(GhostPreFab, transform.position, Quaternion.identity);
        SpriteRenderer[] rends = go.GetComponentsInChildren<SpriteRenderer>();
        go.SetActive(true);

        StartCoroutine(FadeGhost(rends, go));
    }

    private IEnumerator FadeGhost(SpriteRenderer[] rends, GameObject go)
    {
        float elapsedTime = 0f;
        while(elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < rends.Length; i++)
            {
                float newValue = Mathf.Lerp(1f, 0f, (elapsedTime / _fadeTime));
                rends[i].material.SetFloat(_matAlpha, newValue);
            }

            yield return null;
        }

        Destroy(go);
    }
}
