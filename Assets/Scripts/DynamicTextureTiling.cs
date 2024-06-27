using UnityEngine;

[ExecuteInEditMode]
public class DynamicTextureTiling : MonoBehaviour
{
    private Material materialInstance;

    void Start()
    {
        InitMaterial();
    }

    void Update()
    {
        // Ensure material is initialized before updating texture tiling
        if (materialInstance == null)
        {
            InitMaterial();
        }

        if (materialInstance != null) {
            SetTextureTiling(transform.localScale);
        }
    }

    private void InitMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            if (materialInstance == null)
            {
                materialInstance = new Material(renderer.sharedMaterial);
                renderer.material = materialInstance;
            }
        }
    }

    private void SetTextureTiling(Vector3 scale)
    {
        Vector2 tiling = new Vector2(scale.x, scale.z);
        materialInstance.mainTextureScale = tiling;
    }

    private void OnDestroy()
    {
        if (Application.isEditor)
        {
            DestroyImmediate(materialInstance);
        }
        else
        {
            Destroy(materialInstance);
        }
    }
}
