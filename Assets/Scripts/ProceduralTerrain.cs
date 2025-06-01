using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    // ���� ũ�� ����
    [Range(10, 1000)] public int width = 100;
    [Range(10, 1000)] public int height = 100;

    // ������ ���ø� ������ �� ���� ����
    [Range(1f, 200f)] public float scale = 10f;
    [Range(1f, 50f)] public float heightMultiplier = 5f;

    // �۸� ������ ������
    [Range(1, 10)] public int octaves = 4;
    [Range(0f, 1f)] public float persistence = 0.5f;
    [Range(1f, 5f)] public float lacunarity = 2f;

    // �� ���� ���ؿ� �����ϴ� ���� (����ȭ�� ���� ����)
    [Header("Height Ranges (0~1)")]
    [Range(0f, 1f)] public float level1 = 0.1f;
    [Range(0f, 1f)] public float level2 = 0.3f;
    [Range(0f, 1f)] public float level3 = 0.5f;
    [Range(0f, 1f)] public float level4 = 0.7f;
    [Range(0f, 1f)] public float level5 = 0.9f;

    // �� ���� ������ �����ϴ� ��Ƽ�����
    [Header("Materials for each level (1~5)")]
    public Material[] materials = new Material[5];

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // �޽� ���� �޼���
    public void GenerateMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        // ���� �� UV ��ǥ �迭 �ʱ�ȭ
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        float[,] heightMap = new float[width + 1, height + 1];

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        // �� ������ ���� ��� �� ����
        for (int z = 0, i = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float y = CalculateHeight(x, z); // ���� ���
                vertices[i] = new Vector3(x, y, z); // ���� ��ġ ����
                uvs[i] = new Vector2((float)x / width, (float)z / height); // UV ��ǥ ����
                heightMap[x, z] = y;

                // �ּ�/�ִ� ���� ����
                if (y < minHeight) minHeight = y;
                if (y > maxHeight) maxHeight = y;
            }
        }

        // �� ����޽ÿ� �ش��ϴ� �ﰢ�� ����Ʈ �ʱ�ȭ
        List<int>[] submeshTriangles = new List<int>[materials.Length];
        for (int i = 0; i < materials.Length; i++)
            submeshTriangles[i] = new List<int>();

        // �ﰢ�� ���� �� ����޽ÿ� �й�
        for (int z = 0, vert = 0; z < height; z++, vert++)
        {
            for (int x = 0; x < width; x++, vert++)
            {
                int a = vert;
                int b = vert + width + 1;
                int c = vert + 1;
                int d = vert + width + 2;

                // �� ���� �ﰢ������ �簢�� ����
                AddTriangleToSubmesh(a, b, c, vertices, submeshTriangles, minHeight, maxHeight);
                AddTriangleToSubmesh(c, b, d, vertices, submeshTriangles, minHeight, maxHeight);
            }
        }

        // �޽� ��ü ���� �� ����
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // ���� ���� ���� ��� 32��Ʈ �ε��� ���
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.subMeshCount = materials.Length;

        // ����޽ÿ� �ﰢ�� �Ҵ�
        for (int i = 0; i < materials.Length; i++)
            mesh.SetTriangles(submeshTriangles[i], i);

        mesh.RecalculateNormals(); // ��� ���� �ڵ� ���
        meshFilter.sharedMesh = mesh;

        // ��Ƽ���� �迭 ����
        Material[] finalMats = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
            finalMats[i] = materials[i] != null ? materials[i] : materials[0]; // ��������� ù ��°�� ��ü

        meshRenderer.sharedMaterials = finalMats;
    }

    // �۸� ����� �̿��� ���� ���
    float CalculateHeight(int x, int z)
    {
        float amplitude = 1f;
        float frequency = 1f;
        float noiseHeight = 0f;

        float xCoord = x / scale;
        float zCoord = z / scale;

        for (int i = 0; i < octaves; i++)
        {
            float perlin = Mathf.PerlinNoise(xCoord * frequency, zCoord * frequency);
            noiseHeight += perlin * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noiseHeight * heightMultiplier;
    }

    // �ﰢ���� �ش� ���̿� ���� ����޽ÿ� �߰�
    void AddTriangleToSubmesh(int a, int b, int c, Vector3[] verts, List<int>[] submeshTris, float minH, float maxH)
    {
        // ��� ���� ��� �� ����ȭ
        float avgY = (verts[a].y + verts[b].y + verts[c].y) / 3f;
        float t = Mathf.InverseLerp(minH, maxH, avgY);

        int index = 0;
        if (t < level1) index = 0;
        else if (t < level2) index = 1;
        else if (t < level3) index = 2;
        else if (t < level4) index = 3;
        else if (t < level5) index = 4;
        else index = materials.Length - 1;

        submeshTris[index].AddRange(new int[] { a, b, c });
    }
}
