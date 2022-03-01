//Ironsights.cs by Azuline Studios© All Rights Reserved
//Adjusts weapon position and bobbing speeds and magnitudes 
//for various player states like zooming, sprinting, and crouching.
using UnityEngine;
using System.Collections;

public class Ironsights : MonoBehaviour {
	//Set up external script references
	[HideInInspector]
	public SmoothMouseLook SmoothMouseLook;
	private PlayerWeapons PlayerWeaponsComponent;
	private FPSRigidBodyWalker FPSWalker;
	[HideInInspector]
	public VerticalBob VerticalBob;
	[HideInInspector]
	public HorizontalBob HorizontalBob;
	private FPSPlayer FPSPlayerComponent;
	private InputControl InputComponent;
	[HideInInspector]
	public WeaponBehavior WeaponBehaviorComponent;
	[HideInInspector]
	public WeaponPivot WeaponPivotComponent;
	private Animation GunAnimationComponent;
	//other objects accessed by this script
	[HideInInspector]
	public GameObject playerObj;
	[HideInInspector]
	public GameObject weaponObj;
	[HideInInspector]
	public GameObject CameraObj;
	[HideInInspector]
	public Camera WeapCameraObj;
	//weapon object (weapon object child) set by PlayerWeapons.cs script
	[HideInInspector]
	public GameObject gunObj;
	//Var set to sprint animation time of weapon
	[HideInInspector]
	public Transform gun;//this set by PlayerWeapons script to active weapon transform
		
	//weapon positioning	
	private float zPosRecNext = 0.0f;//weapon recoil z position that is smoothed using smoothDamp function
	private float zPosRec = 0.0f;//target weapon recoil z position that is smoothed using smoothDamp function
	private float recZDamp = 0.0f;//velocity that is used by smoothDamp function
	private Vector3 nextPos;//weapon z position that is smoothed using smoothDamp function
	private Vector3 newPos;//target weapon z position that is smoothed using smoothDamp function
	private Vector3 bobPos;
	private Vector3 dampVel = Vector3.zero;//velocities that are used by smoothDamp function
	private Vector3 bobDampVel = Vector3.zero;
	private Vector3 tempGunPos = Vector3.zero;

	//camera FOV handling
	[Tooltip("Default camera field of view value.")]
	public float defaultFov = 75.0f;
	[Tooltip("Default camera field of view value while sprinting.")]
	public float sprintFov = 85.0f;
	[Tooltip("Amount to subtract from main camera FOV for weapon camera FOV.")]
	public float weaponCamFovDiff = 20.0f;//amount to subtract from main camera FOV for weapon camera FOV
	[HideInInspector]
	public float nextFov = 75.0f;//camera field of view that is smoothed using smoothDamp
	[HideInInspector]
	public float newFov = 75.0f;//camera field of view that is smoothed using smoothDamp
	private float FovSmoothSpeed = 0.15f;//speed that camera FOV is smoothed
	private float dampFOV = 0.0f;//target weapon z position that is smoothed using smoothDamp function
		
	//zooming
	public enum zoomType{
		hold,
		toggle,
		both
	}
	[HideInInspector]
	public bool dzAiming;//true if deadzone aiming
	[Tooltip("User may set zoom mode to toggle, hold, or both (toggle on zoom button press, hold on zoom button hold).")]
	public zoomType zoomMode = zoomType.both;
	public float zoomSensitivity = 0.5f;//percentage to reduce mouse sensitivity when zoomed
	public AudioClip sightsUpSnd;
	public AudioClip sightsDownSnd;
	[HideInInspector]
	public bool zoomSfxState = true;//var for only playing sights sound effects once
	[HideInInspector]
	public bool reloading = false;//this variable true when player is reloading
	
	[Header ("Bobbing Speeds and Amounts", order = 0)]
	[Space (10, order = 1)]
	[Tooltip("Amount to roll the screen left or right when strafing and sprinting.")]
	public float sprintStrafeRoll = 2.0f;
	[Tooltip("Amount to roll the screen left or right when strafing and walking.")]
	public float walkStrafeRoll = 1.0f;
	[Tooltip("Amount to roll the screen left or right when moving view horizontally.")]
	public float lookRoll = 1f;
	[Tooltip("Amount to roll the screen left or right when moving view horizontally during bullet time.")]
	public float btLookRoll = 1f;
	[Tooltip("Amount to roll the screen left or right when moving view horizontally and underwater.")]
	public float swimLookRoll = 1f;
	[Tooltip("Speed to return to neutral roll values when above water.")]
	public float rollReturnSpeed = 4.0f;
	[Tooltip("Speed to return to neutral roll values when underwater.")]
	public float rollReturnSpeedSwim = 2.0f;
	[Tooltip("Amount the camera should bob vertically to simulate player breathing.")]
	public float idleBobAmt = 1f;
	[Tooltip("Amount the camera should bob vertically to simulate floating in water.")]
	public float swimBobAmt = 1f;//true if camera should bob slightly up and down when player is swimming
	
	private float strafeSideAmt;//amount to move weapon left or right when strafing
	private float pivotBobAmt;
	
	//bobbing speeds and amounts for player movement states	
	public float WalkHorizontalBobAmount = 0.05f;
	public float WalkVerticalBobAmount = 0.11f;
	public float WalkBobPitchAmount = 0.0175f;
	public float WalkBobRollAmount = 0.01f;
	public float WalkBobYawAmount = 0.01f;
	public float WalkBobSpeed = 12f;
	
	public float CrouchHorizontalBobAmount = 0.075f;
	public float CrouchVerticalBobAmount = 0.11f;
	public float CrouchBobPitchAmount = 0.025f;
	public float CrouchBobRollAmount = 0.055f;
	public float CrouchBobYawAmount = 0.055f;
	public float CrouchBobSpeed = 8f;
	
