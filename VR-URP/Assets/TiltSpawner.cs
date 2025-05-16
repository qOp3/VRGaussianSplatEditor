using UnityEngine;

public class TiltSpawner : MonoBehaviour
{
    public GameObject prefab;        // 要复制的对象
    public int rows = 5;             // 行数
    public int cols = 5;             // 列数
    public float spacing = 2f;       // 每个物体之间的间隔

    void Start()
    {
        if (prefab == null)
        {
            Debug.LogWarning("请指定要平铺的 Prefab");
            return;
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(j * spacing, 0, i * spacing);
                Instantiate(prefab, transform.position + position, Quaternion.identity, transform);
            }
        }
    }
}
