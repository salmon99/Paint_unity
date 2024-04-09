using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingOnModel : MonoBehaviour
{
    public Texture2D brushTexture; // 브러시 텍스처
    public Color brushColor = Color.red; // 브러시 색상
    public float brushSize = 0.1f; // 브러시 크기

    private RaycastHit hit;
    private Texture2D canvasTexture; // 모델에 그려질 텍스처

    void Start()
    {
        // 모델에 그려질 텍스처 생성 및 초기화
        Renderer rend = GetComponent<Renderer>();
        canvasTexture = new Texture2D(rend.material.mainTexture.width, rend.material.mainTexture.height);
        Color[] pixels = new Color[canvasTexture.width * canvasTexture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear; // 투명한 색상으로 초기화
        }
        canvasTexture.SetPixels(pixels);
        canvasTexture.Apply();
        rend.material.mainTexture = canvasTexture;
    }

    void Update()
    {
        // 마우스 클릭 감지
        if (Input.GetMouseButton(0))
        {
            // 레이캐스트를 통해 마우스 포인터가 3D 모델에 닿았는지 확인
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // 텍스처 좌표 계산
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= canvasTexture.width;
                pixelUV.y *= canvasTexture.height;

                // 그림 그리기
                DrawOnTexture((int)pixelUV.x, (int)pixelUV.y);
            }
        }
    }

    // 텍스처에 그림을 그리는 함수
    void DrawOnTexture(int x, int y)
    {
        // 텍스처 픽셀에 브러시 색상을 적용
        for (int i = -Mathf.RoundToInt(brushSize); i <= Mathf.RoundToInt(brushSize); i++)
        {
            for (int j = -Mathf.RoundToInt(brushSize); j <= Mathf.RoundToInt(brushSize); j++)
            {
                int brushX = x + i;
                int brushY = y + j;

                if (brushX >= 0 && brushX < canvasTexture.width && brushY >= 0 && brushY < canvasTexture.height)
                {
                    canvasTexture.SetPixel(brushX, brushY, brushColor);
                }
            }
        }

        // 텍스처 업데이트
        canvasTexture.Apply();
    }
}
