using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParent : MonoBehaviour
{
    public void ParentPlayer()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.playerController.transform.parent = transform;
    }

    public void UnparentPlayer()
    {
        //if (GameManager.Instance != null)
            //GameManager.Instance.playerController.transform.parent = null;
    }
}
