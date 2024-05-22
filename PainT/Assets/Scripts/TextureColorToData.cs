using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TextureColorToData : MonoBehaviour
{
    public Renderer targetRenderer;
    public List<DataItem> dataList = new List<DataItem>();
	public string textureName = "female";

    void Start()
    {
		Texture2D loadedTexture = Resources.Load<Texture2D>(textureName);
		if (loadedTexture != null)
        {
            // 텍스처를 쉐이더 프로퍼티에 적용
            targetRenderer.material.SetTexture("_TrackTex", loadedTexture);
        }
        else
        {
            Debug.LogError("텍스처를 로드할 수 없습니다: " + textureName);
        }
        ReadCSV();
    }

    void ReadCSV()
    {
        TextAsset csvData = Resources.Load<TextAsset>("BodyPartMatch");
    	if (csvData == null)
    	{
        	Debug.LogError("CSV file not found in Resources folder");
        	return;
    	}

    	string[] lines = csvData.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

    	foreach (string line in lines)
    	{
        	if (string.IsNullOrEmpty(line)) continue;

            var values = line.Split(',');

            DataItem item = new DataItem
            {
                Num = values[0].Replace("\"",""),
                BodyPartKR = values[1].Replace("\"",""),
                BodyPartEN = values[2].Replace("\"",""),
                ColorCode = values[3].Replace("\"",""),
            };
            dataList.Add(item);
            Debug.Log("Data line : " + item.ColorCode);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            Color pixelColor = GetColorFromTexture(pos, "_TrackTex");
            String color = pixelColor.r * 255 + "/" + pixelColor.g * 255 + "/" + pixelColor.b * 255;
            FindDataByColor(color, dataList);
        }
    }

    Color GetColorFromTexture(Vector2 screenPosition, string textureName)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
            Material mat = renderer.material; 
            Texture2D tex = mat.GetTexture(textureName) as Texture2D;

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
			Debug.Log(pixelUV.x+","+pixelUV.y);
            return tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
        }
		Debug.Log("RaycastHit이 올바르게 반환되지 않음.");
        return Color.black;
    }
    public void FindDataByColor(string inputColorCode, List<DataItem> dataList)
    {
        foreach(DataItem item in dataList){
            if (item.ColorCode.Equals(inputColorCode))
            {
                Debug.Log("Match found! Color Code: " + item.ColorCode + ", Body part: " + item.BodyPartKR + "/" + item.BodyPartEN);
                return;
            }
        Debug.Log("No match found for color code: " + inputColorCode);
        }
    }
}


[System.Serializable]
public class DataItem
{
    public string Num;
    public string ColorCode;
    public string BodyPartKR;
    public string BodyPartEN;
}