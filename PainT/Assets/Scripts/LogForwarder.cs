using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LogForwarder : MonoBehaviour
{
    // iOS 네이티브 코드로 로그 메시지를 전달하기 위한 함수 선언
    [DllImport("__Internal")]
    private static extern void ForwardLogToiOS(string logMessage);

    // MonoBehaviour의 OnEnable 메서드에서 로그 이벤트 핸들러 등록
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    // MonoBehaviour의 OnDisable 메서드에서 로그 이벤트 핸들러 등록 해제
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    // 로그 메시지를 처리하고 iOS로 전달하는 메서드
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logMessage = $"{type}: {logString}\n{stackTrace}";
        ForwardLogToiOS(logMessage);
    }
}