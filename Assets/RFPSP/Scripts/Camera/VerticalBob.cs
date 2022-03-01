//VerticalBob.cs by Azuline Studios© All Rights Reserved 
using UnityEngine;
using System.Collections;

public class VerticalBob : MonoBehaviour {
	//Set up external script references
	private FPSRigidBodyWalker FPSWalkerComponent;
	private Ironsights IronsightsComponent;
	private CameraControl CameraControlComponent;
	private Footsteps FootstepsComponent;
	[HideInInspector]
	public WeaponPivot WeaponPivotComponent;
	[HideInInspector]
	public FPSPlayer FPSPlayerComponent;
	[HideInInspector]
	public GameObject playerObj;
	//Variables for vertical aspect of sine bob of camera and weapons
	//This script also makes camera view roll and pitch with bobbing
	private float timer;
	private float timerRoll;
	[HideInInspector]
	public float bobbingSpeed;
	//Vars for smoothing view position
	private float dampOrg;//Smoothed view postion to be passed to CameraControl script
	private float dampTo;
	private Vector3 tempLocalEulerAngles = Vector3.zero;
	//These two vars controlled from ironsights script
	//to allow different values for sprinting/walking ect.
	[HideInInspector]
	public float bobbingAmount;
	[HideInInspector]
	public float rollingAmount;
	[HideInInspector]
	public float yawingAmount;
	[HideInInspector]
	public float pitchingAmount;
	private float midpoint;//Vertical position of camera during sine bobbing
	private float idleYBob;
	[HideInInspector]
	public float translateChange;
	private float translateChangeRoll;
	private float translateChangePitch;
	private float translateChangeYaw;
	private float waveslice;
	private float wavesliceRoll;
	private float dampVelocity;
	private Vector3 dampVelocity2;
	private float totalAxes;
	private float horizontal;
	private float vertical;
	private float inputSpeed;
	private bool doOnce;

	void Start(){
		//Set up external script references
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		IronsightsComponent = playerObj.GetComponent<Ironsights>();
		CameraControlComponent = Camera.main.GetComponent<CameraControl>();
		FootstepsComponent = playerObj.GetComponent<Footsteps>();	
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		WeaponPivotComponent = FPSPlayerComponent.WeaponPivotComponent;
	}
	
	void Update(){

		if(Time.timeScale > 0 && Time.smoothDeltaTime > 0){//allow pausing by setting timescale to 0
	
			waveslice = 0.0f;
			horizontal = FPSWalkerComponent.inputX;//get input from player movement script
			vertical = FPSWalkerComponent.inputY;	
			
			midpoint = FPSWalkerComponent.midPos;//Start bob from view position set in player movement script
		
			if(FPSWalkerComponent.moving && FPSWalkerComponent.grounded){//Perform bob only when moving and grounded
		
				waveslice = Mathf.Sin(timer);
				wavesliceRoll = Mathf.Sin(timerRoll);

				if(Mathf.Abs(FPSWalkerComponent.inputY) > Mathf.Abs(FPSWalkerComponent.inputX)){
					inputSpeed = Mathf.Abs(FPSWalkerComponent.inputY);
				}else{
					inputSpeed = Mathf.Abs(FPSWalkerComponent.inputX);
				}

				timer = timer + (bobbingSpeed * inputSpeed) * Time.smoothDeltaTime;
				timerRoll = timerRoll + (bobbingSpeed / 2.0f) * Time.smoothDeltaTime;//Make view roll bob half the speed of view pitch bob
				
				if (timer > Mathf.PI * 2.0f){
					timer = timer - (Mathf.PI * 2.0f);
					if(!FPSWalkerComponent.noClimbingSfx){//dont play climbing footsteps if noClimbingSfx is true
						FootstepsComponent.FootstepSfx();//play footstep sound effect by calling FootstepSfx() function in Footsteps.cs
					}
					if(WeaponPivotComponent.isActiveAndEnabled && !doOnce){
						WeaponPivotComponent.PlayAnim();
						doOnce = true;
					}
				}else{
					doOnce = false;
				}
				
				//Perform bobbing of camera roll
				if (timerRoll > Mathf.PI * 2.0f){
					timerRoll = (timerRoll - (Mathf.PI * 2.0f));
					if (!FPSWalkerComponent.grounded){
						timerRoll = 0;//reset timer when airborne to allow soonest resume of footstep sfx
					}
				}
			   
			}else{
				//reset variables to prevent view twitching when falling
				timer = 0.0f;
				timerRoll = 0.0f;
				tempLocalEulerAngles = new Vector3(0,0,0);//reset camera angles to 0 when stationary
			}
		
			if (waveslice != 0){
				
				translateChange = (waveslice * bobbingAmount);// * Mathf.Abs(IronsightsComponent.WeaponBehaviorComponent.bobDelayedAmt);
				translateChangePitch = (waveslice * pitchingAmount);// * Mathf.Abs(IronsightsComponent.WeaponBehaviorComponent.bobDelayedAmt);
				translateChangeRoll = (wavesliceRoll * rollingAmount);// * Mathf.Abs(IronsightsComponent.WeaponBehaviorComponent.bobDelayedAmt);
				translateChangeYaw = (wavesliceRoll * yawingAmount);// * Mathf.Abs(IronsightsComponent.WeaponBehaviorComponent.bobDelayedAmt);
				totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
				totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
				//needed for smooth return to neutral view pos
				translateChange = totalAxes * translateChange;
				translateChangePitch = totalAxes * translateChangePitch;
				translateChangeRoll = totalAxes * translateChangeRoll;
				//Set position for smoothing function and add jump value
				//divide position by smoothDeltaTime for framerate independence
				dampTo = midpoint + (translateChange / Time.smoothDeltaTime * 0.01f);
				//camera roll and pitch bob
				tempLocalEulerAngles = new Vector3(translateChangePitch, translateChangeYaw, translateChangeRoll);
				
			}else{
				
				if(!FPSWalkerComponent.swimming){
					if(IronsightsComponent.idleBobAmt > 0f){
						idleYBob = ((Mathf.Sin(Time.time * 1.25f) * 0.015f) * IronsightsComponent.idleBobAmt ) ;
					}
				}else{
					if(IronsightsComponent.swimBobAmt > 0f){
						idleYBob = (Mathf.Sin(Time.time * 1.65f) * 0.08f) * IronsightsComponent.swimBobAmt;//increase vertical bob when swimming
					}
				}
				
				//reset variables to prevent view twitching when falling
				dampTo = (midpoint + idleYBob);//add small sine bob for camera idle movement
				totalAxes = 0;
				translateChange = 0;
			}
			//use SmoothDamp to smooth position and remove any small twitches in bob amount 
			dampOrg = Mathf.SmoothDamp(dampOrg, dampTo, ref dampVelocity, FPSWalkerComponent.camDampSpeed, Mathf.Infinity, Time.smoothDeltaTime);
			//Pass bobbing amount and angles to the camera control script in the camera object after smoothing
			CameraControlComponent.dampOriginY = dampOrg;
			CameraControlComponent.bobAngles = Vector3.SmoothDamp(CameraControlComponent.bobAngles, tempLocalEulerAngles, ref dampVelocity2, 0.1f, Mathf.Infinity, Time.smoothDeltaTime);
		}
	}
}