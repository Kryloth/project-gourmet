using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    CircleCollider2D circleCollider;
    AudioSource audioSource;
    public AudioClip audioClip;
    public Foogic foogic;
    public float spellDuration;
    public float delay;
    public bool isDelay;
    float volume;
    bool isPlay = false;
    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        circleCollider = GetComponent<CircleCollider2D>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDelay && delay > 0){
            delay -= Time.deltaTime;
        }
        if(!isDelay && spellDuration > 0){
            if(audioSource.isPlaying && !isPlay){
                audioSource.Stop();
                audioSource.PlayOneShot(audioClip, volume);
                isPlay = true;
            }else if (!audioSource.isPlaying && !isPlay){
                audioSource.PlayOneShot(audioClip, volume);
                isPlay = true;
            }
            spellDuration -= Time.deltaTime;
        }
        if(!isDelay && spellDuration <= 0){
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            Color32 alpha = new Color32(255, 255, 255, 0);
            if(circleCollider != null)
                circleCollider.enabled = false;
            SetDestroy();
        }
        if(delay <= 0){
            isDelay = false;
            if(circleCollider != null)
                circleCollider.enabled = true;
        }
    }
    public void SetDelay(float spawnDelay){
        isDelay = true;
        delay = spawnDelay;
        if(circleCollider != null)
            circleCollider.enabled = false;
    }
    public void SetRadius(float radius){
        if(circleCollider != null)
            circleCollider.radius = radius;
    }
    public void SetFoogic(Foogic Sfoogic){
        foogic = Sfoogic;
    }
    public void SetAudio(AudioClip clip, float vol){
        audioClip = clip;
        volume = vol;
    }
    public void isMeteor(AudioClip audio){
        GameObject meteor = this.transform.GetChild(0).gameObject;
        meteor.transform.localPosition = new Vector3(0f, 12f, 0f);
        LeanTween.moveLocalY(meteor, 0f, .8f);
        audioSource.PlayOneShot(audio, .3f);
    }
    void SetDestroy(){
        Destroy(this.gameObject);
    }
}
