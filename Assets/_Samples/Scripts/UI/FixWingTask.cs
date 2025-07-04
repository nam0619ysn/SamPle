using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            List<int> numberPool = new List<int> { 0, 1, 2, 3 };

            // 왼쪽 전선 색상 섞기
            for (int i = 0; i < mLeftWires.Count; i++)
            {
                int rand = Random.Range(0, numberPool.Count);
                mLeftWires[i].SetWireColor((EWireColor)numberPool[rand]);
                numberPool.RemoveAt(rand);
            }

            numberPool = new List<int> { 0, 1, 2, 3 };

            // 오른쪽 전선 색상 섞기
            for (int i = 0; i < mRightWires.Count; i++)
            {
                int rand = Random.Range(0, numberPool.Count);
                mRightWires[i].SetWireColor((EWireColor)numberPool[rand]);
                numberPool.RemoveAt(rand);
            }
        }

        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;

            // 🖱 클릭 시작
            if (Input.GetMouseButtonDown(0))
            {
                // 1. UI에서 클릭 감지
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> uiHits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, uiHits);

                foreach (RaycastResult result in uiHits)
                {
                    var left = result.gameObject.GetComponentInParent<LeftWire>();
                    if (left != null)
                    {
                        mSelectedWire = left;
                        return;
                    }
                }

                // 2. 월드 오브젝트 클릭 감지
                RaycastHit2D worldHit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (worldHit.collider != null)
                {
                    var left = worldHit.collider.GetComponentInParent<LeftWire>();
                    if (left != null)
                    {
                        mSelectedWire = left;
                    }
                }
            }

            // 🖱 클릭 놓음
            if (Input.GetMouseButtonUp(0))
            {
                if (mSelectedWire != null)
                {
                    bool connected = false;

                    // 1. UI 대상 확인
                    PointerEventData pointerData = new PointerEventData(EventSystem.current)
                    {
                        position = Input.mousePosition
                    };

                    List<RaycastResult> uiHits = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, uiHits);

                    foreach (RaycastResult result in uiHits)
                    {
                        var right = result.gameObject.GetComponentInParent<RightWire>();
                        if (right != null)
                        {
                            mSelectedWire.SetTarget(result.gameObject.transform.position, -50f);
                            mSelectedWire.ConnectWire(right);
                            right.ConnectWire(mSelectedWire);
                            connected = true;
                            break;
                        }
                    }

                    // 2. 월드 대상 확인
                    if (!connected)
                    {
                        RaycastHit2D[] worldHits = Physics2D.RaycastAll(worldPos, Vector2.zero);
                        foreach (var hit in worldHits)
                        {
                            var right = hit.collider?.GetComponentInParent<RightWire>();
                            if (right != null)
                            {
                                mSelectedWire.SetTarget(hit.transform.position, -50f);
                                mSelectedWire.ConnectWire(right);
                                right.ConnectWire(mSelectedWire);
                                connected = true;
                                break;
                            }
                        }
                    }

                    if (!connected)
                    {
                        mSelectedWire.ResetTarget();
                        mSelectedWire.DisconnectWire();
                    }

                    mSelectedWire = null;
                }
            }

            // 🖱 드래그 중 (선 연장)
            if (mSelectedWire != null)
            {
                mSelectedWire.SetTarget(worldPos, -15f);
            }
        }
    }
}
