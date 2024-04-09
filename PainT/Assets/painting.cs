using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingOnModel : MonoBehaviour
{
    public Texture2D brushTexture; // �귯�� �ؽ�ó
    public Color brushColor = Color.red; // �귯�� ����
    public float brushSize = 0.1f; // �귯�� ũ��

    private RaycastHit hit;
    private Texture2D canvasTexture; // �𵨿� �׷��� �ؽ�ó

    void Start()
    {
        // �𵨿� �׷��� �ؽ�ó ���� �� �ʱ�ȭ
        Renderer rend = GetComponent<Renderer>();
        canvasTexture = new Texture2D(rend.material.mainTexture.width, rend.material.mainTexture.height);
        Color[] pixels = new Color[canvasTexture.width * canvasTexture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear; // ������ �������� �ʱ�ȭ
        }
        canvasTexture.SetPixels(pixels);
        canvasTexture.Apply();
        rend.material.mainTexture = canvasTexture;
    }

    void Update()
    {
        // ���콺 Ŭ�� ����
        if (Input.GetMouseButton(0))
        {
            // ����ĳ��Ʈ�� ���� ���콺 �����Ͱ� 3D �𵨿� ��Ҵ��� Ȯ��
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // �ؽ�ó ��ǥ ���
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= canvasTexture.width;
                pixelUV.y *= canvasTexture.height;

                // �׸� �׸���
                DrawOnTexture((int)pixelUV.x, (int)pixelUV.y);
            }
        }
    }

    // �ؽ�ó�� �׸��� �׸��� �Լ�
    void DrawOnTexture(int x, int y)
    {
        // �ؽ�ó �ȼ��� �귯�� ������ ����
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

        // �ؽ�ó ������Ʈ
        canvasTexture.Apply();
    }
}
