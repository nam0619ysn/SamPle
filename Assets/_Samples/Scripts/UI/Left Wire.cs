using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Wilson.Player
{
    public class LeftWire : MonoBehaviour
    {
        public EWireColor WireColor { get; private set; }

        public bool IsConnected { get; private set; }

        [SerializeField]
        private List<Image> mWireImages;

        [SerializeField]
        private Image mLightImage;

        [SerializeField]
        private RightWire mConnectedWire;

        [SerializeField]
        private RectTransform mWireBody;

        [SerializeField]
        private float offset = 15f;

        private Canvas mGameCanvas;
        // Start is called before the first frame update
        void Start()
        {
            mGameCanvas = FindObjectOfType<Canvas>();
        }

        public void SetTarget(Vector3 targetPosition, float offset)
        {
            // 1. Canvas 스케일 반영
            float scaleFactor = mGameCanvas.transform.localScale.x;

            // 2. 각도 계산
            Vector2 direction = targetPosition - mWireBody.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 3. 거리 계산
            float distance = direction.magnitude + offset;

            // 4. 적용
            mWireBody.localRotation = Quaternion.Euler(0, 0, angle);
            mWireBody.sizeDelta = new Vector2(distance / scaleFactor, mWireBody.sizeDelta.y);
        }

        public void ResetTarget()
        {
            mWireBody.localRotation = Quaternion.Euler(Vector3.zero);
            mWireBody.sizeDelta = new Vector2(0f, mWireBody.sizeDelta.y);
        }

        public void SetWireColor(EWireColor wireColor)
        {
            WireColor = wireColor;
            Color color = Color.black;
            switch (WireColor)
            {
                case EWireColor.Red:
                    color = Color.red;
                    break;

                case EWireColor.Blue:
                    color = Color.blue;
                    break;

                case EWireColor.Yellow:
                    color = Color.yellow;
                    break;

                case EWireColor.Magenta:
                    color = Color.magenta;
                    break;
            }

            foreach (var image in mWireImages)
            {
                image.color = color;
            }
        }

        public void ConnectWire(RightWire rightWire)
        {
            if (mConnectedWire != null && mConnectedWire != rightWire)
            {
                mConnectedWire.DisconnectWire(this);
                mConnectedWire = null;
            }

            mConnectedWire = rightWire;
            if (mConnectedWire.WireColor == WireColor)
            {
                mLightImage.color = Color.yellow;
                IsConnected = true;
            }
        }

        public void DisconnectWire()
        {
            if (mConnectedWire != null)
            {
                mConnectedWire.DisconnectWire(this);
                mConnectedWire = null;
            }
            mLightImage.color = Color.gray;
            IsConnected = false;
        }
    }

}