	public float ProneHorizontalBobAmount = 0.25f;
	public float ProneVerticalBobAmount = 0.075f;
	public float ProneBobPitchAmount = 0.04f;
	public float ProneBobRollAmount = 0.03f;
	public float ProneBobYawAmount = 0.1f;
	public float ProneBobSpeed = 8f;
	
	public float ZoomHorizontalBobAmount = 0.016f;
	public float ZoomVerticalBobAmount = 0.0075f;
	public float ZoomBobPitchAmount = 0.001f;
	public float ZoomBobRollAmount = 0.008f;
	public float ZoomBobYawAmount = 0.008f;
	public float ZoomBobSpeed = 8f;
		
	public float SprintHorizontalBobAmount = 0.135f;
	public float SprintVerticalBobAmount = 0.16f;
	public float SprintBobPitchAmount = 0.12f;
	public float SprintBobRollAmount = 0.075f;
	public float SprintBobYawAmount = 0.075f;
	public float SprintBobSpeed = 19f;
		
	//gun X position amount for tweaking ironsights position
	private float horizontalGunPosAmt = -0.02f;
	private float weaponSprintXPositionAmt = 0.0f;
	//vars to scale up bob speeds slowly to prevent jerky transitions
	private float HBamt = 0.075f;//dynamic head bobbing variable
	private float HRamt = 0.125f;//dynamic head rolling variable
	private float HYamt = 0.125f;//dynamic head yawing variable
	private float HPamt = 0.1f;//dynamic head pitching variable
	private float GBamt = 0.075f;//dynamic gun bobbing variable
	//weapon sprinting positioning
	private float gunup = 0.015f;//amount to move weapon up while sprinting
	private float gunRunSide = 1.0f;//to control horizontal bobbing of weapon during sprinting
	private float gunRunUp = 1.0f;//to control vertical bobbing of weapon during sprinting
	private float sprintBob = 0.0f;//to modify weapon bobbing speeds when sprinting
	private float sprintBobAmtX = 0.0f;//actual horizontal weapon bobbing speed when sprinting
	private float sprintBobAmtY = 0.0f;//actual vertical weapon bobbing speed when sprinting
	//weapon positioning
	private float yDampSpeed= 0.0f;//this value used to control speed that weapon Y position is smoothed
	private float yDampSpeedAmt;
	private float zDampSpeed= 0.0f;//this value used to control speed that weapon Z position is smoothed
	public float bobDampSpeed = 0.1f;
	private float bobDir = 0.0f;//positive or negative direction of bobbing
	private float bobMove = 0.0f;
	private float sideMove = 0.0f;
	[HideInInspector]
	public float switchMove = 0.0f;//for moving weapon down while switching weapons
	[HideInInspector]
	public float climbMove = 0.0f;//for moving weapon down while climbing
	private float jumpmove = 0.0f;//for moving weapon down while jumping
	[HideInInspector]
	public float jumpAmt = 0.0f;
	private float idleX = 0.0f;//amount of weapon movement when idle
	private float idleY = 0.0f;
	[HideInInspector]
	public float side = 0.0f;//amount to sway weapon position horizontally
	[HideInInspector]
	public float raise = 0.0f;//amount to sway weapon position vertically
	[HideInInspector]
	public float gunAnimTime = 0.0f;
	
	private float proneSwayX;
	private float proneSwayY;

	private AudioSource aSource;
	[Tooltip("Point to rotate weapon models for vertical bobbing effect.")]
	public Transform pivot;
	private float pivotAmt;
	private float dampVel2;
	private float rotateAmtNeutral;
	
	[HideInInspector]
	public float gunPosBack;
	[HideInInspector]
	public float gunPosUp;
	[HideInInspector]
	public float gunPosSide;

	private bool zoomInit;

	[Header ("Perlin Noise For Bobbing Effects")]
	[Space (10)]
	[Tooltip("True if Perlin Noise generation should be added to weapon and camera bobbing effects to add variation to bobbing (shaky cam effect).")]
	public bool usePerlinNoise = true;
	
	[Tooltip("Amount of Perlin Noise to add in to camera bobbing (pitch, yaw, roll) for (walking, zoomed, crouched, prone, sprinting).")]
	public Vector3[] CamBobAmts;
	[HideInInspector]
	public Vector3 camBobPerlinAmt;
	[Tooltip("Ratio of Perlin Noise to sine wave bobbing, based on player state (walking, zoomed, crouched, prone, sprinting).")]
	public float[] perlinFactors;
	private float weaponPerlinAmt;//Amount to scale Perlin Noise in bobbing per weapon for player state (unzoomed, zoomed, sprinting).
	[Tooltip("Scale of Perlin Noise generation (walking, zoomed, crouched, prone, sprinting).")]
	public float[] perlinScales;
	[Tooltip("Speed of Perlin Noise cycle (walking, zoomed, crouched, prone, sprinting).")]
	public float[] perlinSpeeds;
	
	private float perlinScaleAmt;
	private float perlinSpeedAmt;
	
	[Tooltip("Percentage of 1 which Perlin Noise will generate in. Used to scale up or down Perlin Noise on bobbing effects.")]
	[Range(0.0f, 1.0f)]
	private float perlinbobAmt;
	[Tooltip("Vertical Perlin Noise value (not editable, for visual reference when tweaking Perlin speed and scale values).")]
	[Range(0.0f, 2.5f)]
	public float perlinUnscaledV;
	[Tooltip("Horizontal Perlin Noise value (not editable, for visual reference when tweaking Perlin speed and scale values).")]
	[Range(0.0f, 2.5f)]
	public float perlinUnscaledH;
	[Tooltip("Vertical Perlin Noise value scaled by Perlin Factor (not editable, for visual reference when tweaking Perlin speed and scale values).")]
	[Range(0.0f, 2.5f)]
	public float perlinHeight;
	[Tooltip("Horizontal Perlin Noise value scaled by Perlin Factor (not editable, for visual reference when tweaking Perlin speed and scale values).")]
	[Range(0.0f, 2.5f)]
	public float perlinWidth;

