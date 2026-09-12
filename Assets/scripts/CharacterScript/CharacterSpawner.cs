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
        // Pega a escolha feita na tela de seleção de personagem
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject prefabToSpawn = (selectedIndex == 0) ? Valina : Doro;

        // Fallback caso nao tenha arrastado um spawn point no Inspector
        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            spawnPoint.position = Vector3.zero;
        }

        GameObject spawnedPlayer = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // Aponta a câmera pro personagem que acabou de nascer na cena
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