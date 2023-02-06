using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev
{
    public class SampleInput : MonoBehaviour
    {
        private void Start()
        {
            InputManager.Instance.RegisterAction("W", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.W } });
            InputManager.Instance.RegisterAction("A", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.A } });
            InputManager.Instance.RegisterAction("S", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.S } });
            InputManager.Instance.RegisterAction("D", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.D } });

            InputManager.Instance.RegisterAction("����", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.LeftControl, KeyCode.X } });
            InputManager.Instance.RegisterAction("����", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.LeftControl, KeyCode.C } });
            InputManager.Instance.RegisterAction("ճ��", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.LeftControl, KeyCode.V } });

            InputManager.Instance.RegisterAction("�ո�", new InputData() { KeyCodes = new List<KeyCode>() { KeyCode.Space } });

            InputManager.Instance.RegisterAction("Mouse X", new InputData() { Type = InputData.InputType.Axes, Axis = "Mouse X" });

            InputManager.Instance.Save();
        }


        private void Update()
        {
            //if (InputManager.Instance.GetKey("W"))
            //{
            //    Debug.Log("W");
            //}
            //if (InputManager.Instance.GetKey("A"))
            //{
            //    Debug.Log("A");
            //}
            //if (InputManager.Instance.GetKey("S"))
            //{
            //    Debug.Log("S");
            //}
            //if (InputManager.Instance.GetKey("D"))
            //{
            //    Debug.Log("D");
            //}


            if (InputManager.Instance.GetKeyDown("����"))
            {
                Debug.Log("����");
            }

            if (InputManager.Instance.GetKeyDown("ճ��"))
            {
                Debug.Log("ճ��");
            }

            if (InputManager.Instance.GetKeyUp("����"))
            {
                Debug.Log("����");
            }

            if (InputManager.Instance.GetKeyUp("�ո�"))
            {
                Debug.Log("�ո�");
            }


            if (InputManager.Instance.GetAxis("Mouse X") > 0f)
            {
                Debug.Log("Mouse X");
            }


            if (InputManager.Instance.GetKeyUp("A") || InputManager.Instance.GetKeyUp("D"))
            {
                Debug.Log("AD:" + Time.frameCount);
            }

            if ( InputManager.Instance.GetKeyDown("S"))
            {
                Debug.Log("WS:" + Time.frameCount);
            }

            if (InputManager.Instance.GetKeyDown("W") )
            {
                Debug.Log("WS:" + Time.frameCount);
            }
        }
    }
}
