// Gerencia o spawn do personagem selecionado pelo jogador, aplicando upgrades e configurando a câmera
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Prefabs dos Personagens")]
    public GameObject Valina;
    public GameObject Doro;

    [Header("Configurações")]
    public Transform spawnPoint;
    public Camera mainCamera;

    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject prefabToSpawn = (selectedIndex == 0) ? Valina : Doro;

        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            spawnPoint.position = Vector3.zero;
        }

        GameObject spawnedPlayer = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // Reaplica todos os upgrades já coletados em cenas/áreas anteriores
        // Aponta a câmera pro personagem que acabou de nascer na cena
        IUpgradable upgradable = spawnedPlayer.GetComponent<IUpgradable>();
        if (upgradable != null)
        {
            PlayerUpgrades.ApplyAllTo(upgradable);
        }

        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.target = spawnedPlayer.transform;
            }
        }
    }
}