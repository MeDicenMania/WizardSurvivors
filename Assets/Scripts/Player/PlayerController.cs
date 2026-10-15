using UnityEditor.Toolbars;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Variables
 
    private float acc;
    [SerializeField] public Vector2 moveDir;
    public float lastHorizontal;
    public float lastVertical;
    [HideInInspector] public Vector2 lastMovedVector;

    //REFERENCIASS
    Rigidbody2D rb;
    public WizardsScriptableObject wizardData;


   

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);

    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Update()
    {
        InputManagement();
    }
    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if(moveDir.x != 0)
        {
            lastHorizontal = moveDir.x;
            lastMovedVector = new Vector2 (lastHorizontal,0f);
        }
        if (moveDir.y != 0)
        {
            lastVertical = moveDir.y;
            lastMovedVector = new Vector2(0f,lastVertical);
        }

        if(moveDir.x != 0 && moveDir.y !=0)
        {
            lastMovedVector = new Vector2(lastHorizontal, lastVertical);
        }

    }
    void Move()
    {
        rb.linearVelocity = new Vector2(moveDir.x * wizardData.MoveSpeed , moveDir.y * wizardData.MoveSpeed );
    }
}
