using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts
{
    class ClickEverywhereScript : MonoBehaviour
    {
        UnityEngine.UI.Button Button;
        public Camera renderCamera;
        void Start()
        {
            Button = GetComponent<UnityEngine.UI.Button>();
        }

        void OnMouseDown()
        {
			Debug.Log("OnMouseDown___==click or tap detected");
//#if !UNITY_EDITOR
//            var wp = renderCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
//            Vector2 touchPos = new Vector2(wp.x, wp.y);
//
//            RaycastHit hit = new RaycastHit();
//            bool f = Physics.Raycast(touchPos, Vector3.back,out hit, 50.0f, 0xFFFF);
//            Debug.Log("HITSTATUS_:::" + hit);
//            if (GetComponent<Collider2D>() == hit.collider)
//            {
//                Button.onClick.Invoke();
//            }
//#else
            Button.onClick.Invoke();
//#endif
        }
    }
}
