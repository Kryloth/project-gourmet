using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public AudioClip spawning;
    public AudioClip spawnfinish;
    AudioSource audioSource;
    SpriteRenderer sprite;
    Vector3 idPos;
    Vector3 rotation;
    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        sprite.enabled = false;
    }
    void Start()
    {
        idPos = this.gameObject.transform.position;
        rotation = new Vector3(0f, 0f, 1080f);
        
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Y)){
            StartCoroutine(Spawn(1, 2, 1));
        }
    }

    public IEnumerator Spawn(int min,int max, int round)
    {
        Enemy enemy = null;
        Vector3 spawnPos;
        int spawnCount = Random.Range(min, max + 1);
        sprite.enabled = true;
        LeanTween.rotateLocal(this.gameObject, rotation, 3f);
        MainGameController.instance.currEnemies += spawnCount;
        audioSource.PlayOneShot(spawning, .1f);
        yield return new WaitForSeconds(3f);
        
        for(int i = 0; i < spawnCount; i++)
        {
            spawnPos = new Vector3(idPos.x + Random.Range(-1f, 1f), idPos.y + Random.Range(-1f, 1f), idPos.z);
            GameObject enemySpawn = Instantiate(MainGameController.instance.enemyPrefab, spawnPos, Quaternion.identity);
            enemy = enemySpawn.GetComponent<Enemy>();
            enemy.MAX_HEALTH += round * 5f;
            enemy.baseDamage += round * .5f;
        }
        audioSource.Stop();
        yield return null;
        audioSource.PlayOneShot(spawnfinish, .5f);
        sprite.enabled = false;
    }

}
