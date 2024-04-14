using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVPositionRecognizer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 마우스 클릭 감지
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider == null || meshCollider.sharedMesh == null)
                    return;

                Mesh mesh = meshCollider.sharedMesh;
                Vector2 uv = mesh.uv[hit.triangleIndex * 3];  // UV 좌표 추출

                Debug.Log("UV Position: " + uv);
                ProcessUVPosition(uv);
            }
        }
    }

    void ProcessUVPosition(Vector2 uv)
    {
        // UV 위치에 따른 처리 로직 구현
        Debug.Log($"Processing at UV: {uv}");
    }
}
