using UnityEngine;

namespace Sample_Test
{
    //�ٴ� üũ ����� �ϴ� Ŭ����
    public class GroundCheck : MonoBehaviour
    {
        #region Property
        public bool IsGrounded { get; private set; }
        #endregion

        #region Unity Event Method
        private void OnTriggerStay(Collider other)
        {
            if (other.transform.tag == "Ground")
            {
                IsGrounded = true;
                //Debug.Log("��Ҵ�");
            }
        }
        

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag == "Ground")
            {
                IsGrounded = false;
                Debug.Log("��������");
            }
        }
        #endregion

    }

}
