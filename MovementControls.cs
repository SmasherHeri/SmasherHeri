using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MovementControls : MonoBehaviour
{

    [Header ("Inputs")]
    [SerializeField] protected float valorDelta = 0f;
    
    protected bool ShootButton;
    protected bool DashButton;
    protected bool ReloadButton;
    protected bool JumpButton;
    protected bool ClimbButton;
    [SerializeField] protected float HorizontalMoveButton;
    [SerializeField] protected float VerticalMoveButton;
    protected float ElevationMoveButton;

   

    [Header ("Camera")]
    
    Camera myCamera;/// solo para el fuego
    [SerializeField] public MoveCamY _cameraY;

    [Header ("Player References")]

    [SerializeField]
    protected PlayableCharacter _character;
    
    [SerializeField]
    protected Transform playerInputSpace = default;

    	[SerializeField]
	protected LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, waterMask = 0, iceMask = 0, windMask = 0;

    [SerializeField] 
    protected Material diPe;

    public ClimbingRigidB climbingScript;
    [SerializeField] protected Rigidbody PhysicsA;
    public Rigidbody Rb => PhysicsA;
    [SerializeField] Rigidbody  connectedBody, previousConnectedBody;

    [SerializeField] public Transform characterCore;
    [SerializeField] public Transform _characterModel;
    [SerializeField] public Transform _orientation;
    [SerializeField] public Collider _collider;
    protected CharacterController _controller;

    public bool leaveDeadZone;

     [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    protected float startYScale;

    [Header ("Slope Handling")]
    [SerializeField] bool checkSteepTest;
    [SerializeField, Range(0f, 90f)]
	protected float maxGroundAngle = 25f, maxStairsAngle = 50f;
    [SerializeField, Range(0f, 100f)]
	float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)]
	float probeDistance = 1f;
    [SerializeField]
    protected float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    protected Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;

    [SerializeField] protected bool exitingSlope;
    

    [Header ("Ground check")]
   	[SerializeField] protected int groundContactCount, steepContactCount, climbContactCount;

     [SerializeField] public bool _isGround;// Testing Purposes 
	public bool OnGround => groundContactCount > 0;
     protected bool OnSteep => steepContactCount > 0;

    protected int stepsSinceLastGrounded, stepsSinceLastJump;
    [SerializeField] protected Vector3 gravityTest;
    [SerializeField] protected float gravityNormalDotTest;


    [Header ("Swimming")]

    [SerializeField] public WaterEnvironment _waterScript;
    [SerializeField] public float waterVelocity; // Is not used
    [SerializeField] public bool _isSwiming = false; //Is for old camera
    [SerializeField] public bool _isSwimingTest = false; //Testing Purposes
  
    [SerializeField] protected bool InWater => submergence > 0f;
    [SerializeField] protected float submergence;

    [SerializeField, Range(0f, 10f)] 
    protected float waterDrag = 1f;

    [SerializeField, Min(0f)]
	protected float buoyancy = 1f;

    [SerializeField, Range(0.01f, 1f)]
	protected float swimThreshold = 0.5f;

    // protected bool Swimming => submergence >= swimThreshold;
    public bool Swimming => submergence >= swimThreshold;
     [SerializeField] protected bool SwimmingTest; // test purposes

     [SerializeField]
	float submergenceOffset = 0.5f;

	[SerializeField, Min(0.1f)]
	float submergenceRange = 1f;


    [Header ("Movement Measures")]

    [SerializeField]
    protected Vector3 originalUpAxis, upAxis, rightAxis, forwardAxis;

    [SerializeField, Range(0f, 100f)]
	float maxSpeed = 10f, maxClimbSpeed = 2f, maxSwimSpeed = 5f;
    [SerializeField] protected Vector3 playerInput;

    protected Vector3 connectionWorldPosition, connectionLocalPosition;

    //[SerializeField]
	// Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    // [SerializeField, Range(0f, 1f)]
	// float bounciness = 0.5f;

    [SerializeField] protected float testHorizontal;
    [SerializeField] protected float testVertical;
    [SerializeField] protected float testRotationX;
    public float rotationDiference;
    [SerializeField] public float testRotationY;
    protected float rotationY = 0.0f;
    protected float rotationX = 0.0f;
    [SerializeField] protected float _gravity;
    [SerializeField] protected float _RBgravity;
    protected float _normalGravity = 1.3f;
    ///////////////////////Move tutorial   
    protected float desiredMoveSpeed;
    protected float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    
    Vector3 globalVelocity;
    //Vector3 velocity, desiredVelocity, connectionVelocity;
    [SerializeField] protected Vector3 velocity, connectionVelocity;
    Vector3 velocityVesel;

    [SerializeField, Range(0f, 100f)]
	protected float maxAcceleration = 10f, maxRunAcceleration = 10f, maxAirAcceleration = 1f, maxClimbAcceleration = 20f, maxSwimAcceleration = 5f;

    [SerializeField] Vector3 _velocitySlopeHandled;
    //[SerializeField] Vector3 direction;
    [SerializeField] Vector3 direction;
    [SerializeField] bool directionCast;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector3 GetSlopeVar;
    [SerializeField] Quaternion _getSlopeRotation;
    [SerializeField] Vector3 _getAdjustVelocity;
    [SerializeField] float _getAdjustVelocityY;
    [SerializeField] float _originalSteepOffset;

    [SerializeField] float adjustVelX;
    [SerializeField] float adjustVelY;
    [SerializeField] float adjustVelZ;


