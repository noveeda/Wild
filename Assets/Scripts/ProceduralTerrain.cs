using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    // 지형 크기 설정
    [Range(10, 400)] public int width = 100;
    [Range(10, 400)] public int height = 100;

    // 노이즈 샘플링 스케일 및 높이 배율
    [Range(1f, 100f)] public float scale = 10f;
    [Range(1f, 50f)] public float heightMultiplier = 5f;

    // 퍼린 노이즈 설정값
    [Range(1, 10)] public int octaves = 4;
    [Range(0f, 1f)] public float persistence = 0.5f;
    [Range(1f, 5f)] public float lacunarity = 2f;

    // 각 높이 수준에 대응하는 범위 (정규화된 높이 기준)
    [Header("Height Ranges (0~1)")]
    [Range(0f, 1f)] public float level1 = 0.1f;
    [Range(0f, 1f)] public float level2 = 0.3f;
    [Range(0f, 1f)] public float level3 = 0.5f;
    [Range(0f, 1f)] public float level4 = 0.7f;
    [Range(0f, 1f)] public float level5 = 0.9f;

    // 각 높이 범위에 대응하는 머티리얼들
    [Header("Materials for each level (1~5)")]
    public Material[] materials = new Material[5];

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // 메쉬 생성 메서드
    public void GenerateMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        // 정점 및 UV 좌표 배열 초기화
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        float[,] heightMap = new float[width + 1, height + 1];

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        // 각 정점의 높이 계산 및 저장
        for (int z = 0, i = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float y = CalculateHeight(x, z); // 높이 계산
                vertices[i] = new Vector3(x, y, z); // 정점 위치 설정
                uvs[i] = new Vector2((float)x / width, (float)z / height); // UV 좌표 설정
                heightMap[x, z] = y;

                // 최소/최대 높이 추적
                if (y < minHeight) minHeight = y;
                if (y > maxHeight) maxHeight = y;
            }
        }

        // 각 서브메시에 해당하는 삼각형 리스트 초기화
        List<int>[] submeshTriangles = new List<int>[materials.Length];
        for (int i = 0; i < materials.Length; i++)
            submeshTriangles[i] = new List<int>();

        // 삼각형 생성 및 서브메시에 분배
        for (int z = 0, vert = 0; z < height; z++, vert++)
        {
            for (int x = 0; x < width; x++, vert++)
            {
                int a = vert;
                int b = vert + width + 1;
                int c = vert + 1;
                int d = vert + width + 2;

                // 두 개의 삼각형으로 사각형 구성
                AddTriangleToSubmesh(a, b, c, vertices, submeshTriangles, minHeight, maxHeight);
                AddTriangleToSubmesh(c, b, d, vertices, submeshTriangles, minHeight, maxHeight);
            }
        }

        // 메쉬 객체 생성 및 설정
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 정점 수가 많을 경우 32비트 인덱스 사용
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.subMeshCount = materials.Length;

        // 서브메시에 삼각형 할당
        for (int i = 0; i < materials.Length; i++)
            mesh.SetTriangles(submeshTriangles[i], i);

        mesh.RecalculateNormals(); // 노멀 벡터 자동 계산
        meshFilter.sharedMesh = mesh;

        // 머티리얼 배열 적용
        Material[] finalMats = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
            finalMats[i] = materials[i] != null ? materials[i] : materials[0]; // 비어있으면 첫 번째로 대체

        meshRenderer.sharedMaterials = finalMats;
    }

    // 퍼린 노이즈를 이용해 높이 계산
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

    // 삼각형을 해당 높이에 따라 서브메시에 추가
    void AddTriangleToSubmesh(int a, int b, int c, Vector3[] verts, List<int>[] submeshTris, float minH, float maxH)
    {
        // 평균 높이 계산 후 정규화
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
