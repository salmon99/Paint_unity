using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

public class TextureColorToData : MonoBehaviour
{
    public Renderer targetRenderer;
    public List<DataItem> dataList = new List<DataItem>();

    void Start()
    {
        StartCoroutine(ReadCSV());
    }

    IEnumerator ReadCSV()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "bodyPartMatch.csv");
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                ParseCSV(www.downloadHandler.text);
            }
        }
    }

    void ParseCSV(string csvData)
    {
        string[] lines = csvData.Split('\n');
        bool isFirstLine = true;

        foreach (var line in lines)
        {
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }
            if (line == "") continue;

            var values = line.Split(',');

            DataItem item = new DataItem
            {
                Num = values[0].Replace("\"", ""),
                BodyPartKR = values[1].Replace("\"", ""),
                BodyPartEN = values[2].Replace("\"", ""),
                ColorCode = values[3].Replace("\"", ""),
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
            return tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
        }

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