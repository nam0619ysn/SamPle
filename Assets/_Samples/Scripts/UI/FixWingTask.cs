using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Wilson.Player
{
    public enum EWireColor
    {
        None = -1,
        Red,
        Blue,
        Yellow,
        Magenta
    }
    public class FixWingTask : MonoBehaviour
    {

        [SerializeField]
        private List<LeftWire> mLeftWires;

        [SerializeField]
        private List<RightWire> mRightWires;

        private LeftWire mSelectedWire;

        private void OnEnable()
        {
            List<int> numberPool = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                numberPool.Add(i);
            }

            int index = 0;
            while (numberPool.Count != 0)
            {
                var number = numberPool[Random.Range(0, numberPool.Count)];
                mLeftWires[index++].SetWireColor((EWireColor)number);
                numberPool.Remove(number);
            }

            for (int i = 0; i < 4; i++)
            {
                numberPool.Add(i);
            }

            index = 0;
            while (numberPool.Count != 0)
            {
                var number = numberPool[Random.Range(0, numberPool.Count)];
                mRightWires[index++].SetWireColor((EWireColor)number);
                numberPool.Remove(number);
            }
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0f; // 또는 Camera.main.nearClipPlane
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (hit.collider != null)
                {
                    var left = hit.collider.GetComponentInParent<LeftWire>();
                    if (left != null)
                    {
                        mSelectedWire = left;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (mSelectedWire != null)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
                    foreach (var hit in hits)
                    {
                        var right = hit.collider?.GetComponentInParent<RightWire>();
                        if (right != null)
                        {
                            mSelectedWire.SetTarget(hit.transform.position, -50f);
                            mSelectedWire.ConnectWire(right);
                            right.ConnectWire(mSelectedWire);
                            mSelectedWire = null;
                            return;
                        }
                    }

                    mSelectedWire.ResetTarget();
                    mSelectedWire.DisconnectWire();
                    mSelectedWire = null;
                }
            }

            if (mSelectedWire != null)
            {
                mSelectedWire.SetTarget(worldPos, -15f);
            }
        }
    }
}