	void Start(){
		//Set up external script references
		SmoothMouseLook = CameraObj.GetComponent<SmoothMouseLook>();
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		FPSWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		VerticalBob = playerObj.GetComponent<VerticalBob>();
		HorizontalBob = playerObj.GetComponent<HorizontalBob>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		InputComponent = playerObj.GetComponent<InputControl>();
		WeaponPivotComponent = FPSPlayerComponent.WeaponPivotComponent;

		tempGunPos = Vector3.zero;

		aSource = playerObj.AddComponent<AudioSource>(); 
		aSource.spatialBlend = 0.0f;
		aSource.playOnAwake = false;
	}
	
	void Update (){
		
		if(Time.timeScale > 0.0f && Time.deltaTime > 0.0f && Time.smoothDeltaTime > 0.0f){//allow pausing by setting timescale to 0

			GunAnimationComponent = gunObj.GetComponent<Animation>();

			if(usePerlinNoise){//scale Perlin bobbing based on player state
				if(FPSPlayerComponent.zoomed){					
					perlinScaleAmt = perlinScales[1];
					perlinSpeedAmt = perlinSpeeds[1];
					perlinbobAmt = perlinFactors[1];
					camBobPerlinAmt = CamBobAmts[1];
					weaponPerlinAmt = WeaponBehaviorComponent.perlinBobAmts[1];
					
				}else{
					if(FPSWalker.sprintActive){
						perlinScaleAmt = perlinScales[4];
						perlinSpeedAmt = perlinSpeeds[4];
						perlinbobAmt = perlinFactors[4];
						camBobPerlinAmt = CamBobAmts[4];
						weaponPerlinAmt = WeaponBehaviorComponent.perlinBobAmts[4];
					}else{
						if(FPSWalker.crouched){
							perlinbobAmt = perlinFactors[2];
							camBobPerlinAmt = CamBobAmts[2];
							perlinScaleAmt = perlinScales[2];
							perlinSpeedAmt = perlinSpeeds[2];
							weaponPerlinAmt = WeaponBehaviorComponent.perlinBobAmts[2];
						}else if(FPSWalker.prone){
							perlinbobAmt = perlinFactors[3];
							camBobPerlinAmt = CamBobAmts[3];							
							perlinScaleAmt = perlinScales[3];
							perlinSpeedAmt = perlinSpeeds[3];
							weaponPerlinAmt = WeaponBehaviorComponent.perlinBobAmts[3];
						}else{
							perlinbobAmt = perlinFactors[0];
							camBobPerlinAmt = CamBobAmts[0];							
							perlinScaleAmt = perlinScales[0];
							perlinSpeedAmt = perlinSpeeds[0];
							weaponPerlinAmt = WeaponBehaviorComponent.perlinBobAmts[0];
						}
					}
				}
				//calculate Perlin noise for bobbing
				perlinUnscaledV = perlinScaleAmt * Mathf.PerlinNoise(0.0f, Time.time * perlinSpeedAmt);
				perlinUnscaledH = perlinScaleAmt * Mathf.PerlinNoise(Time.time * perlinSpeedAmt, 0.0f);
				//calculate percentage of Perlin noise to sine wave bobbing
				perlinHeight = perlinUnscaledV * perlinbobAmt + (1f - perlinbobAmt);
				perlinWidth = perlinUnscaledH * perlinbobAmt + (1f - perlinbobAmt);
			}else{
				weaponPerlinAmt = 1.0f;
				camBobPerlinAmt = Vector3.one;
				perlinHeight = 1.0f;
				perlinWidth = 1.0f;
			}

			float deltaAmount = Time.smoothDeltaTime * 100;//define delta for framerate independence
			float bobDeltaAmount = 0.12f / Time.smoothDeltaTime;//define bobbing delta for framerate independence

			yDampSpeedAmt = Mathf.MoveTowards(yDampSpeedAmt, yDampSpeed, Time.deltaTime);

			bobPos.x = Mathf.SmoothDamp(bobPos.x, 
			    (((HorizontalBob.translateChange / sprintBobAmtX * proneSwayX ) * (perlinWidth * weaponPerlinAmt) * bobDeltaAmount) * gunRunSide * bobDir), 
				ref bobDampVel.x, yDampSpeedAmt, Mathf.Infinity, Time.deltaTime);
			
			bobPos.y = Mathf.SmoothDamp(bobPos.y, 
			    (((VerticalBob.translateChange / sprintBobAmtY * proneSwayY ) * (perlinWidth * weaponPerlinAmt)* bobDeltaAmount) * gunRunUp * bobDir), 
				ref bobDampVel.y, yDampSpeedAmt, Mathf.Infinity, Time.deltaTime);
				
			if(SmoothMouseLook.playerMovedTime + 0.1 < Time.time || FPSWalker.moving){
				//main weapon position smoothing happens here
				newPos.x = Mathf.SmoothDamp(newPos.x, nextPos.x, ref dampVel.x, yDampSpeedAmt, Mathf.Infinity, Time.deltaTime);
				newPos.y = Mathf.SmoothDamp(newPos.y, nextPos.y, ref dampVel.y, yDampSpeedAmt, Mathf.Infinity, Time.deltaTime);
				newPos.z = Mathf.SmoothDamp(newPos.z, nextPos.z, ref dampVel.z, zDampSpeed, Mathf.Infinity, Time.deltaTime);
				zPosRec = Mathf.SmoothDamp(zPosRec, zPosRecNext, ref recZDamp, 0.25f, Mathf.Infinity, Time.deltaTime);//smooth recoil kick back of weapon
			}else{
				//main weapon position smoothing happens here
				newPos.x = nextPos.x;
				newPos.y = nextPos.y;
				newPos.z = nextPos.z;
				zPosRec = zPosRecNext;
			}
			
			newFov = Mathf.SmoothDamp(Camera.main.fieldOfView, nextFov, ref dampFOV, FovSmoothSpeed, Mathf.Infinity, Time.deltaTime);//smooth camera FOV
			
			//Sync camera FOVs
			if(WeapCameraObj){
				WeapCameraObj.fieldOfView = Camera.main.fieldOfView - weaponCamFovDiff;
			}
			Camera.main.fieldOfView = newFov;
			//Get input from player movement script
			float horizontal = FPSWalker.inputX;
			float vertical = FPSWalker.inputY;
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Adjust weapon position and bobbing amounts dynamicly based on movement and player states
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//move weapon back towards camera based on kickBack amount in WeaponBehavior.cs					
			if(WeaponBehaviorComponent.shootStartTime + 0.1f > Time.time){
				if(FPSPlayerComponent.zoomed){
					zPosRecNext = WeaponBehaviorComponent.kickBackAmtZoom;	
				}else{
					zPosRecNext = WeaponBehaviorComponent.kickBackAmtUnzoom;	
				}
			}else{
				if(WeaponBehaviorComponent.pullBackAmt != 0.0f){
					zPosRecNext = WeaponBehaviorComponent.pullBackAmt * WeaponBehaviorComponent.fireHoldMult;
				}else{
					zPosRecNext = 0.0f;
				}
			}
				
			if(FPSWalker.moving){
				idleY = 0;
				idleX = 0;
				//check for sprinting or prone
				if(
					(
						(//player has stood up from crouch or prone and is sprinting
							FPSWalker.sprintActive
							&& !FPSWalker.crouched
							&& !FPSWalker.cancelSprint
							&& (FPSWalker.midPos >= FPSWalker.standingCamHeight && FPSWalker.proneRisen)	
						) 
						|| (!reloading && (!FPSWalker.proneRisen && !FPSWalker.crouched) 
					    && (FPSWalker.prone || FPSWalker.sprintActive))//player is prone
					)
				&& FPSWalker.fallingDistance < 0.75f	
				&& !FPSPlayerComponent.zoomed
				&& !FPSWalker.jumping
				){
					
					sprintBob = 128.0f;
					
					if (!FPSWalker.cancelSprint
					&& (!reloading || FPSWalker.sprintReload)
					&& FPSWalker.fallingDistance < 0.75f
					&& !FPSWalker.jumping){//actually sprinting now
						//set the camera's fov back to normal if the player has sprinted into a wall, but the sprint is still active
						if(((FPSWalker.inputY != 0 && FPSWalker.forwardSprintOnly) || (!FPSWalker.forwardSprintOnly && FPSWalker.moving))
						&& !FPSWalker.prone){
							nextFov = sprintFov;
						}else{
							nextFov = defaultFov;	
						}
						
						if(!reloading){
							//gradually move weapon more towards center while sprinting
							weaponSprintXPositionAmt = Mathf.MoveTowards(weaponSprintXPositionAmt, WeaponBehaviorComponent.weaponSprintXPosition, Time.deltaTime * 16);
							horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition + weaponSprintXPositionAmt;
							gunRunSide = 2.0f;
							
							if(gunRunUp < 1.4f){gunRunUp += Time.deltaTime / 4.0f;}//gradually increase for smoother transition
							bobMove = gunup + WeaponBehaviorComponent.weaponSprintYPosition;//raise weapon while sprinting
							sideMove = 0.0f;
						}else{//weapon positioning for sprinting and reloading
							gunRunSide = 1.0f;
							gunRunUp = 1.0f;
							sprintBob = 216;
							bobMove = 0.0f;
							sideMove = 0.0f;
							weaponSprintXPositionAmt = Mathf.MoveTowards(weaponSprintXPositionAmt, 0, Time.deltaTime * 16);
							horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition + weaponSprintXPositionAmt;
						}
						
					}else{//not sprinting
						nextFov = defaultFov;
						gunRunSide = 1.0f;
						gunRunUp = 1.0f;
						bobMove = -0.01f;
						//make this check to prevent weapon occasionally not lowering during switch while prone and moving 
						if(!FPSWalker.prone){
							switchMove = 0.0f;
						}
					}
				}else{//walking
					gunRunSide = 1.0f;
					gunRunUp = 1.0f;
					//reset horizontal weapon positioning var and make sure it returns to zero when not sprinting to prevent unwanted side movement
					weaponSprintXPositionAmt = Mathf.MoveTowards(weaponSprintXPositionAmt, 0, Time.deltaTime * 16);
					horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition + weaponSprintXPositionAmt;
					if(reloading){//move weapon position up when reloading and moving for full view of animation
						nextFov = defaultFov;
						sprintBob = 216;
						bobMove = 0.0f;
						sideMove = 0.0f;
					}else{
						nextFov = defaultFov;
						if(FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0) {//zoomed and not melee weapon
							sprintBob = 96.0f;
							bobMove = 0.0F;//move weapon to idle
						}else{//not zoomed
							sprintBob = 216.0f;
							if(FPSWalker.moving){
								//move weapon down and left when crouching
								if (FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f) {
									bobMove = WeaponBehaviorComponent.crouchWalkYPosition;
									sideMove = -0.0125f;
								}else{
									bobMove = WeaponBehaviorComponent.weaponWalkYPosition;
									sideMove = 0.00f;
								}
							}else{
								//move weapon to idle
								bobMove = 0.0F;
								sideMove = 0.0F;
							}
						}
					}
				}
			}else{//if not moving (no player movement input)
				nextFov = defaultFov;
				horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition;
				if(weaponSprintXPositionAmt > 0){weaponSprintXPositionAmt -= Time.deltaTime / 4;}
				sprintBob = 96.0f;
				if(reloading){
					nextFov = defaultFov;
					sprintBob = 96.0f;
					bobMove = 0.0F;
					sideMove = 0.0f;
				}else{
					//move weapon to idle
					if((FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f) && !FPSPlayerComponent.zoomed) {
						bobMove = WeaponBehaviorComponent.weaponCrouchYPosition;
						sideMove = -0.0125f;
					}else{
						bobMove = 0.0f;
						sideMove = 0.0f;
					}
				}
				//weapon idle motion
				if(FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0) {
					idleX = Mathf.Sin(Time.time * 1.25f) * 0.0005f * WeaponBehaviorComponent.zoomIdleSwayAmt;
					idleY = Mathf.Sin(Time.time * 1.5f) * 0.0005f * WeaponBehaviorComponent.zoomIdleSwayAmt;
				}else{
					if(!FPSWalker.swimming){
						idleX = Mathf.Sin(Time.time * 1.25f) * 0.0012f * WeaponBehaviorComponent.idleSwayAmt;
						idleY = Mathf.Sin(Time.time * 1.5f) * 0.0012f * WeaponBehaviorComponent.idleSwayAmt;
					}else{
						idleX = Mathf.Sin(Time.time * 1.25f) * 0.003f * WeaponBehaviorComponent.swimIdleSwayAmt;
						idleY = Mathf.Sin(Time.time * 1.5f) * 0.003f * WeaponBehaviorComponent.swimIdleSwayAmt;	
					}
				}
			}
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Weapon Swaying/Bobbing while moving
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//track X axis while walking for side to side bobbing effect	
			if(HorizontalBob.waveslice != 0){bobDir = 1;}else{bobDir = -1;}
				
			//Reduce weapon bobbing by sprintBobAmount value defined in WeaponBehavior script.
			//This is for fine tuning of weapon bobbing. Pistols look better with less sprint bob 
			//because they use a different sprinting anim and have a different sprinting position 
			//than the animation used by rifle-type weapons.
			if(!FPSWalker.canRun){
				sprintBobAmtX = Mathf.MoveTowards(sprintBobAmtX, sprintBob / WeaponBehaviorComponent.walkBobAmountX, Time.smoothDeltaTime * 128.0f);
				sprintBobAmtY = Mathf.MoveTowards(sprintBobAmtY, sprintBob / WeaponBehaviorComponent.walkBobAmountY, Time.smoothDeltaTime * 128.0f);
			}else{
				sprintBobAmtX = Mathf.MoveTowards(sprintBobAmtX, sprintBob / WeaponBehaviorComponent.sprintBobAmountX, Time.smoothDeltaTime * 128.0f);
				sprintBobAmtY = Mathf.MoveTowards(sprintBobAmtY, sprintBob / WeaponBehaviorComponent.sprintBobAmountY, Time.smoothDeltaTime * 128.0f);
			}
			
			//increase weapon bobbing amounts for prone
			if(FPSWalker.allowProne
			&& FPSWalker.midPos <= FPSWalker.crouchingCamHeight 
			&& FPSWalker.prone 
			&& FPSWalker.moving){
				if(!WeaponBehaviorComponent.PistolSprintAnim){
					proneSwayX = 3.5f;//x
					proneSwayY = 4.5f;//y
				}else{//less prone bobbing for pistol-type weapons
					proneSwayX = 1.5f;
					proneSwayY = 2.5f;	
				}
			}else{
				proneSwayX = 1.0f;
				proneSwayY = 1.0f;
			}
				
			//set smoothed weapon position to actual gun position vector
			tempGunPos.x = newPos.x + bobPos.x;
			tempGunPos.y = newPos.y + bobPos.y;
			tempGunPos.z = newPos.z + zPosRec;//add weapon z position and recoil kick back

			if(!WeaponBehaviorComponent.unarmed && Time.timeSinceLevelLoad > 0.3f){
				//apply temporary vector to gun's transform position
				gun.localPosition = tempGunPos;
			}
			
			if(gun.transform.parent.transform.localEulerAngles.x < 180.0f){
				rotateAmtNeutral = -gun.transform.parent.transform.localEulerAngles.x;
			}else{
				rotateAmtNeutral = 360.0f - gun.transform.parent.transform.localEulerAngles.x;
			}
			
			//compensate for floating point imprecision in RotateAround when player is a large distance from scene origin
			gun.transform.parent.transform.localPosition = Vector3.MoveTowards(gun.transform.parent.transform.localPosition, Vector3.zero, 0.005f * Time.smoothDeltaTime);
				
			if(!FPSPlayerComponent.zoomed || WeaponPivotComponent.deadzoneZooming){
				if(WeaponPivotComponent.deadzoneZooming && FPSPlayerComponent.zoomed){
					pivotBobAmt = WeaponBehaviorComponent.pivotBob * 0.5f;
				}else{
					pivotBobAmt = WeaponBehaviorComponent.pivotBob;
				}
			}else{
				pivotBobAmt = 0.0f;
			}
			
			deltaAmount = Time.deltaTime * 75.0f;//define delta for framerate independence (default values were set @ 75Hz monitor refresh rate)

			//rotate weapon vertically along pivot for bobbing effect
			pivotAmt = Mathf.SmoothDampAngle(pivotAmt, ((-WeaponPivotComponent.mouseOffsetTarg.x * pivotBobAmt) 
													   + (WeaponPivotComponent.animOffsetTarg.x)) * deltaAmount, ref dampVel2, Time.deltaTime);
			gun.transform.parent.transform.RotateAround(pivot.position, gun.transform.parent.transform.right, pivotAmt + ((rotateAmtNeutral * 0.2f) * deltaAmount));

			deltaAmount = Time.smoothDeltaTime * 100.0f;//define delta for framerate independence (default values were set @ 100Hz monitor refresh rate, will change later)
			bobDeltaAmount = 0.12f / Time.deltaTime;//define bobbing delta for framerate independence
			
		
			//lower weapon when jumping, falling, or slipping off ledge
			if(FPSWalker.jumping || FPSWalker.fallingDistance > 1.25f){
				//lower weapon less when zoomed
				if (!FPSPlayerComponent.zoomed){
					//raise weapon when jump is ascending and lower when descending
					if((FPSWalker.airTime + 0.175f) > Time.time){
						jumpmove = 0.015f;
					}else{
						jumpmove = -0.025f;
					}
				}else{
					jumpmove = -0.01f;
				}
			}else{
				jumpmove = 0.0f;
			}
		   
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Adjust vars for zoom and other states
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		  	
			if(!WeaponBehaviorComponent.PistolSprintAnim || !FPSWalker.canRun){
				gunAnimTime = GunAnimationComponent["RifleSprinting"].normalizedTime;//Track playback position of rifle sprinting animation
			}else{
				gunAnimTime = GunAnimationComponent["PistolSprinting"].normalizedTime;//Track playback position of pistol sprinting animation	
			}
		   
			//if zoomed
			//check time of weapon sprinting anim to make weapon return to center, then zoom normally 
			if((FPSPlayerComponent.zoomed || (FPSPlayerComponent.canBackstab && !WeaponBehaviorComponent.shooting))
			&& FPSPlayerComponent.hitPoints > 1.0f
			&& PlayerWeaponsComponent.switchTime + WeaponBehaviorComponent.readyTime < Time.time//don't raise sights when readying weapon 
			&& !reloading 
			&& gunAnimTime < 0.35f 
			//&& WeaponBehaviorComponent.meleeSwingDelay == 0//not a melee weapon
			&& PlayerWeaponsComponent.currentWeapon != 0
			//move weapon to zoom values if zoomIsBlock is true, also
			&& (FPSPlayerComponent.canBackstab || (((WeaponBehaviorComponent.zoomIsBlock && ((WeaponBehaviorComponent.shootStartTime + WeaponBehaviorComponent.fireRate < Time.time && !WeaponBehaviorComponent.shootFromBlock) || WeaponBehaviorComponent.shootFromBlock)) || !WeaponBehaviorComponent.zoomIsBlock)))
			&& WeaponBehaviorComponent.reloadLastStartTime + WeaponBehaviorComponent.reloadLastTime < Time.time){

				if(!zoomInit){
					HorizontalBob.translateChange = 0.0f;
					VerticalBob.translateChange = 0.0f;
					zoomInit = true;
				}

				if(!dzAiming){
					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideZoom, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16);
					}
					if(!FPSPlayerComponent.canBackstab){
						//X pos with idle movement
						nextPos.x = WeaponBehaviorComponent.weaponZoomXPosition + (side / 1.5f) + idleX + gunPosSide + (FPSWalker.inputX * 0.1f * strafeSideAmt);
						//Y pos with idle movement
						nextPos.y = WeaponBehaviorComponent.weaponZoomYPosition + (raise / 1.5f) + idleY + (bobMove + switchMove + climbMove + jumpAmt + jumpmove + gunPosUp); 
						//Z pos
						nextPos.z = WeaponBehaviorComponent.weaponZoomZPosition + gunPosBack;
					}else{
						//X pos with idle movement
						nextPos.x = WeaponBehaviorComponent.weaponBackstabXPosition + (side / 1.5f) + idleX + gunPosSide + (FPSWalker.inputX * 0.1f * strafeSideAmt);
						//Y pos with idle movement
						nextPos.y = WeaponBehaviorComponent.weaponBackstabYPosition + (raise / 1.5f) + idleY + (bobMove + switchMove + climbMove + jumpAmt + jumpmove + gunPosUp); 
						//Z pos
						nextPos.z = WeaponBehaviorComponent.weaponBackstabZPosition + gunPosBack;
					}

					nextFov = WeaponBehaviorComponent.zoomFOV;

					//If not a melee weapon, play sound effect when raising sights
					if(zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0 && !WeaponBehaviorComponent.unarmed){
						aSource.clip = sightsUpSnd;
						aSource.volume = 1.0f;
						aSource.pitch = 1.0f * Time.timeScale;
						aSource.Play();
						zoomSfxState = false;
					}
				}else{//zooming with deadzone (goldeneye/perfect dark style)
					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideZoom, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16);
					}
					//X pos with idle movement
					nextPos.x = side + idleX + gunPosSide + sideMove + horizontalGunPosAmt + (FPSWalker.leanAmt / 60.0f) + (FPSWalker.inputX * 0.1f * strafeSideAmt);
					//Y pos with idle movement
					nextPos.y = raise + idleY + (bobMove + climbMove + switchMove + jumpAmt + jumpmove + gunPosUp) + WeaponBehaviorComponent.weaponUnzoomYPosition;
					//Z pos
					nextPos.z = WeaponBehaviorComponent.weaponUnzoomZPosition + gunPosBack;
					nextFov = WeaponBehaviorComponent.zoomFOVDz;
				}

				//adjust FOV and weapon position for zoom
				FovSmoothSpeed = 0.09f;//faster FOV zoom speed when zooming in
				yDampSpeed = 0.09f;
				zDampSpeed = 0.15f;
				
				if(!FPSPlayerComponent.canBackstab){
					//slow down turning and movement speed for zoom
					FPSWalker.zoomSpeed = true;
				
					if(!WeaponBehaviorComponent.zoomIsBlock){
						//Reduce mouse sensitivity when zoomed, but maintain sensitivity set in SmoothMouseLook script
						SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity * WeaponBehaviorComponent.zoomSensitivity;
					}
					
					//Gradually increase or decrease bobbing amounts for smooth transitions between movement states
						
					////zoomed bobbing amounts////
					//horizontal bobbing 
					GBamt = Mathf.MoveTowards(GBamt, ZoomHorizontalBobAmount, Time.smoothDeltaTime);
					//vertical bobbing	
					HBamt = Mathf.MoveTowards(HBamt, ZoomVerticalBobAmount, Time.smoothDeltaTime);
					//pitching	
					HPamt = Mathf.MoveTowards(HPamt, ZoomBobPitchAmount, Time.smoothDeltaTime);
					//rolling	
					HRamt = Mathf.MoveTowards(HRamt, ZoomBobRollAmount, Time.smoothDeltaTime);
					//yawing	
					HYamt = Mathf.MoveTowards(HYamt, ZoomBobYawAmount, Time.smoothDeltaTime);
					
					if(!FPSWalker.swimming){
						//Set bobbing speeds and amounts in other scripts to these smoothed values
						VerticalBob.bobbingSpeed = ZoomBobSpeed;
						//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
						HorizontalBob.bobbingSpeed = ZoomBobSpeed / 2.0f;
					}else{
						//Set bobbing speeds and amounts in other scripts to these smoothed values
						VerticalBob.bobbingSpeed = ZoomBobSpeed / 2.0f;
						//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
						HorizontalBob.bobbingSpeed = ZoomBobSpeed / 4.0f;	
					}
				}
				VerticalBob.bobbingAmount = HBamt * deltaAmount;//apply delta at this step for framerate independence
				VerticalBob.rollingAmount = HRamt * deltaAmount;
				VerticalBob.yawingAmount = HYamt * deltaAmount;
				VerticalBob.pitchingAmount = HPamt * deltaAmount;
				HorizontalBob.bobbingAmount = GBamt * deltaAmount;
				
			}else{//not zoomed

				if(zoomInit){
					HorizontalBob.translateChange = 0.0f;
					VerticalBob.translateChange = 0.0f;
					zoomInit = false;
				}
				
				FovSmoothSpeed = 0.18f;//slower FOV zoom speed when zooming out
				
				//adjust weapon Y position smoothing speed for unzoom and switching weapons
				if(!PlayerWeaponsComponent.switching){
					yDampSpeed = 0.18f;//weapon swaying speed
				}else{
					yDampSpeed = 0.2f;//weapon switch raising speed
				}
				zDampSpeed = 0.1f;
				//X pos with idle movement
				nextPos.x = side + idleX + gunPosSide + sideMove + horizontalGunPosAmt + (FPSWalker.leanAmt / 60.0f) + (FPSWalker.inputX * 0.1f * strafeSideAmt);
				//Y pos with idle movement
				nextPos.y = raise + idleY + (bobMove + climbMove + switchMove + jumpAmt + jumpmove + gunPosUp) + WeaponBehaviorComponent.weaponUnzoomYPosition;
				//Z pos
				if(!FPSWalker.prone){
					nextPos.z = WeaponBehaviorComponent.weaponUnzoomZPosition + (((HorizontalBob.translateChange * bobDeltaAmount) * bobDir) * 0.003f * WeaponBehaviorComponent.zBobWalk) + gunPosBack;
				}else{
					nextPos.z = WeaponBehaviorComponent.weaponUnzoomZPosition + gunPosBack;
				}
				//Set turning and movement speed for unzoom
				FPSWalker.zoomSpeed = false;	
				//If not a melee weapon, play sound effect when lowering sights	
				if(!zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0 && !WeaponBehaviorComponent.unarmed){
					aSource.clip = sightsDownSnd;
					aSource.volume = 1.0f;
					aSource.pitch = 1.0f * Time.timeScale;
					aSource.Play();
					zoomSfxState = true;
				}
				//Return mouse sensitivity to normal
				SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity;
				
				//Set weapon and view bobbing amounts
				if (FPSWalker.sprintActive
				&& !(FPSWalker.forwardSprintOnly && (Mathf.Abs(horizontal) != 0.0f) && (Mathf.Abs(vertical) < 0.75f))
				&& (Mathf.Abs(vertical) != 0.0f || (!FPSWalker.forwardSprintOnly && FPSWalker.moving))
				&& !FPSWalker.cancelSprint
				&& !FPSWalker.crouched
				&& !FPSWalker.prone
				&& FPSWalker.midPos >= FPSWalker.standingCamHeight
				&& !FPSPlayerComponent.zoomed
				&& !InputComponent.fireHold){

					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideSprint, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16);
					}
					
					//scale up bob speeds slowly to prevent jerky transition
					if (FPSWalker.grounded){
						////sprinting bobbing amounts////
						//horizontal bobbing 
						GBamt = Mathf.MoveTowards(GBamt, SprintHorizontalBobAmount, Time.smoothDeltaTime);
						//vertical bobbing
						HBamt = Mathf.MoveTowards(HBamt, SprintVerticalBobAmount, Time.smoothDeltaTime);
						//pitching
						HPamt = Mathf.MoveTowards(HPamt, SprintBobPitchAmount, Time.smoothDeltaTime);
						//rolling
						HRamt = Mathf.MoveTowards(HRamt, SprintBobRollAmount, Time.smoothDeltaTime);
						//yawing
						HYamt = Mathf.MoveTowards(HYamt, SprintBobYawAmount, Time.smoothDeltaTime);
						
					}else{
						//reduce bobbing amounts for smooth jumping/landing transition
						HBamt = Mathf.MoveTowards(HBamt, 0f, Time.smoothDeltaTime);
						HRamt = Mathf.MoveTowards(HRamt, 0f, Time.smoothDeltaTime * 2f);
						HYamt = Mathf.MoveTowards(HYamt, 0f, Time.smoothDeltaTime * 2f);
						HPamt = Mathf.MoveTowards(HPamt, 0f, Time.smoothDeltaTime * 2f);
						GBamt = Mathf.MoveTowards(GBamt, 0f, Time.smoothDeltaTime);
					}
					//Set bobbing speeds and amounts in other scripts to these smoothed values
					VerticalBob.bobbingSpeed = SprintBobSpeed;
					//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
					HorizontalBob.bobbingSpeed = SprintBobSpeed / 2.0f;
					HorizontalBob.bobbingAmount = GBamt * deltaAmount;//apply delta at this step for framerate independence
					VerticalBob.rollingAmount = HRamt * deltaAmount;
					VerticalBob.yawingAmount = HYamt * deltaAmount;
					VerticalBob.pitchingAmount = HPamt * deltaAmount;
					VerticalBob.bobbingAmount = HBamt * deltaAmount;
					if(!reloading){
						//move weapon toward or away from camera while sprinting
						nextPos.z = WeaponBehaviorComponent.weaponSprintZPosition + (((HorizontalBob.translateChange * bobDeltaAmount) * bobDir) * 0.003f * WeaponBehaviorComponent.zBobSprint) + gunPosBack;
					}

				}else{

					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideUnzoom, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16);
					}
					
					//scale up bob speeds slowly to prevent jerky transition
					if (FPSWalker.grounded) {
						if (!FPSWalker.crouched && !FPSWalker.prone && FPSWalker.midPos >= FPSWalker.standingCamHeight){
							////walking bob amounts///
							//horizontal bobbing 
							GBamt = Mathf.MoveTowards(GBamt, WalkVerticalBobAmount, Time.smoothDeltaTime);
							//vertical bobbing
							HBamt = Mathf.MoveTowards(HBamt, WalkHorizontalBobAmount, Time.smoothDeltaTime);
							//pitching
							HPamt = Mathf.MoveTowards(HPamt, WalkBobPitchAmount, Time.smoothDeltaTime);
							//rolling
							if(!FPSWalker.swimming){
								HRamt = Mathf.MoveTowards(HRamt, WalkBobRollAmount, Time.smoothDeltaTime);
							}else{
								HRamt = Mathf.MoveTowards(HRamt, WalkBobRollAmount * 2.0f, Time.smoothDeltaTime);	
							}
							//yawing
							HYamt = Mathf.MoveTowards(HYamt, WalkBobYawAmount, Time.smoothDeltaTime);	
										
							if(!FPSWalker.swimming){
								VerticalBob.bobbingSpeed = WalkBobSpeed;
								//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
								HorizontalBob.bobbingSpeed = WalkBobSpeed / 2.0f;
							}else{//bobbing is slower while swimming
								VerticalBob.bobbingSpeed = WalkBobSpeed/2;
								HorizontalBob.bobbingSpeed = WalkBobSpeed / 4.0f;
							}
						}else{
							if(FPSWalker.crouched){
								////crouching bob amounts////
								//horizontal bobbing 
								GBamt = Mathf.MoveTowards(GBamt, CrouchVerticalBobAmount, Time.smoothDeltaTime);
								//vertical bobbing
								HBamt = Mathf.MoveTowards(HBamt, CrouchHorizontalBobAmount, Time.smoothDeltaTime);
								//pitching
								HPamt = Mathf.MoveTowards(HPamt, CrouchBobPitchAmount, Time.smoothDeltaTime);
								//rolling
								HRamt = Mathf.MoveTowards(HRamt, CrouchBobRollAmount, Time.smoothDeltaTime);
								//yawing
								HYamt = Mathf.MoveTowards(HYamt, CrouchBobYawAmount, Time.smoothDeltaTime);
					
								VerticalBob.bobbingSpeed = CrouchBobSpeed;
								HorizontalBob.bobbingSpeed = CrouchBobSpeed / 2.0f;
							}else if(FPSWalker.prone){
								////prone bob amounts////
								//horizontal bobbing 
								HBamt = Mathf.MoveTowards(HBamt, ProneHorizontalBobAmount, Time.smoothDeltaTime);
								//vertical bobbing
								GBamt = Mathf.MoveTowards(GBamt, ProneVerticalBobAmount, Time.smoothDeltaTime);
								//pitching
								HPamt = Mathf.MoveTowards(HPamt, ProneBobPitchAmount, Time.smoothDeltaTime);
								//rolling
								HRamt = Mathf.MoveTowards(HRamt, ProneBobRollAmount, Time.smoothDeltaTime);
								//yawing
								HYamt = Mathf.MoveTowards(HYamt, ProneBobYawAmount, Time.smoothDeltaTime);
					
								VerticalBob.bobbingSpeed = ProneBobSpeed;
								HorizontalBob.bobbingSpeed = ProneBobSpeed / 2.0f;
								//move weapon toward or away from camera while prone
//								if((Mathf.Abs(horizontal) > 0.15f) || (Mathf.Abs(vertical) > 0.15f)){
//									nextPos.z = 0.02f;
//								}else{
//									nextPos.z = 0.0f;
//								}
							}
						}
					}else{
						//reduce bobbing amounts for smooth jumping/landing transition
						HBamt = Mathf.MoveTowards(HBamt, 0f, Time.smoothDeltaTime);
						HRamt = Mathf.MoveTowards(HRamt, 0f, Time.smoothDeltaTime * 2f);
						HYamt = Mathf.MoveTowards(HYamt, 0f, Time.smoothDeltaTime * 2f);
						HPamt = Mathf.MoveTowards(HPamt, 0f, Time.smoothDeltaTime * 2f);
						GBamt = Mathf.MoveTowards(GBamt, 0f, Time.smoothDeltaTime);
					}
					VerticalBob.bobbingAmount = GBamt * deltaAmount;//apply delta at this step for framerate independence
					VerticalBob.rollingAmount = HRamt * deltaAmount;
					VerticalBob.yawingAmount = HYamt * deltaAmount;
					VerticalBob.pitchingAmount = HPamt * deltaAmount;
					HorizontalBob.bobbingAmount = HBamt * deltaAmount;
				}
			}
		}
	}
	
}