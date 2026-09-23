using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gera proceduralmente um cenário de fundo estilo "ruínas de castelo" (colunas, arcos,
/// muros com musgo, banners, tochas, plantas) usando os sprites cortados de um sprite sheet
/// tipo o "tileset.png" (colunas / arcos / muros / decorações).
///
/// COMO USAR:
/// 1. Crie um GameObject vazio na cena, nomeie "RuinsBackground".
/// 2. Adicione este componente (Add Component -> Ruins Background Builder).
/// 3. No seu tileset.png (Sprite Mode = Multiple, já cortado), selecione no Sprite Editor
///    os sprites de cada categoria e arraste-os nos arrays correspondentes no Inspector:
///      - Pillar Sprites   -> colunas verticais (ex: sprites de pilar liso)
///      - Arch Sprites     -> arcos / portais
///      - Wall Ledge Sprites -> blocos de muro com musgo no topo (usados como "chão"/plataforma)
///      - Banner Sprites   -> bandeiras
///      - Torch Sprites    -> tochas / braseiros
///      - Plant Sprites    -> arbustos, árvores, vinhas
///      - Prop Sprites     -> caixotes, barris, estátuas, cruzes etc (decoração aleatória no chão)
/// 4. Ajuste Total Width, Ground Y e as demais opções.
/// 5. Clique com o botão direito no componente (ou no menu "⋮" do Inspector) e escolha
///    "Generate Background" para montar a cena. Escolha "Clear Background" para limpar e refazer.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public class RuinsBackgroundBuilder : MonoBehaviour
{
    [Header("Sprites por categoria (arraste os slices do tileset.png)")]
    public Sprite[] pillarSprites;
    public Sprite[] archSprites;
    public Sprite[] wallLedgeSprites;
    public Sprite[] bannerSprites;
    public Sprite[] torchSprites;
    public Sprite[] plantSprites;
    public Sprite[] propSprites;

    [Header("Layout")]
    [Tooltip("Largura total do cenário gerado, em unidades do mundo.")]
    public float totalWidth = 40f;
    [Tooltip("Posição Y da linha de base (chão) onde colunas/arcos são apoiados.")]
    public float groundY = 0f;
    [Tooltip("Espaço mínimo/máximo entre uma coluna/arco e o próximo.")]
    public Vector2 spacingRange = new Vector2(0.2f, 1.2f);
    [Tooltip("Chance (0-1) de um arco aparecer no lugar de uma coluna comum.")]
    [Range(0f, 1f)] public float archChance = 0.25f;

    [Header("Muro / chão")]
    [Tooltip("Gera uma faixa de blocos de muro (wallLedgeSprites) cobrindo toda a largura, no nível groundY.")]
    public bool buildGroundStrip = true;
    public float groundStripYOffset = -0.05f;

    [Header("Decorações")]
    [Range(0f, 1f)] public float bannerChance = 0.35f;
    [Range(0f, 1f)] public float torchChance = 0.25f;
    [Range(0f, 1f)] public float plantChance = 0.4f;
    [Range(0f, 1f)] public float propChance = 0.3f;
    [Tooltip("Deslocamento vertical aplicado a decorações penduradas (banners/tochas) em relação ao topo da coluna/arco.")]
    public float decorationYOffset = -0.1f;

    [Header("Render")]
    public string sortingLayerName = "Default";
    public int baseSortingOrder = 0;
    [Tooltip("Unidade de referência: quantos pixels do sprite equivalem a 1 unidade do mundo (deve bater com o Pixels Per Unit do import).")]
    public float pixelsPerUnit = 32f;

    [Header("Aleatoriedade")]
    public int randomSeed = 12345;
    public bool useRandomSeed = true;

    const string kContainerName = "__Generated__";

    [ContextMenu("Generate Background")]
    public void GenerateBackground()
    {
        ClearBackground();

        if (useRandomSeed)
            Random.InitState(randomSeed);

        Transform container = GetOrCreateContainer();

        float halfWidth = totalWidth * 0.5f;
        float x = -halfWidth;
        int order = baseSortingOrder;

        // 1) Linha de base: muro/plataforma cobrindo toda a largura
        if (buildGroundStrip && wallLedgeSprites != null && wallLedgeSprites.Length > 0)
        {
            float gx = -halfWidth;
            while (gx < halfWidth)
            {
                Sprite s = RandomFrom(wallLedgeSprites);
                if (s == null) break;
                float w = s.rect.width / pixelsPerUnit;
                var go = SpawnSprite(s, new Vector3(gx + w * 0.5f, groundY + groundStripYOffset, 0f),
                    container, "GroundStrip", order);
                gx += w;
            }
            order++;
        }

        // 2) Colunas / arcos ao longo da largura, com decorações no topo
        while (x < halfWidth)
        {
            bool useArch = archSprites != null && archSprites.Length > 0 && Random.value < archChance;
            Sprite mainSprite = useArch ? RandomFrom(archSprites) : RandomFrom(pillarSprites);

            if (mainSprite == null)
            {
                // fallback: se não houver nem coluna nem arco configurados, evita loop infinito
                x += 1f;
                continue;
            }

            float w = mainSprite.rect.width / pixelsPerUnit;
            float h = mainSprite.rect.height / pixelsPerUnit;
            Vector3 basePos = new Vector3(x + w * 0.5f, groundY + h * 0.5f, 0f);

            var mainGO = SpawnSprite(mainSprite, basePos, container,
                useArch ? "Arch" : "Pillar", order);

            // decoração pendurada no topo (banner / tocha)
            float topY = groundY + h + decorationYOffset;
            if (bannerSprites != null && bannerSprites.Length > 0 && Random.value < bannerChance)
            {
                Sprite b = RandomFrom(bannerSprites);
                SpawnSprite(b, new Vector3(basePos.x, topY, 0f), container, "Banner", order + 1);
            }
            else if (torchSprites != null && torchSprites.Length > 0 && Random.value < torchChance)
            {
                Sprite t = RandomFrom(torchSprites);
                SpawnSprite(t, new Vector3(basePos.x, topY, 0f), container, "Torch", order + 1);
            }

            // planta ou prop aleatório na base, ao lado
            if (plantSprites != null && plantSprites.Length > 0 && Random.value < plantChance)
            {
                Sprite p = RandomFrom(plantSprites);
                float ph = p.rect.height / pixelsPerUnit;
                SpawnSprite(p, new Vector3(x - 0.2f, groundY + ph * 0.5f, 0f), container, "Plant", order - 1);
            }
            if (propSprites != null && propSprites.Length > 0 && Random.value < propChance)
            {
                Sprite pr = RandomFrom(propSprites);
                float prh = pr.rect.height / pixelsPerUnit;
                SpawnSprite(pr, new Vector3(x + w + 0.3f, groundY + prh * 0.5f, 0f), container, "Prop", order - 1);
            }

            x += w + Random.Range(spacingRange.x, spacingRange.y);
        }
    }

    [ContextMenu("Clear Background")]
    public void ClearBackground()
    {
        Transform existing = transform.Find(kContainerName);
        if (existing != null)
        {
            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
        }
    }

    Transform GetOrCreateContainer()
    {
        var go = new GameObject(kContainerName);
        go.transform.SetParent(transform, false);
        return go.transform;
    }

    GameObject SpawnSprite(Sprite sprite, Vector3 localPos, Transform parent, string label, int order)
    {
        var go = new GameObject(label + "_" + sprite.name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = order;
        return go;
    }

    Sprite RandomFrom(IList<Sprite> arr)
    {
        if (arr == null || arr.Count == 0) return null;
        return arr[Random.Range(0, arr.Count)];
    }
}
