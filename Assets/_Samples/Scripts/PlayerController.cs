using UnityEngine;

namespace Sample_Test
{
    //윌슨의 이동을 제어하는 클래스
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        //참조
        public Rigidbody volleyball;            //배구공 강체
        public GroundCheck groundCheck;         //그라운드 체크 트리거
        private Transform cameraTransform;

        //물리수치들
        [SerializeField] private float defaultMoveForce = 5f;   //이동 힘 기본값
        [SerializeField] private float runForceFactor = 2f;     //달리기 인수(계수)
        [SerializeField] private float maxSpeed = 10f;          //최대 속도
        private float moveForce;                                //윌슨에 최종적으로 가해지는 힘 (move)

        [SerializeField] private float jumpForce = 5f;          //점프 힘
        private bool isJump = false;

        private Vector3 moveInput;      //이동 입력

        #endregion

        #region Unity Event Method
        private void Start()
        {
            cameraTransform = Camera.main.transform;
            //커서 제어
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            transform.position = volleyball.transform.position;
            RotateWithAspect();     //카메라가 바라보고 있는 방향으로
            HandleInput();          //WASD 입력받기

            //달리기 - 공중에서 갑자기 빨라지면 어색하니까 땅에서만 달릴 수 있게
            if (Input.GetKey(KeyCode.LeftShift) && groundCheck.IsGrounded)    //땅에서 Shift 입력할 시
            {
                Run();
            }
            else
            {
                //이동속도 정상화
                moveForce = defaultMoveForce;
            }


        }
        private void FixedUpdate()
        {
            ClampLinearVelocity();
            Move();
            if (isJump)
            {
                Jump();
            }
            
        }
        #endregion

        #region Custom Method
        //인풋 받아오기
        private void HandleInput()
        {
            //WASD 입력
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            moveInput = new Vector3(horizontal, 0f, vertical).normalized;

            //점프 인풋
            if (Input.GetKeyDown(KeyCode.Space) && groundCheck.IsGrounded)  //땅에서 Space를 입력할 시
            {
                isJump = true;
                //Debug.Log(isJump);
            }
        }

        //받아온 인풋 값을 토대로 윌슨 굴리기
        private void Move()
        {
            if (volleyball.linearVelocity.z > maxSpeed || volleyball.linearVelocity.x > maxSpeed)
                return;

            //로컬 기준으로 방향 변환
            Vector3 localMove = transform.TransformDirection(moveInput);
            volleyball.AddForce(localMove * moveForce, ForceMode.Force);
        }
        
        //카메라가 바라보는 방향으로 회전 (y축 무시)
        private void RotateWithAspect()
        {
            Vector3 dir = transform.position - cameraTransform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        private void Run()
        {
            //가해지는 힘 연산
            moveForce = runForceFactor * defaultMoveForce;
            //비주얼 이펙트 (흑먼지 etc...)
            //...
        }

        //최대속도 제한
        void ClampLinearVelocity()
        {
            Vector3 velocity = volleyball.linearVelocity;

            // XZ 평면 속도 추출
            Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

            // 평면 속도가 max를 넘었으면 자르기
            if (flatVelocity.magnitude > maxSpeed)
            {
                flatVelocity = flatVelocity.normalized * maxSpeed;
                volleyball.linearVelocity = new Vector3(flatVelocity.x, velocity.y, flatVelocity.z);
            }

            //Debug.Log(flatVelocity.magnitude);
        }

        private void Jump()
        {
            volleyball.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //비주얼 이펙트
            //...

            //isJump 초기화
            isJump = false;
            //Debug.Log(isJump);
        }
        #endregion
    }

}

/*
25-06-20 : 최대속도 실험
이론상 가속하면 무한정 올라갈 것 같은데 어느 순간 속도가 안올라가는 것 같음 (아니면 매우 더디게 올라가거나)
그래서 속도제한이 딱히 필요없을 수도??
*/