using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev
{
    public class SampleMsgDispatch : MonoBehaviour
    {
        private void Update()
        {
            //�߳�ͬ��֧��
            //MsgManager.Instance.Enqueue(0, 300);

            MsgManager.Instance.Dispatch(0, 100);
        }
    }
}
