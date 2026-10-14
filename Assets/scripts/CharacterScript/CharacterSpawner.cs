// Gerencia o spawn do personagem selecionado pelo jogador, aplicando upgrades e configurando a câmera
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Prefabs dos Personagens")]
    public GameObject Valina;
    public GameObject Doro;
    public GameObject DoroRonin;

    [Header("Configurações")]
    public Transform spawnPoint;
    public Camera mainCamera;

    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject prefabToSpawn;

        switch (selectedIndex)
        {
            case 1: prefabToSpawn = Doro; break;
            case 2: prefabToSpawn = DoroRonin; break;
            default: prefabToSpawn = Valina; break;
        }

        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            spawnPoint.position = Vector3.zero;
        }

        GameObject spawnedPlayer = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

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