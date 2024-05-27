using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class DataUploder : MonoBehaviour{

    private HashSet<string> logMessagesSet = new HashSet<string>();
    private List<string> bodyPartKRLogs = new List<string>();
    private List<string> bodyPartENLogs = new List<string>();

    public string uploadURL = "https://yourserver.com/upload"; //주소 입력 필요

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logMessage = $"{type}: {logString}\n{stackTrace}";
        // 중복된 로그가 아닌 경우에만 처리
        if (!logMessagesSet.Contains(logMessage))
        {
            logMessagesSet.Add(logMessage);
            string[] splitLog = logString.Split('/');
            if (splitLog.Length == 2)
            {
                string bodyPartKR = splitLog[0].Trim();
                string bodyPartEN = splitLog[1].Trim();
                bodyPartKRLogs.Add(bodyPartKR);
                bodyPartENLogs.Add(bodyPartEN);
            }
            else
            {
                // "/"가 포함되지 않은 로그는 무시
                Debug.LogWarning("Log format is incorrect: " + logMessage);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureAndUploadData());
        }
    }

    private IEnumerator CaptureAndUploadData()
    {
        yield return new WaitForEndOfFrame();

        // 스크린샷 캡처
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        // 이미지를 PNG 형식의 바이트 배열로 변환
        byte[] imageBytes = screenImage.EncodeToPNG();

        // 로그 메시지를 JSON 형식으로 변환
        var logData = new Dictionary<string, object>
        {
            { "BodyPartKR", bodyPartKRLogs },
            { "BodyPartEN", bodyPartENLogs }
        };
        string jsonLogData = JsonUtility.ToJson(logData);

        // FormData 생성
        WWWForm form = new WWWForm();
        form.AddField("logData", jsonLogData);
        form.AddBinaryData("image", imageBytes, "screenshot.png", "image/png");

        // UnityWebRequest를 사용하여 서버로 데이터 업로드
        UnityWebRequest www = UnityWebRequest.Post(uploadURL, form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error uploading data: " + www.error);
        }
        else
        {
            Debug.Log("Data uploaded successfully");
            // 업로드 후 로그 초기화
            bodyPartKRLogs.Clear();
            bodyPartENLogs.Clear();
        }
    }
}
