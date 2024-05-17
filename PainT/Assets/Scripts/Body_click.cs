using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomToObject : MonoBehaviour
{
    public Transform targetObject; // 확대하여 보여줄 개체의 Transform
    public float zoomDistance = 5f; // 개체로부터의 카메라 거리

    private Vector3 originalCameraPosition; // 원래 카메라 위치 저장
    private bool isZoomed = false; // 개체를 확대 중인지 여부를 나타내는 플래그

    void Start()
    {
        // 원래 카메라 위치 저장
        originalCameraPosition = transform.position;
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭하면서 개체에 마우스 커서가 있을 때
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast를 사용하여 개체에 충돌하는지 확인
            if (Physics.Raycast(ray, out hit) && hit.transform == targetObject)
            {
                // 개체를 확대하거나 원래 위치로 되돌립니다.
                if (!isZoomed)
                {
                    // 개체를 비추는 방향으로 카메라 이동
                    Vector3 directionToTarget = (targetObject.position - transform.position).normalized;
                    transform.position = targetObject.position - directionToTarget * zoomDistance;

                    // 카메라의 높이를 대상의 높이와 동일하게 설정
                    Vector3 cameraPosition = transform.position;
                    cameraPosition.y = targetObject.position.y;
                    transform.position = cameraPosition;

                    isZoomed = true;
                }
                else
                {
                    // 원래 카메라 위치로 되돌리기
                    transform.position = originalCameraPosition;
                    isZoomed = false;
                }
            }
        }
    }
}
