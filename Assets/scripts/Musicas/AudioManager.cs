using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fontes de Áudio")]
    public AudioSource musicSource;   // pra música de fundo
    public AudioSource sfxSource;     // pra efeitos pontuais
    [Header("Ambientação")]
    public AudioClip backgroundAmbience;

    [Header("Efeitos do Player")]
    public AudioClip playerHitClip;  // player sendo atingido
    public AudioClip playerAttackClip; // player atacando
    public AudioClip playerDefeatClip; // player derrotado

    [Header("Efeitos do Inimigo")]
    public AudioClip enemyHitClip; // inimigo sendo atingido
    public AudioClip enemyDeathClip; // inimigo derrotado

    [Header("Esquiva e Ataques Especiais")]
    public AudioClip dodgeClip;  // Esquiva (Valina e Doro)
    public AudioClip doroSpecialAttackClip;  // Ataque Especial da Doro (a flecha)
    public AudioClip valinaDashAttackClip;  // Ataque Especial da Valina (o dash)

    [Header("Chefe")]
    public AudioClip bossAttackClip;  // Ataque do chefe (melee ou à distância)

    [Header("Fim de jogo")]
    public AudioClip victoryClip;  // Vitória do jogador

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // sobrevive entre troca de cenas
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