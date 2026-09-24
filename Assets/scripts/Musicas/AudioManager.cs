using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fontes de Áudio")]
    public AudioSource musicSource;   // pra ambientação (loop contínuo)
    public AudioSource sfxSource;     // pra efeitos pontuais (não precisa ser loop)

    [Header("Ambientação")]
    public AudioClip backgroundAmbience;

    [Header("Efeitos do Player")]
    public AudioClip playerHitClip;
    public AudioClip playerAttackClip;
    public AudioClip playerDefeatClip;

    [Header("Efeitos do Inimigo")]
    public AudioClip enemyHitClip;
    public AudioClip enemyDeathClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // sobrevive entre troca de cenas, se precisar
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayAmbience();
    }

    public void PlayAmbience()
    {
        if (musicSource != null && backgroundAmbience != null)
        {
            musicSource.clip = backgroundAmbience;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}