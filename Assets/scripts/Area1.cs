
using UnityEngine;
using UnityEngine.Tilemaps;

public class Area1 : MonoBehaviour
{
    [Header("Tilemap do mapa")]
    public Tilemap tilemap;

    [Header("Tiles de pedra")]
    public TileBase tileChao;
    public TileBase tileParede;
    public TileBase tilePlataforma;

    [Header("Tiles de estruturas")]
    public TileBase tileColuna;
    public TileBase tileTopoColuna;

    [Header("Configurações")]
    public bool gerarAoIniciar = false;

    private void Start()
    {
        if (gerarAoIniciar)
        {
            GerarMapa();
        }
    }

    [ContextMenu("Gerar Mapa")]
    public void GerarMapa()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap não foi configurado!");
            return;
        }

        LimparMapa();

        // CHÃO PRINCIPAL
        CriarLinha(0, 0, 127, tileChao);

        // PLATAFORMAS E RUÍNAS
        CriarLinha(8, 5, 20, tilePlataforma);
        CriarLinha(30, 8, 42, tilePlataforma);
        CriarLinha(52, 5, 65, tilePlataforma);
        CriarLinha(75, 9, 88, tilePlataforma);
        CriarLinha(98, 6, 112, tilePlataforma);

        // COLUNAS
        CriarColuna(12, 1, 5);
        CriarColuna(35, 1, 8);
        CriarColuna(57, 1, 5);
        CriarColuna(80, 1, 9);
        CriarColuna(104, 1, 6);

        // PAREDES LATERAIS
        CriarParede(0, 1, 0, 8);
        CriarParede(127, 1, 127, 8);

        Debug.Log("Mapa de ruínas gerado!");
    }

    [ContextMenu("Limpar Mapa")]
    public void LimparMapa()
    {
        if (tilemap != null)
        {
            tilemap.ClearAllTiles();
        }
    }

    private void CriarLinha(
        int xInicial,
        int y,
        int xFinal,
        TileBase tile)
    {
        if (tile == null) return;

        for (int x = xInicial; x <= xFinal; x++)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }
    }

    private void CriarParede(
        int xInicial,
        int yInicial,
        int xFinal,
        int yFinal)
    {
        if (tileParede == null) return;

        for (int x = xInicial; x <= xFinal; x++)
        {
            for (int y = yInicial; y <= yFinal; y++)
            {
                tilemap.SetTile(
                    new Vector3Int(x, y, 0),
                    tileParede
                );
            }
        }
    }

    private void CriarColuna(
        int x,
        int yInicial,
        int altura)
    {
        if (tileColuna == null) return;

        for (int y = yInicial; y <= altura; y++)
        {
            TileBase tileAtual = tileColuna;

            if (y == altura && tileTopoColuna != null)
            {
                tileAtual = tileTopoColuna;
            }

            tilemap.SetTile(
                new Vector3Int(x, y, 0),
                tileAtual
            );
        }
    }
}