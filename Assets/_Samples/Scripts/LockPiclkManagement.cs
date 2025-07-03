using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockPiclkManagement : MonoBehaviour
{
    #region Variables
    float pickPosition;
    [SerializeField]
    float pickSpeed=3f;
    [SerializeField]
    float cyllinderRotationSpeed = 0.4f;
    [SerializeField]
    float cylinderPosition;

    Animator animator;
    bool paused = false;

    float targetPosition;
    [SerializeField] float leanency = 0.1f;
    float MaxRotationDistance { 
        get { return 1f-Mathf.Abs(targetPosition - PickPosition) + leanency;}

    }

    bool shaking;
    float tension = 0f;
    [SerializeField] float tensionMultiplicator = 1f;

    #endregion 

    #region Property
    public float PickPosition
    {
        get { return pickPosition; }
        set
        {
            pickPosition = value;
            pickPosition = Mathf.Clamp(pickPosition, 0f, 1f);

        }
    }
    public float CylinderPosition
    {
        get { return cylinderPosition; }
        set {
            cylinderPosition = value;
            cylinderPosition = Mathf.Clamp(cylinderPosition, 0f, 1f); }
    }
    #endregion
    #region Unity Event Method
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Init();
    }
   
    private void Update()
    {
        if (paused == true) { return; }
        if (Input.GetAxisRaw("Vertical") == 0)
        {
            Pick();
        }
        Shaking();
        UpdateAnimator();
        Cyllinder();
    }
    #endregion

    #region Custom Method
    void Init()
    {
        Reset();
       
        targetPosition = UnityEngine.Random.value;
    }

    public void Reset()
    {
        CylinderPosition = 0;
        PickPosition = 0;
        tension = 0f;
        paused = false;
    }

    private void Shaking()
    {
        shaking = MaxRotationDistance - cylinderPosition < 0.03f;
        if (shaking)
        {
            tension += Time.deltaTime * tensionMultiplicator;
            if(tension > 1f)
            {
                PickBreake();
            }
        }
    }
    private void PickBreake()
    {
        Debug.Log("You broke the pick");
        Reset();
    }
    private void Cyllinder()
    {
        CylinderPosition -= cyllinderRotationSpeed * Time.deltaTime;
        CylinderPosition += Mathf.Abs(Input.GetAxisRaw("Vertical")) * Time.deltaTime * cyllinderRotationSpeed;
        if(CylinderPosition > 0.98f)
                {
            Win();
        }
    }
    private void Win()
    {
        paused = true;
        Debug.Log("You Opened the lock");
    }
    private void UpdateAnimator()
    {
        animator.SetFloat("PickPosition",PickPosition);
        animator.SetFloat("LockOpen", CylinderPosition);
        animator.SetBool("Shake", shaking);
    }
    private void Pick()
    {
        PickPosition += Input.GetAxis("Horizontal") * Time.deltaTime * pickSpeed;
    }
    #endregion
}
