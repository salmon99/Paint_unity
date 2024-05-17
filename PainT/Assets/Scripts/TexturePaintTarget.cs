using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TexturePaintTarget : MonoBehaviour
{
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

    private MeshRenderer _mr;

    public int resolution = 512;
    public RenderTexture renderTexture = null;

    private void Awake()
    {
        Init();
        InitRenderTexture();
    }

    private void Init()
    {
        TryGetComponent(out _mr);
    }

    /// <summary> 렌더 텍스쳐 초기화 </summary>
    private void InitRenderTexture()
    {
        renderTexture = new RenderTexture(resolution, resolution, 32);
        Graphics.Blit(ClearTex, renderTexture);
        
        TextureBlock.SetTexture(PaintTexPropertyName, renderTexture);
        _mr.SetPropertyBlock(TextureBlock);
    }

    /// <summary> 렌더 텍스쳐에 브러시 텍스쳐로 그리기 </summary>
    public void DrawTexture(float posX, float posY, float brushSize, Texture2D brushTexture)
    {
        RenderTexture.active = renderTexture; 
        GL.PushMatrix();                      
        GL.LoadPixelMatrix(0, resolution, resolution, 0); 

        float brushPixelSize = brushSize * resolution;

        
        Graphics.DrawTexture(
            new Rect(
                posX - brushPixelSize * 0.5f,
                (renderTexture.height - posY) - brushPixelSize * 0.5f,
                brushPixelSize,
                brushPixelSize
            ),
            brushTexture
        );

        GL.PopMatrix();              
        RenderTexture.active = null; 
    }
}