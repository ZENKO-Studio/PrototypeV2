using UnityEngine;

public class AudioManager : Singelton<AudioManager>
{
   [SerializeField] private AudioSource[] soundFx;
   
   [SerializeField] private AudioSource bgm, levelEndMusic;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySfx(int soundToPlay)
    {
        soundFx[soundToPlay].Stop();

        soundFx[soundToPlay].pitch = Random.Range(.9f, 1.3f);

        soundFx[soundToPlay].Play();

    }
}
