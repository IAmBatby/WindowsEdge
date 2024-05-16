using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class ProBuilderGizmos : MonoBehaviour
{
    public ProBuilderMesh proBuilderMesh;

    public void OnDrawGizmosSelected()
    {
        if (proBuilderMesh == null)
            proBuilderMesh = GetComponent<ProBuilderMesh>();
        Gizmos.color = Color.green;

        float lowestZ = 0f;
        float higherestZ = 0f;
        float highestY = 0f;

        ProBuilderMeshInfo newInfo = new ProBuilderMeshInfo(proBuilderMesh);
        Gizmos.DrawLine(transform.position + new Vector3(0, 0, lowestZ), transform.position + new Vector3(0, 0, higherestZ));

        GUIStyle newStyle = new GUIStyle();
        newStyle.fontSize = 30;
        newStyle.alignment = TextAnchor.MiddleCenter;
        Handles.Label(transform.position + new Vector3(0, highestY + 2, 0), "Length: " + (newInfo.highestZ - newInfo.lowestZ), newStyle);

        List<ProBuilderMesh> proBuilderMeshes = new List<ProBuilderMesh>();
        foreach (Object selectionobject in Selection.objects)
        {
            //Debug.Log(selectionobject.name + " : " + selectionobject.GetType());
            if (selectionobject is GameObject selectionGameObject)
            {
                if (selectionGameObject.TryGetComponent(out ProBuilderMesh proBuilderMesh))
                {
                    //Debug.Log("Selecting " + proBuilderMesh.name);
                    proBuilderMeshes.Add(proBuilderMesh);
                }
            }
        }

        //proBuilderMeshes = proBuilderMeshes.OrderBy(o => o.transform.position).ToList();

        Debug.Log("ProBuilderMesh Selection Count: " + proBuilderMeshes.Count);
        int counter = 0;


        Vector3 firstPosition = Vector3.zero;
        Vector3 secondPosition = Vector3.zero;

        foreach (ProBuilderMesh proBuilderMesh1 in proBuilderMeshes)
        {
            if (counter > 0 && proBuilderMeshes.Count > 1)
            {
                ProBuilderMeshInfo firstSelectionInfo = new ProBuilderMeshInfo(proBuilderMeshes[counter - 1]);
                ProBuilderMeshInfo secondSelectioninfo = new ProBuilderMeshInfo(proBuilderMeshes[counter]);

                if (firstSelectionInfo.lowestZ > secondSelectioninfo.highestZ)
                {
                    firstPosition = proBuilderMeshes[counter - 1].transform.position + new Vector3(0f, 0f, firstSelectionInfo.highestZ);
                    secondPosition = proBuilderMeshes[counter].transform.position + new Vector3(0f, 0f, secondSelectioninfo.lowestZ);
                }
                else
                {
                    firstPosition = proBuilderMeshes[counter - 1].transform.position + new Vector3(0f, 0f, firstSelectionInfo.lowestZ);
                    secondPosition = proBuilderMeshes[counter].transform.position + new Vector3(0f, 0f, secondSelectioninfo.highestZ);
                }

            }
            counter++;
        }

        Debug.DrawLine(firstPosition, secondPosition);
        //Handles.Label(new Vector3(transform.position.x, transform.position.y, proBuilderMeshes[1].transform.position.z + Vector3.Distance(proBuilderMeshes[0].transform.position, proBuilderMeshes[1].transform.position)), "Length: " + Vector3.Distance(firstPosition, secondPosition).ToString(), newStyle);

    }

    public struct ProBuilderMeshInfo
    {
        public float lowestZ;
        public float highestZ;

        public float highestY;

        public ProBuilderMeshInfo(ProBuilderMesh proBuilderMesh)
        {
            lowestZ = 0f;
            highestZ = 0f;

            highestY = 0f;
            foreach (Vector3 vertPosition in proBuilderMesh.positions)
            {
                Gizmos.DrawCube(proBuilderMesh.transform.position + vertPosition, new Vector3(0.1f, 0.1f, 0.1f));
                if (vertPosition.z < lowestZ)
                    lowestZ = vertPosition.z;
                if (vertPosition.z > highestZ)
                    highestZ = vertPosition.z;
                if (vertPosition.y > highestY)
                    highestY = vertPosition.y;
            }
        }
    }
}
