using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��¥ : 2021-06-19 AM 3:00:38
// �ۼ��� : Rito

/*
 * [NOTE]
 * 
 * - Rito/PaintTexture ���׸��� ���
 * 
 */

/// <summary> 
/// �׸� �׷��� ���
/// </summary>
[DisallowMultipleComponent]
public class TexturePaintTarget : MonoBehaviour
{
    /***********************************************************************
    *                               Static Fields
    ***********************************************************************/
    #region .
    private static Texture2D ClearTex
    {
        get
        {
            if (_clearTex == null)
            {
                _clearTex = new Texture2D(1, 1);
                _clearTex.SetPixel(0, 0, Color.clear);
                _clearTex.Apply();
            }
            return _clearTex;
        }
    }
    private MaterialPropertyBlock TextureBlock
    {
        get
        {
            if (_textureBlock == null)
            {
                _textureBlock = new MaterialPropertyBlock();
            }
            return _textureBlock;
        }
    }

    private static Texture2D _clearTex;
    private MaterialPropertyBlock _textureBlock;

    private static readonly string PaintTexPropertyName = "_PaintTex";

    #endregion
    /***********************************************************************
    *                               Private Fields
    ***********************************************************************/
    #region .
    private MeshRenderer _mr;

    #endregion
    /***********************************************************************
    *                               Public Fields
    ***********************************************************************/
    #region .
    public int resolution = 512;
    public RenderTexture renderTexture = null;

    #endregion
    /***********************************************************************
    *                               Unity Magics
    ***********************************************************************/
    #region .
    private void Awake()
    {
        Init();
        InitRenderTexture();
    }

    #endregion
    /***********************************************************************
    *                               Private Methods
    ***********************************************************************/
    #region .

    private void Init()
    {
        TryGetComponent(out _mr);
    }

    /// <summary> ���� �ؽ��� �ʱ�ȭ </summary>
    private void InitRenderTexture()
    {
        renderTexture = new RenderTexture(resolution, resolution, 32);
        Graphics.Blit(ClearTex, renderTexture);

        // ���׸��� ������Ƽ ��� �̿��Ͽ� ��Ī �����ϰ�
        // ���׸����� ������Ƽ�� ���� �ؽ��� �־��ֱ�
        TextureBlock.SetTexture(PaintTexPropertyName, renderTexture);
        _mr.SetPropertyBlock(TextureBlock);
    }

    #endregion
    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .
    /// <summary> ���� �ؽ��Ŀ� �귯�� �ؽ��ķ� �׸��� </summary>
    public void DrawTexture(float posX, float posY, float brushSize, Texture2D brushTexture)
    {
        RenderTexture.active = renderTexture; // �������� ���� Ȱ�� ���� �ؽ��� �ӽ� �Ҵ�
        GL.PushMatrix();                      // ��Ʈ���� ����
        GL.LoadPixelMatrix(0, resolution, resolution, 0); // �˸��� ũ��� �ȼ� ��Ʈ���� ����

        float brushPixelSize = brushSize * resolution;

        // ���� �ؽ��Ŀ� �귯�� �ؽ��ĸ� �̿��� �׸���
        Graphics.DrawTexture(
            new Rect(
                posX - brushPixelSize * 0.5f,
                (renderTexture.height - posY) - brushPixelSize * 0.5f,
                brushPixelSize,
                brushPixelSize
            ),
            brushTexture
        );

        GL.PopMatrix();              // ��Ʈ���� ����
        RenderTexture.active = null; // Ȱ�� ���� �ؽ��� ����
    }
    #endregion
}