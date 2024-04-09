using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��¥ : 2021-06-18 AM 2:30:31
// �ۼ��� : Rito

/// <summary> ���콺 �巡�׷� �ؽ��Ŀ� �׸� �׸��� </summary>
[DisallowMultipleComponent]
public class TexturePaintBrush : MonoBehaviour
{
    /***********************************************************************
    *                               Public Fields
    ***********************************************************************/
    #region .

    [Range(0.01f, 1f)] public float brushSize = 0.1f;
    public Texture2D brushTexture;
    public Color brushColor = Color.white;

    #endregion
    /***********************************************************************
    *                               Private Fields
    ***********************************************************************/
    #region .

    private TexturePaintTarget paintTarget;
    private Collider prevCollider;

    private Texture2D CopiedBrushTexture; // �ǽð����� ���� ĥ�ϴµ� ���Ǵ� �귯�� �ؽ��� ī�Ǻ�
    private Vector2 sameUvPoint; // ���� �����ӿ� ���콺�� ��ġ�� ��� UV ���� (���� ��ġ�� ��ø�ؼ� �׸��� ���� ����)

    #endregion

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Awake()
    {
        // ����� �귯�� �ؽ��İ� ���� ���, �� ����� �ؽ��� ����
        if (brushTexture == null)
        {
            CreateDefaultBrushTexture();
        }

        CopyBrushTexture();
    }

    private void Update()
    {
        UpdateBrushColorOnEditor();

        if (Input.GetMouseButton(0) == false) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit)) // delete previous and uncomment for mouse painting
        {
            Collider currentCollider = hit.collider;
            if (currentCollider != null)
            {
                // ��� ���� ����
                if (prevCollider == null || prevCollider != currentCollider)
                {
                    prevCollider = currentCollider;
                    currentCollider.TryGetComponent(out paintTarget);
                }

                // ������ �������� ��ø�Ͽ� �ٽ� �׸��� ����
                if (sameUvPoint != hit.lightmapCoord)
                {
                    sameUvPoint = hit.lightmapCoord;
                    Vector2 pixelUV = hit.lightmapCoord;
                    pixelUV.x *= paintTarget.resolution;
                    pixelUV.y *= paintTarget.resolution;
                    paintTarget.DrawTexture(pixelUV.x, pixelUV.y, brushSize, CopiedBrushTexture);
                }
            }
        }
    }
    #endregion
    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .
    /// <summary> �귯�� ���� ���� </summary>
    public void SetBrushColor(in Color color)
    {
        brushColor = color;
        CopyBrushTexture();
    }

    #endregion
    /***********************************************************************
    *                               Private Methods
    ***********************************************************************/
    #region .

    /// <summary> �⺻ ����(��)�� �귯�� �ؽ��� ���� </summary>
    private void CreateDefaultBrushTexture()
    {
        int res = 512;
        float hRes = res * 0.5f;
        float sqrSize = hRes * hRes;

        brushTexture = new Texture2D(res, res);
        brushTexture.filterMode = FilterMode.Point;
        brushTexture.alphaIsTransparency = true;

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                // Sqaure Length From Center
                float sqrLen = (hRes - x) * (hRes - x) + (hRes - y) * (hRes - y);
                float alpha = Mathf.Max(sqrSize - sqrLen, 0f) / sqrSize;

                //brushTexture.SetPixel(x, y, (sqrLen < sqrSize ? brushColor : Color.clear));
                brushTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        brushTexture.Apply();
    }

    /// <summary> ���� �귯�� �ؽ��� -> ���� �귯�� �ؽ���(���� ����) ���� </summary>
    private void CopyBrushTexture()
    {
        if (brushTexture == null) return;

        // ������ ī�� �ؽ��Ĵ� �޸� ����
        DestroyImmediate(CopiedBrushTexture);

        // ���Ӱ� �Ҵ�
        {
            CopiedBrushTexture = new Texture2D(brushTexture.width, brushTexture.height);
            CopiedBrushTexture.filterMode = FilterMode.Point;
            CopiedBrushTexture.alphaIsTransparency = true;
        }

        int height = brushTexture.height;
        int width = brushTexture.width;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = brushColor;
                c.a *= brushTexture.GetPixel(x, y).a;

                CopiedBrushTexture.SetPixel(x, y, c);
            }
        }

        CopiedBrushTexture.Apply();

        Debug.Log("Copy Brush Texture");
    }

    #endregion
    /***********************************************************************
    *                               Editor Only
    ***********************************************************************/
    #region .
#if UNITY_EDITOR
    // ���� ���� �����Ͽ� �귯�� �ؽ��� �ٽ� ����
    private Color prevBrushColor;
    private float brushTextureUpdateCounter = 0f;
    private const float BrushTextureUpdateCounterInitValue = 0.7f;
    private void OnValidate()
    {
        if (Application.isPlaying && prevBrushColor != brushColor)
        {
            brushTextureUpdateCounter = BrushTextureUpdateCounterInitValue;
            prevBrushColor = brushColor;
        }
    }
#endif
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void UpdateBrushColorOnEditor()
    {
        if (brushTextureUpdateCounter > 0f &&
            brushTextureUpdateCounter <= BrushTextureUpdateCounterInitValue)
        {
            brushTextureUpdateCounter -= Time.deltaTime;
        }

        if (brushTextureUpdateCounter < 0f)
        {
            CopyBrushTexture();
            brushTextureUpdateCounter = 9999f;
        }
    }
    #endregion
}