using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVPositionRecognizer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // ���콺 Ŭ�� ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider == null || meshCollider.sharedMesh == null)
                    return;

                Mesh mesh = meshCollider.sharedMesh;
                Vector2 uv = mesh.uv[hit.triangleIndex * 3];  // UV ��ǥ ����

                Debug.Log("UV Position: " + uv);
                ProcessUVPosition(uv);
            }
        }
    }

    void ProcessUVPosition(Vector2 uv)
    {
        // UV ��ġ�� ���� ó�� ���� ����
        Debug.Log($"Processing at UV: {uv}");
    }
}
