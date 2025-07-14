using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzleTest : MonoBehaviour
{
    [Header("Board Settings")]
    [Range(2, 10)] public int columns = 3;  // how many across
    [Range(2, 10)] public int rows    = 2;  // how many down
    public float gapThickness = 0.01f;

    [Header("Preview Settings")]
    public float previewDuration = 2f;
    public List<Texture2D> imageTextures;

    [Header("Prefabs & Transforms")]
    public Transform   gameTransform;
    public Transform   piecePrefab;
    public GameObject  previewQuadPrefab;

    [Header("Hookup")]
    public SlidingPuzzleUIManager uiManager;

    private List<Transform> pieces;
    private int emptyLocation;         // where the blank currently is
    private int originalEmptyIndex;    // always the bottom-right slot
    private Texture2D chosenTexture;

    void Start()
    {
        // 1) record which index we want to remain the “empty” slot forever
        originalEmptyIndex = columns * rows - 1;

        pieces = new List<Transform>();

        if (imageTextures == null || imageTextures.Count == 0)
        {
            Debug.LogError("No images assigned!");
            return;
        }
        chosenTexture = imageTextures[Random.Range(0, imageTextures.Count)];
        StartCoroutine(ShowPreviewAndStart());
    }

    IEnumerator ShowPreviewAndStart()
    {
        var quad = Instantiate(previewQuadPrefab);
        var rend = quad.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Unlit/Texture"));
        rend.material.mainTexture = chosenTexture;

        var cam = Camera.main;
        quad.transform.position = cam.transform.position + cam.transform.forward * 2f;
        quad.transform.rotation = cam.transform.rotation;

        float halfFOV = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float height  = 2f * Mathf.Tan(halfFOV) * 2f;
        float width   = height * cam.aspect;
        quad.transform.localScale = new Vector3(width, height, 1f);

        yield return new WaitForSeconds(previewDuration);
        Destroy(quad);

        CreateGamePieces(gapThickness);
        Shuffle();
    }

private void CreateGamePieces(float gap)
{
    float tileW = 2f / columns;
    float tileH = 2f / rows;

    for (int r = 0; r < rows; r++)
    {
        for (int c = 0; c < columns; c++)
        {
            // 1) invert r so that r=0 is *bottom* row for naming & UVs
            int invR = (rows - 1) - r;
            int correctIdx = invR * columns + c;

            // instantiate
            var piece = Instantiate(piecePrefab, gameTransform);
            pieces.Add(piece);

            // position & scale (we still build top-down in world space)
            float x = -1 + tileW * c + tileW / 2;
            float y = +1 - tileH * r - tileH / 2;
            piece.localPosition = new Vector3(x, y, 0);
            piece.localScale    = new Vector3(tileW - gap, tileH - gap, 1);

            // name it so completion check works
            piece.name = correctIdx.ToString();

            // permanent blank slot is the *bottom-right* of the image
            if (correctIdx == originalEmptyIndex)
            {
                emptyLocation = pieces.Count - 1;
                piece.gameObject.SetActive(false);
                continue;
            }

            piece.gameObject.AddComponent<BoxCollider2D>();

            // slice UV’s so bottom-left of image is (0,0)
            float u0 = (float)c       / columns;
            float u1 = (float)(c + 1) / columns;
            float v0 = (float)invR    / rows;
            float v1 = (float)(invR+1) / rows;

            var mesh = piece.GetComponent<MeshFilter>().mesh;
            mesh.uv = new Vector2[]
            {
                new Vector2(u0, v0), // bottom-left
                new Vector2(u1, v0), // bottom-right
                new Vector2(u0, v1), // top-left
                new Vector2(u1, v1), // top-right
            };

            var mat = new Material(Shader.Find("Unlit/Texture"));
            mat.mainTexture = chosenTexture;
            piece.GetComponent<MeshRenderer>().material = mat;
        }
    }
}

    void Shuffle()
    {
        int shuffleCount = columns * rows * columns * rows;
        int last = emptyLocation;
        var rnd = new System.Random();

        for (int i = 0; i < shuffleCount; i++)
        {
            int candidate = rnd.Next(0, columns * rows);
            if (candidate == last) continue;
            last = emptyLocation;

            if (TrySwap(candidate, -columns, columns)) continue;
            if (TrySwap(candidate, +columns, columns)) continue;
            if (TrySwap(candidate, -1, 0))            continue;
            if (TrySwap(candidate, +1, columns - 1))  continue;
        }
    }

    bool TrySwap(int i, int offset, int colCheck)
    {
        if ((i % columns) != colCheck && i + offset == emptyLocation)
        {
            // swap tiles in our list
            var a = pieces[i];
            pieces[i] = pieces[i + offset];
            pieces[i + offset] = a;

            // swap their positions
            var pa = a.localPosition;
            a.localPosition = pieces[i].localPosition;
            pieces[i].localPosition = pa;

            emptyLocation = i;
            return true;
        }
        return false;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0;
        var hit = Physics2D.Raycast(world, Vector2.zero);
        if (hit.collider == null) return;

        int idx = pieces.FindIndex(t => t == hit.transform);
        if (idx < 0) return;

        // if we made a legal move, check for completion
        if (TrySwap(idx, -columns, columns) ||
            TrySwap(idx, +columns, columns) ||
            TrySwap(idx, -1, 0)            ||
            TrySwap(idx, +1, columns - 1))
        {
            if (CheckCompletion())
                uiManager.EndPuzzle(true);
        }
    }

    bool CheckCompletion()
    {
        // world‐slot  i  has row = i/columns, col = i%columns
        // the inverted image wants invR = (rows-1) - row
        for (int i = 0; i < pieces.Count; i++)
        {
            // skip the blank slot
            if (i == emptyLocation) continue;

            int row = i / columns;
            int col = i % columns;
            int invR = (rows - 1) - row;
            int desiredSliceIndex = invR * columns + col;

            // piece.name was set to the sliceIndex it *is*
            if (pieces[i].name != desiredSliceIndex.ToString())
                return false;
        }
        return true;
    }

}