///////////////////////////////////////
    //protected int _countJump;
    //protected bool _canSaveJump= false;
    [SerializeField] protected float RotationSpeed;
    [SerializeField] protected float _normalRotation = 150f;
    [SerializeField] protected float _shootRotation = 90f;


    [Header ("Jumping")]

    [SerializeField] public bool _isJumping;
    [SerializeField] protected bool desiredJump, desiresClimbing , desiredDash;
    [SerializeField, Range(0f, 10f)] protected float jumpHeight = 2f;

    [SerializeField, Range(0, 5)] protected int maxAirJumps = 0;

    [SerializeField] protected int jumpPhase;
    [SerializeField] protected float _jumpHeight = 12.0f;// 20
    [SerializeField] protected float _jumpWait; ///////////
    [SerializeField] protected float _airMultiplier; ////////////// 
    [SerializeField] protected bool _airMultiCurrent;
    [SerializeField] public bool _jumpRun;
    [SerializeField] protected float _yVelocity;//// Only for documentation
        [SerializeField] protected float _zVelocity;//// Only for documentation
    [SerializeField] protected float _xVelocity;//// Only for documentation
    [SerializeField]  protected bool _canDoubleJump = false;//// Only for documentation
    [SerializeField] protected bool readyToJump;
    [SerializeField] public float jumpCooldown;
    [SerializeField] public float _airTime;
    [SerializeField] public float _airTimeMax;
   // [SerializeField] public float _airTime;
    [SerializeField] public float jumpTimeMax;
    






    [Header ("Walk")]
    public float walkSpeed;

    [Header ("Run")]

    public float sprintSpeed;
    [SerializeField] public int lastPressed;
    [SerializeField] public int tapCountLimit;
    [SerializeField] public float buttonComboTime;
    [SerializeField] public float buttonComboMaxTime = 0.2f;
    [SerializeField] protected bool isDashing;
    [SerializeField] public float resetTimer;
    [SerializeField] protected float dashMultiplier;
    [SerializeField] protected float dashAccelerationMultiplier;

    /////////////////////////////////////////////////////////7
    // WALL JuMP
    [Header ("Wall Jump And Run")]

    protected float wallRunSpeed;
    protected float wallJumpUpForce;
    protected float wallJumpSideForce;
    public Vector3 wallrunDirection;
    public Vector3 wallrunVelocity;
    public bool wallRunning;

    public float wallCheckDistance;
    public float minJumpHeight;
    protected RaycastHit leftWallhit;
    protected RaycastHit rightWallhit;
    [SerializeField] protected bool wallLeft = false ;
    [SerializeField] protected bool wallRight = false;
    [SerializeField] protected bool _aboveGrounf = false;

     [Header ("Climb")]
    public bool wallFront; // maybe is for testing

    [SerializeField, Range(90, 180)]
	protected float maxClimbAngle = 140f;

    public bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

        [SerializeField]
	Material normalMaterial = default, climbingMaterial = default, swimmingMaterial = default;  

    [SerializeField] protected SkinnedMeshRenderer meshRenderer;

    [SerializeField] protected float wallLookAngle;
    [SerializeField]  protected RaycastHit frontWallHit;
    [SerializeField] protected float detectionLength; 
        [SerializeField] protected float sphereCastRadius;
    protected RaycastHit frontWallHitinfo;
    protected RaycastHit hitInfo;
    public float maxWallAngle;
    Vector3 toWallDirection;
    public bool isWallVar;
    public bool isWallAngleVar;

    [Header("Frozzen")]

    public bool _isTouchIce = false;
    [SerializeField] public Explosion _IceCubeScript;

    protected Vector3 heading;
    [SerializeField] protected bool getPositionOnIce = false;

    protected Vector3 objectDirection;

     [SerializeField] protected float distance;
     [SerializeField] float defDistance;
      [SerializeField] Vector3 defHeading;

    protected float angleOfTarget;

    [SerializeField] Vector3 contact;
    [SerializeField] protected Vector3 fixedContact;

    bool _break = false;

    [SerializeField] protected Explosion _iceBlock;


    protected Vector3 pos, fw, up;

    [Header ("Raising Wind")]

    [SerializeField] protected bool isInRaisingAir;

    [Header ("Grappling")]

    public bool activeGrapple;
    public bool swinging;


    [Header ("Movement State")]
    public MovementState state;

   [SerializeField] Vector3 relativeVelocityDoc ;
   [SerializeField] Vector3 axisxdoc;
   [SerializeField] Vector3 axisydoc;
   [SerializeField] Vector3 axisUnivdoc;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        climbing,
        swimming,
        wallrunningAAA
    }

    
    public Vector3 initialDirRay;
    public Quaternion resultRot;

    
    public Vector3 DampVel = Vector3.zero;
    public Vector3 newWallDirection;

    protected float  _rayRotationRate = 0.1f;
    protected float _canRayRotate = -1;
    public Vector3 initPos;
    public Vector3 toWallTransform = Vector3.zero;
    public float speedTestAdjust;
    public float newXTest;
    public float newZTest;

    public float    currentXTest;
    public float playerInputSpeedTestX;
    public float  currentZTest;
     public float    playerInputSpeedTestZ;
     public float InputZTest;
    public float inputSlowTest;
    // Start is called before the first frame update

    private void Awake() 
    {
        _controller = GetComponent<CharacterController>();
        //GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        
        PhysicsA = (Rigidbody)GetComponent("Rigidbody");
        PhysicsA.useGravity = false;

       // meshRenderer = GameObject.Find("CreatureNewGameArmature/LambDPBig").GetComponent<SkinnedMeshRenderer>();
        OnValidate();
    }

    void Start()
    {
        // if (playerInputSpace == null)
        //     playerInputSpace
        leaveDeadZone = false;
        _airTime = 0f;
        getPositionOnIce = false;
        lastPressed = 0;
        isDashing = false;// not used 
        buttonComboTime = 0; // Not used, please check
        RotationSpeed = _normalRotation;

        

        startYScale = transform.localScale.y;

        PhysicsA.freezeRotation = true;
        readyToJump = true;
    }

 
    public void Update()
    {
        // Debug.DrawRay(transform.position, Vector3.Dot(gravityTest, contactNormal) * 10f, new Color(1, 0.5f, 0));
        
        gravityNormalDotTest = Vector3.Dot(gravityTest, contactNormal);
        SwimmingTest = Swimming;

         RaycastHit smothRayHit = new RaycastHit();

        if(Time.time > _canRayRotate)
        {
            _canRayRotate = Time.time + 1f;
            toWallTransform = _characterModel.forward * 10;
        
        }
   
        newWallDirection = Vector3.MoveTowards((_characterModel.forward * 10) -toWallTransform,_characterModel.forward * 10, 1f * Time.deltaTime);

        bool RayFirst = Physics.Raycast(transform.position, ProjectDirectionOnPlane(playerInputSpace.forward, upAxis), out smothRayHit, 10);
        Quaternion newWallDirectionLook = Quaternion.LookRotation(newWallDirection);
        
        initPos = toWallTransform;
        

        Vector3 targetDirection = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis)* 10;
                        Vector3 newDirection = Vector3.MoveTowards(ProjectDirectionOnPlane(playerInputSpace.forward, upAxis),targetDirection, 0.5f * Time.deltaTime);
               
        if(connectedBody == null)
		    isInRaisingAir = false;
        
        _isSwimingTest = InWater;
        
        
        if (_isTouchIce == false)
        {
             getPositionOnIce = false;

            if(CheckSteepContacts())
            checkSteepTest = true;
            else
            checkSteepTest = false;
            CalculateMovement();
        }
            
        

 

        
        //isWallAngleVar = isWallAngle();
        //frontWallHitinfo = new RaycastHit();
         toWallDirection = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis) * playerInput.y + ProjectDirectionOnPlane(playerInputSpace.right, upAxis) * playerInput.x;
        //wallFront = Physics.SphereCast(transform.position, sphereCastRadius,  toWallDirection, out frontWallHitinfo, detectionLength);
        // Debug.DrawRay(transform.position, velocity * detectionLength, Color.cyan);

        if (_waterScript == null)
        {
            _isSwiming = false;
        }

        if (_isTouchIce == false)
        {
            //this.transform.parent = null;
            //DontDestroyOnLoad(gameObject);
            
            Rb.constraints = ~RigidbodyConstraints.FreezePosition;
            Rb.constraints = RigidbodyConstraints.FreezeRotation;
            enabled = true;

        }

        InputZTest = Input.GetAxis("Vertical");

        bool rayPhisic;
             hitInfo = new RaycastHit();
        rayPhisic =  Physics.Raycast(transform.position, Vector3.down, out hitInfo,  10f);
       
        _RBgravity = Physics.gravity.y;
     

         PhysicsA.drag = 0;
        
        if (_isTouchIce == false)
        {
             desiredDash = DashButton;
            if (Swimming) {
                
                if(submergence > 0 && submergence < 1)
                {
                     desiredJump |= JumpButton;
                    //desiredJump = JumpButton;
                    
                    desiresClimbing = ClimbButton;
                }
                else 
                {
                    desiresClimbing = false;
                }
            }
            else {
                 desiredJump |= JumpButton;
                //desiredJump = JumpButton;
                
                desiresClimbing = ClimbButton;
            }
       }

        
        

        

        //diPe = GameObject.Find("CreatureNewGameArmature/LambDPBig").GetComponent<SkinnedMeshRenderer>().material;
        if (diPe != null){
                diPe.SetColor(
                "_Color", OnGround ? Color.black : Color.white
            );
        }
        
        if(meshRenderer != null)
        {
            // meshRenderer.material =
			// Climbing ? climbingMaterial :
			// InWater ? swimmingMaterial : normalMaterial;

            meshRenderer.material =
			Climbing ? climbingMaterial :
			Swimming ? swimmingMaterial : normalMaterial;

            meshRenderer.material.color = Color.white * submergence;
        }
        
        
    }

    public void FixedUpdate()
    {
        if (OnGround) {
            _airTime = 0f; // Code from Coyote Time
		}
        else if (!OnGround  && jumpPhase < 1){
             _airTime += Time.deltaTime;
             if ( _airTime >= _airTimeMax)
             jumpPhase = 1;
        }

        isWallVar = isWall();
        isWallAngleVar = isWallAngle();
        
        // if (_IceCubeScript == null)
        // {

            _isTouchIce = false;
            

            
        // }

        if (_isTouchIce == false)
        {
            CalculateMovementFixed();

            //transform.parent = null;
            transform.localEulerAngles = new Vector3(0,0,0);
            transform.localScale = new Vector3(1,1,1);
            
        }
        
        if(_isTouchIce == true)
        {
            //PhysicsA.velocity = Vector3.zero;
        }

        
    }


    public void  CalculateMovement()
    {   
        
        //        if (climbingScript.exitingWall) return;

        valorDelta = Time.deltaTime;

        // Vector2 playerInput;
  
        
        playerInput.x = HorizontalMoveButton;
        playerInput.y = VerticalMoveButton;
        // playerInput.z = Swimming ? Input.GetAxis("UpDown") : 0f;
        // playerInput.z = Swimming && ClimbButton ? 1 : 0f;
        playerInput.z = ElevationMoveButton;
        

		playerInput = Vector3.ClampMagnitude(playerInput, 1f);
        // playerInput.Normalize();
        //playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        
        if (playerInputSpace) {
            rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
			forwardAxis =
				ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
			
		}
		else {
			rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
		}
        //desiredVelocity =
		//	new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
       //velocity += acceleration * Time.deltaTime;

        
        Vector3 inputDir = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis) * playerInput.y + ProjectDirectionOnPlane(playerInputSpace.right, upAxis) * playerInput.x;

        if (inputDir != Vector3.zero)
        {
            //_characterModel.forward = Vector3.Slerp(_characterModel.forward, inputDir.normalized, Time.deltaTime * RotationSpeed);
            _characterModel.forward = Vector3.Slerp(_characterModel.forward, inputDir.normalized, Time.deltaTime * RotationSpeed);
            
            //_characterModel.Rotate(_characterModel.forward);
        }

       
        
  
    }

    

    public void CalculateMovementFixed()
    {
        //upAxis = -Physics.gravity.normalized;
		Vector3 gravity = CustomGravity.GetGravity(PhysicsA.position, out originalUpAxis);
        gravityTest = gravity;
        
        if(Swimming)
        {
            upAxis = playerInputSpace.up;
        }
        else{
            upAxis = originalUpAxis;
        }
                 
         //velocity = PhysicsA.velocity;
		UpdateState();

        if (InWater) {
            if(isInRaisingAir)
			    velocity *= 1f - waterDrag * submergence * Time.deltaTime;
            else
                velocity *= 1f - waterDrag * submergence * Time.deltaTime;
		}

        AdjustVelocity();
       
        if (desiredJump) {
			desiredJump = false;
			JumpAction(gravity);
		}

        if (Climbing) {
			velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
		}
        else if (InWater) {
			velocity +=
				// gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
                gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
		}
        else if (OnGround && velocity.sqrMagnitude < 0.01f) {
			velocity +=
				contactNormal *
				(Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
		}
        else if (desiresClimbing && OnGround) {
			velocity +=
				(gravity - contactNormal * (maxClimbAcceleration * 0.9f)) *
				Time.deltaTime;
		}
		else {
			velocity += gravity * Time.deltaTime;
		}

        // if (isWallAngle())
        // {
        //     //GetWallAngleMoveDirection();
        //     // xAxis =  GetWallAngleMoveDirection(ProjectDirectionOnPlane(xAxis, contactNormal));
        //     // zAxis =  GetWallAngleMoveDirection(ProjectDirectionOnPlane(zAxis, contactNormal));

        //     velocityVesel = velocity;
        //     // velocity = GetWallAngleMoveDirection(velocity);
        //     PhysicsA.velocity = GetWallAngleMoveDirection(velocityVesel);
        // }
        // else{
        //     PhysicsA.velocity = velocity;
        // }
        
        PhysicsA.velocity = velocity;
        

        //onGround = false;
		ClearState();
    }


    public void JumpAction(Vector3 gravity)
    {
        Vector3 jumpDirection;
        /////////////////////////////////
        //////////////////// ||  _airTime < _airTimeMax

        if (OnGround  || (!OnGround  && jumpPhase < 1)) {
			jumpDirection = contactNormal;
            //_airTimeIsReset = true;
            
		}
		else if (OnSteep) {
			jumpDirection = steepNormal;
            jumpPhase = 0;
		}
		// else if (jumpPhase < maxAirJumps)
        else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
            if (jumpPhase == 0) 
           // if (jumpPhase == 0 && !OnGround && _airTime >= _airTimeMax)
            {
				jumpPhase = 1;
			}
			jumpDirection = contactNormal;
		}
		else {
			return;
		}
       // if (OnGround || jumpPhase < maxAirJumps) {
        stepsSinceLastJump = 0;

        

        jumpPhase += 1;
        //float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
        float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
        if (InWater) {
			jumpSpeed *= Mathf.Max(0f, 1f - submergence / swimThreshold);
		}
        jumpDirection = (jumpDirection + upAxis).normalized;
        // float alignedSpeed = Vector3.Dot(velocity, contactNormal);
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        //if (velocity.y > 0f) 

        //if (alignedSpeed > 0f) {   ////TemporaryOut of service
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        //}  ///// TemporaryOut of service
       
			//velocity.y += jumpSpeed;
			//velocity += contactNormal * jumpSpeed;
        velocity += jumpDirection * jumpSpeed;
		//}
    }

    public void UpdateState () {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
		velocity = PhysicsA.velocity;
		if (CheckClimbing() || CheckSwimming() || OnGround || SnapToGround() || CheckSteepContacts()) {
            stepsSinceLastGrounded = 0;
			if (stepsSinceLastJump > 1) {
				jumpPhase = 0;
			}
            if (groundContactCount > 1) {
				contactNormal.Normalize();
			}
		}
        else 
        {
			contactNormal = upAxis;
		}

        if(connectedBody != null)
        {
            if (connectedBody.isKinematic || connectedBody.mass >= PhysicsA.mass) {
				UpdateConnectionState();
		    }
        }
        
	}

    public void UpdateConnectionState () {
        if (connectedBody == previousConnectedBody) {
			Vector3 connectionMovement =
				connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
			connectionVelocity = connectionMovement / Time.deltaTime;
		}
		connectionWorldPosition = PhysicsA.position;
		connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
			connectionWorldPosition
		);
	}

    public void AdjustVelocity () {
		
        float acceleration, speed;
        
        if (desiredDash)
        {
            dashMultiplier = 2;
            dashAccelerationMultiplier = 50;
        }
        else 
        {
            dashMultiplier = 1;
            dashAccelerationMultiplier = 1;
        }        

		Vector3 xAxis, zAxis;
		if (Climbing) {
            acceleration = maxClimbAcceleration;
			speed = OnGround && desiresClimbing ? maxClimbSpeed * dashMultiplier : maxSpeed * dashMultiplier ;
			xAxis = Vector3.Cross(contactNormal, upAxis);
			zAxis = upAxis;
		}
        else if (InWater) {
			float swimFactor = Mathf.Min(1f, submergence / swimThreshold);
			acceleration = Mathf.LerpUnclamped(
				OnGround ? maxAcceleration : maxAirAcceleration,
				maxSwimAcceleration * dashAccelerationMultiplier, swimFactor
			);
			speed = Mathf.LerpUnclamped(maxSpeed, maxSwimSpeed, swimFactor) * dashMultiplier ;
			xAxis = rightAxis;
			zAxis = forwardAxis;
		}
        
		else {
            acceleration = OnGround ? maxAcceleration  * dashAccelerationMultiplier: maxAirAcceleration;
            speed = maxSpeed * dashMultiplier;
            
                
			 xAxis = rightAxis;
		     zAxis = forwardAxis;
                
                
                 
		}

            xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
            zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);
           
        Vector3 relativeVelocity = velocity - connectionVelocity;
        relativeVelocityDoc = relativeVelocity;
		float currentX = Vector3.Dot(relativeVelocity, xAxis);
		float currentZ = Vector3.Dot(relativeVelocity, zAxis);
        
        //float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange;
		if (isInRaisingAir)
			maxSpeedChange = 1000 * Time.deltaTime;
		else
			maxSpeedChange = acceleration * Time.deltaTime;

        currentXTest = currentX;////////////TestingPurposes
        currentZTest = currentZ;

        playerInputSpeedTestX = playerInput.x * 20f;//test
         playerInputSpeedTestZ = playerInput.y * 20f;

		float newX =
			Mathf.MoveTowards(currentX, playerInput.x * speed, maxSpeedChange);
		float newZ =
			Mathf.MoveTowards(currentZ, playerInput.y * speed, maxSpeedChange);
        
        speedTestAdjust = speed;

        //inputSlowTest = SlowInterpolator();
        newXTest = newX;
        newZTest = newZ;

        Vector3 defDirection;

           
        if (isWallAngleVar && !Climbing)
        {
            Vector3 wallMoveDirection = GetWallAngleMoveDirection(toWallDirection);
            float currentWallVelocity = Vector3.Dot(relativeVelocity, wallMoveDirection);
            
            float newWallVelocity = 0;
            
                newWallVelocity = Mathf.MoveTowards(currentWallVelocity, Mathf.Sqrt((playerInput.x * playerInput.x) + (playerInput.y * playerInput.y)) * speed, maxSpeedChange);
            Vector3 newWallVelocityVector = wallMoveDirection * (newWallVelocity - currentWallVelocity);
            velocity += newWallVelocityVector;
        }
        else
        {
            velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        
        
        if (Swimming) {
			float currentY = Vector3.Dot(relativeVelocity, upAxis);
			float newY = Mathf.MoveTowards(
				currentY, playerInput.z * speed, maxSpeedChange
			);
			velocity += upAxis * (newY - currentY);
		}
	}

    public bool CheckClimbing () {
		if (Climbing) {
            if (climbContactCount > 1) {
				climbNormal.Normalize();
				float upDot = Vector3.Dot(upAxis, climbNormal);
				if (upDot >= minGroundDotProduct) {
					climbNormal = lastClimbNormal;
				}
			}
			groundContactCount = 1;
			contactNormal = climbNormal;
			return true;
		}
		return false;
	}

    public bool SnapToGround () {
        //if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || InWater) {
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
			return false;
		}
        float speed = velocity.magnitude;
		if (speed > maxSnapSpeed) {
			return false;
		}
        if (!Physics.Raycast(PhysicsA.position, -upAxis, out RaycastHit hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore)) {
			return false;
		}

        float upDot = Vector3.Dot(upAxis, hit.normal);
        // if (hit.normal.y < minGroundDotProduct) 
        if (upDot < GetMinDot(hit.collider.gameObject.layer)) {
			return false;
		}
		
        groundContactCount = 1;
		contactNormal = hit.normal;
        //float speed = velocity.magnitude;
		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f) {
			velocity = (velocity - hit.normal * dot).normalized * speed;
		}
        if(hit.rigidbody != null)
        {
            connectedBody = hit.rigidbody;
        }
		return true;
	}

    public void PreventSnapToGround () {  // That functioon was only on player script
		stepsSinceLastJump = -1;
	}

    public float GetMinDot (int layer) {
		return (stairsMask & (1 << layer)) == 0 ?
			minGroundDotProduct : minStairsDotProduct;
	}

    public void ClearState () {
		//onGround = false;
		groundContactCount = steepContactCount = climbContactCount = 0;
		//contactNormal = steepNormal =  connectionVelocity = Vector3.zero;
        contactNormal = steepNormal = climbNormal = Vector3.zero;
        connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
        
        //InWater = false;
		submergence = 0f;
	}

    public bool CheckSteepContacts () {
		if (steepContactCount > 1) {
			steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
			if (upDot  >= minGroundDotProduct) {
                steepContactCount = 0; //Verify this May19 2024
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

    public void ResetJump()
    {
        readyToJump = true;
        _canDoubleJump = true;
        
        exitingSlope = false;
    }

    public Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		 return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}

    public bool isWallAngle() 
    {
        if (Physics.SphereCast(transform.position, sphereCastRadius,  toWallDirection, out frontWallHit, detectionLength))

        {
            float angle = Vector3.Angle(toWallDirection, -frontWallHit.normal);
            wallLookAngle = angle;

            return angle > maxWallAngle && angle != 0;
        }
        return false;
    }

    public bool isWall()
    {
        if (Physics.SphereCast(transform.position, sphereCastRadius, toWallDirection, out frontWallHit, detectionLength))

        {
            float angle = Vector3.Angle(toWallDirection, -frontWallHit.normal);
            wallLookAngle = angle;

            return angle < maxWallAngle;
        }
        return false;
    }


    public Vector3 GetWallAngleMoveDirection(Vector3 direction)
    {
        //return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        return Vector3.ProjectOnPlane(direction, frontWallHit.normal).normalized;
    }


    public IEnumerator JumpFalling()
    {
        //_animator.SetBool("IsJumping" , true);
        //_animator.SetBool("IsFalling", false);
        //_animator.SetInteger("Condition" , 2); 
        _isJumping = true;
        yield return new WaitForSeconds(1f);
        //_animator.SetBool("IsJumping" , false);
        //_animator.SetBool("IsFalling", true);
        _isJumping = false;
        //_animator.SetInteger("Condition" , 4);
        
    }

    public void Freeze()
    {
        
            PhysicsA.constraints = RigidbodyConstraints.FreezePosition;
            PhysicsA.constraints = RigidbodyConstraints.FreezeRotation;
            PhysicsA.velocity = Vector3.zero;
        
    }

    public void EvaluateSubmergence (Collider collider) {
		
        if (Physics.Raycast(
			PhysicsA.position + originalUpAxis * submergenceOffset,
			-originalUpAxis, out RaycastHit hit, submergenceRange + 1f,
			waterMask, QueryTriggerInteraction.Collide
		)) {
			submergence = 1f - hit.distance / submergenceRange;
		}
        else {
			submergence = 1f;
		}

        if (Swimming) {
			connectedBody = collider.attachedRigidbody;
		}
	}

    public bool CheckSwimming () {
		if (Swimming) {
			groundContactCount = 0;
			// contactNormal = upAxis;
            // contactNormal = originalUpAxis; // this is the precvious value
            contactNormal = playerInputSpace.up;
			return true;
		}
		return false;
	}

    public void OnTriggerEnter(Collider other)
    {

        if ((waterMask & (1 << other.gameObject.layer)) != 0) {
            EvaluateSubmergence(other);
            //InWater = true;
        }

        if (other.gameObject.layer == 26) {
            isInRaisingAir = true;
            //transform.position = other.transform.position + new Vector3(0,2,0);
        }

        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "IceBlock(Clone)")
        {
            //_isTouchIce = true;
            _IceCubeScript = other.GetComponent<Explosion>();
            //DontDestroyOnLoad(gameObject);
        }

        if ((waterMask & (1 << other.gameObject.layer)) != 0) {
			//InWater = true;
            EvaluateSubmergence(other);
		}   

        // if (other.gameObject.layer == 26) {
		// 	isInRaisingAir = true;
        //      PullObject(other);
          
		// }
       
        
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.name == "IceBlock(Clone)")
        {
            //_IceCubeScript = other.GetComponent<Explosion>();
           // DontDestroyOnLoad(gameObject);
        }
 
    }

    void OnCollisionEnter (Collision collision) {

		EvaluateCollision(collision);      
	}

    private void OnCollisionStay(Collision collision)
    {

        //onGround = true;
		EvaluateCollision(collision);
        
          if(collision.gameObject != null)
        {

            _iceBlock = collision.gameObject.GetComponent<Explosion>();
            
            if(((iceMask & (1 << collision.gameObject.layer)) != 0) && leaveDeadZone == false && (_character != null && ((_iceBlock._Oponent == Explosion.Oponent.player && _character._CharacterType == PlayableCharacter.CharacterType.player) || (_iceBlock._Oponent == Explosion.Oponent.enemy && _character._CharacterType == PlayableCharacter.CharacterType.enemy))))
            {
                 //Debug.Break();
                
                                //distance = Vector3.Distance(collision.GetContact(0).point, transform.position);
                    
                    if (getPositionOnIce == false)
                    {
                        getPositionOnIce = true; 

                        fixedContact = collision.GetContact(0).point;
                        //heading = (_iceBlock.transform.position - fixedContact).normalized;
                        heading = (fixedContact - _iceBlock.transform.position).normalized;
                        //heading = (collision.GetContact(0).point - transform.position).normalized;
                        angleOfTarget = Vector3.Angle(_iceBlock.transform.forward, heading);
                        //distance = Vector3.Distance(_player.transform.position, transform.position);
                        distance = Vector3.Distance(fixedContact, _iceBlock.transform.position);
                        //defDistance = distance;
                        objectDirection = heading/distance;
                        //_player.transform.position = direction;
                        
                        transform.position = _iceBlock.transform.position + heading * distance;
                        pos = _iceBlock.transform.InverseTransformPoint(transform.position);
                        fw = _iceBlock.transform.InverseTransformDirection(transform.forward);
                        up = _iceBlock.transform.InverseTransformDirection(transform.up);
                    }
                    //Debug.DrawRay(fixedContact, - heading * distance, Color.green);
                    //Debug.Break();
                    // transform.position = _iceBlock.transform.position + heading * distance;
                        //////var updateHeading = (transform.position - _iceBlock.transform.position).normalized;
                                // iceCollision = true;
                        var newpos = _iceBlock.transform.TransformPoint(pos);
                        var newfw = _iceBlock.transform.TransformDirection(fw);
                       //var newfw = updateHeading;
                      // _characterModel.LookAt(_iceBlock.transform.position);
                        var newup = _iceBlock.transform.TransformDirection(up);
                        var newrot = Quaternion.LookRotation(newfw, newup);
                        transform.position = newpos;
                        transform.rotation = newrot;


                    
            }
        }


     }

    public void EvaluateCollision (Collision collision) {
        if (Swimming) {
			return;
		}

        int layer = collision.gameObject.layer;
		float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            //onGround |= normal.y >= minGroundDotProduct;
            //if (normal.y >= minGroundDotProduct)
            if (upDot >= minDot)  {
				//onGround = true;
				groundContactCount += 1;
				contactNormal += normal;
                connectedBody = collision.rigidbody;
			}
            //else if (upDot  > -0.01f) {
            else {
				if (upDot > -0.01f) {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0) {
                        connectedBody = collision.rigidbody;
                    }
			    }
                if (desiresClimbing && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) != 0) {
					climbContactCount += 1;
					climbNormal += normal;
                    lastClimbNormal = normal;
					connectedBody = collision.rigidbody;
				}
			}


		}
    }

    public void OnValidate () {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
	}

    public void ModifyRotationSpeed(bool isShooting)
    {
        if(isShooting)
        RotationSpeed = _shootRotation;
        else
        RotationSpeed = _normalRotation;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Debug.DrawLine(transform.position, Vector3.ProjectOnPlane(toWallDirection, hitInfo.normal) * 10);
        Gizmos.DrawWireSphere(transform.position + Vector3.ProjectOnPlane(toWallDirection, hitInfo.normal) * 10, sphereCastRadius);
    }

    IEnumerator ResetTaps()
    {
        yield return new WaitForSeconds(resetTimer);
        lastPressed = 0;
    }

     IEnumerator MoveRaycast(Vector3 initPos, Vector3 endPos, float duration)
    {
        float timeElapsed = 0f;
        //vector3 current;

        if (timeElapsed < duration)
        {
            //Debug.Log(current);
            newWallDirection =  Vector3.Lerp(initPos, endPos, 
                timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            //initPos = endPos;
            yield return null;
            
        }

        //toWallTransform = endPos;

    }

    IEnumerator MoveRaycastTry(Vector3 initPosFunc, Vector3 endPos, float speed)
    {
        while (initPosFunc != endPos)
        {
            
            newWallDirection =  Vector3.Lerp(initPosFunc, endPos, 
                speed * Time.deltaTime);
            //initPos = endPos;
            yield return null;
            
        }
        // initPosFunc = endPos;
    }


    public float SlowInterpolator()
    {
        float r = 0.0f;

        if (Input.GetKey(KeyCode.G))
        {
            r += 1;
            
        }
        
        //r += Input.GetAxis("K_MainHorizontal");
        inputSlowTest = r;
        
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public Vector3[] GravCast(Vector3 startPos, Vector3 direction, int killAfter, int velocity, bool reflect)
    {
         RaycastHit hit;
         Vector3[] vectors = new Vector3[killAfter];
         Ray ray = new Ray(startPos, direction);
         for (int i = 0; i < killAfter; i++)
         {
             if(Physics.Raycast(ray,out hit,1f))
             {
                 if (reflect)
                 {
                     print(hit.normal);
                    //  for (int e = 0; e < killAfter; e++)
                    //  {
                    //      if (Physics.Raycast(ray, out hit, 1f))
                    //      {
                    //          return vectors;
                    //      }
                    //      ray = new Ray(ray.origin + ray.direction, ray.direction + (Physics.gravity / killAfter / velocity));
                    //  }
                 }
                 return vectors;
             }
             Debug.DrawRay(ray.origin, ray.direction, Color.green);
            //  ray = new Ray(ray.origin + ray.direction, ray.direction + (Physics.gravity / killAfter / velocity));
            ray = new Ray(ray.origin + ray.direction, ray.direction + (newWallDirection / killAfter / velocity));
            vectors[i] = ray.origin;
 
         }
         return vectors;
    }

    //IEnumerator PullObject(Collider windObject, bool shouldPull)
	public void PullObject(Collider windObject)
	{
		
			Vector3 foreDirection =  windObject.transform.position - transform.position;
			//PhysicsA.AddForce(foreDirection.normalized * 30000 * Time.deltaTime);
			PhysicsA.velocity = PhysicsA.velocity + foreDirection.normalized  * 3300 * Time.deltaTime;
			//  yield return refreshRate;
			//  StartCoroutine(PullObject(objectAbsorbed,shouldPull));
		
	}


    public void PlayableController(bool dashButton, bool jumpButton, bool climbButton, float horizontalMoveButton, float verticalMoveButton, float elevationMoveButton)
    {
        DashButton = dashButton;
        JumpButton = jumpButton;
        ClimbButton = climbButton;
        HorizontalMoveButton = horizontalMoveButton;
        VerticalMoveButton = verticalMoveButton;
        ElevationMoveButton = elevationMoveButton;

    }

    private bool enableMovementOnNextTouch;
    
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;
    
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        PhysicsA.velocity = velocityToSet;

        //cam.DoFov(grappleFov);
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
        //cam.DoFov(85f);
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
