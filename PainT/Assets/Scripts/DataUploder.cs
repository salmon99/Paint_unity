using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

[System.Serializable]
public class LogData
{
    public string[] BodyPartKR;
    public string[] BodyPartEN;
}
public class DataUploder : MonoBehaviour{

    private HashSet<string> logMessagesSet = new HashSet<string>();
    private List<string> bodyPartKRLogs = new List<string>();
    private List<string> bodyPartENLogs = new List<string>();

    public string uploadURL = "http://chi-iu.com/unity/images";
    
    public Button uploadButton;

    public Camera mainCamera;
    public Transform model;

    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        initialCameraPosition = mainCamera.transform.position;
        initialCameraRotation = mainCamera.transform.rotation;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logMessage = $"{type}: {logString}\n{stackTrace}";
        //Debug.Log($"Received log: {logMessage}");
        
        // 중복된 로그가 아닌 경우에만 처리
        if (!logMessagesSet.Contains(logMessage))
        {
            logMessagesSet.Add(logMessage);
            string[] splitLog = logString.Split('/');
            //Debug.Log($"splitLog Length: {splitLog.Length}");
            
            if (splitLog.Length == 2)
            {
                string bodyPartKR = splitLog[0].Trim();
                string bodyPartEN = splitLog[1].Trim();
                bodyPartKRLogs.Add(bodyPartKR);
                bodyPartENLogs.Add(bodyPartEN);
                //Debug.Log($"Added log: {bodyPartKR} / {bodyPartEN}");
            }
            else
            {
                // "/"가 포함되지 않은 로그는 무시
                Debug.LogWarning("Log format is incorrect: " + logMessage);
            }
        }
    }

    void Start()
    {
        uploadButton.onClick.AddListener(OnUploadButtonClick);
    }
    
    void OnUploadButtonClick()
    {
        StartCoroutine(CaptureAndUploadData());
    }
    
    private IEnumerator CaptureAndUploadData()
    {
        uploadButton.gameObject.SetActive(false);
        
        // 초기 위치에서 스크린샷 캡처        
        mainCamera.transform.position = initialCameraPosition;
        mainCamera.transform.rotation = initialCameraRotation;        
        yield return new WaitForEndOfFrame();
        Texture2D screenImage1 = CaptureScreenshot();
        
        // 현재 위치에서 모델을 기준으로 반대편에서 스크린샷 캡처
        mainCamera.transform.RotateAround(model.position, Vector3.up, 180f);
        yield return new WaitForEndOfFrame();
        Texture2D screenImage2 = CaptureScreenshot();

        // 카메라를 원래 위치로 복구
        mainCamera.transform.position = initialCameraPosition;
        mainCamera.transform.rotation = initialCameraRotation;

        // 이미지를 PNG 형식의 바이트 배열로 변환
        byte[] imageBytes1 = screenImage1.EncodeToPNG();
        byte[] imageBytes2 = screenImage2.EncodeToPNG();

        //Debug.Log($"bodyPartKRLogs: {string.Join(", ", bodyPartKRLogs)}");
        //Debug.Log($"bodyPartENLogs: {string.Join(", ", bodyPartENLogs)}");

        // 로그 메시지를 JSON 형식으로 변환
        LogData logData = new LogData
        {
            BodyPartKR = bodyPartKRLogs.ToArray(),
            BodyPartEN = bodyPartENLogs.ToArray() 
        };
        string jsonLogData = JsonUtility.ToJson(logData);
        //Debug.Log($"JSON Log Data: {jsonLogData}");

        // FormData 생성
        WWWForm form = new WWWForm();
        form.AddField("username", "user123");
        form.AddField("logData", jsonLogData);
        form.AddBinaryData("files", imageBytes1, "screenshot1.png", "image/png");
        form.AddBinaryData("files", imageBytes2, "screenshot2.png", "image/png");

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
            
            string jsonResponse = www.downloadHandler.text;
            Debug.Log("Server Response: " + jsonResponse);

            // 업로드 후 로그 초기화
            bodyPartKRLogs.Clear();
            bodyPartENLogs.Clear();
        }
        uploadButton.gameObject.SetActive(true);
    }
    
    private Texture2D CaptureScreenshot()
    {
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        return screenImage;
    }    
}
