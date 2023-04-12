using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FastDev
{
    public class SamplePool : MonoBehaviour
    {
        public Button btAllocate;
        // Start is called before the first frame update
        void Start()
        {
            btAllocate.onClick.AddListener(() =>
            {
                GameObject obj = PoolManager.Instance.Allocate("Assets/FastDev/4.PoolManager/Sample/Cube.prefab");
                obj.SetActive(true);
                obj.transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));

                PoolManager.Instance.Recycle(obj, 1000).Forget();
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
