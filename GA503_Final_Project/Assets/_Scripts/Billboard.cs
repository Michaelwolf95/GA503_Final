using System;
using UnityEngine;

namespace GA503
{
    public class Billboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main)
            {
                this.transform.LookAt(transform.position - (Camera.main.transform.position - transform.position), Vector3.up);
            }
        }
    }
}