using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {


	//Forward & Backwards Walking Speeds
	public float WalkSpeed = 20F;

	//Left & Right Walking Speeds
	public float StrafeSpeed = 15F;

	//Upwards Jumping Force
	public float JumpForce = 10F;

	//Sensitivity of Horizontal Camera Movement
	private float HorizontalSensitivity = 2F;

	//Sensitivity of Vertical Camera Movement
	private float VerticalSensitivity = 0.6F;

	//The Character Controller On The Player
	public CharacterController player;

	//The "Eyes" of the Player
	private GameObject Camera;

	//For Moving The Player Later In The Code
	private float MoveForward;
	private float MoveStrafe;

	//For Rotating The Camera & Player
	private float RotX;
	private float RotY;

	//Bool That Decides If The Player Can Move (Good For Cutscenes & Player Death)
	public bool CanPlayerMove;

	//How Long The Player Has Been In The Air (Could Be Used To Calculate Fall Damage)
	public float AirTime;

	//Bool Active If Player Is Currently Jumping	
	public bool IsJumping;

	//Vertical Velocity of Player (Used For Calculating Jumps & Simulating Gravity)
	public float VerticalVelocity;

	//How Strong Gravity Will Be On The Player
	public float Gravity = 25F;

	//Active If The Player Is On The Ground
	public bool Grounded;




	void Start ()
	{
		//Setting The Player & Main Camera = To Their In Game Objects
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
		Camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject;

		//Setting The Bools To Their Initial State
		IsJumping = false;
		CanPlayerMove = true;
	}





	void Update ()
	{
		//Rotate The Camera
		RotX = Input.GetAxis("Mouse X") * HorizontalSensitivity;
		RotY = Input.GetAxis("Mouse Y") * VerticalSensitivity;

		//Rotate The Player
		GetComponentInChildren<Camera>().transform.Rotate(-RotY, 0, 0);	
		transform.Rotate(0, RotX, 0);


		//Running & Jumping
		if(CanPlayerMove == true)
		{
			MovePlayer();
			VerticalMovement();
		}
	}




	public void VerticalMovement()
	{
		//Vectors
		Vector3 MoveVector = new Vector3(0, VerticalVelocity, 0);
		player.Move(MoveVector * Time.deltaTime);

		//Vertical Velocity
		if(!player.isGrounded)
		{
			VerticalVelocity -= Gravity * 1F * Time.deltaTime;
		}

		//Calculate The Air Time
		if(!player.isGrounded)
		{
			AirTime++;
		}



		//Reset The Air Time
		if(player.isGrounded)
		{
			IsJumping = false;
			AirTime = 0;
		}

		//Jump
		if(player.isGrounded && Input.GetKeyDown(KeyCode.Space))
		{
			VerticalVelocity = JumpForce;
			IsJumping = true;
		}
	}



	void MovePlayer()
	{
		//Calculate The Movement Speeds
		MoveForward = Input.GetAxis("Vertical") * WalkSpeed;
		MoveStrafe = Input.GetAxis("Horizontal") * StrafeSpeed;

		//Create The Movement Vectors
		Vector3 FBMovement = new Vector3 (0, 0, MoveForward);
		FBMovement = transform.rotation * FBMovement;

		Vector3 RLMovement = new Vector3 (MoveStrafe, 0, 0);
		RLMovement = transform.rotation * RLMovement;


		//Moving The Actual Player (If Touching The Ground)
		if(player.isGrounded)
		{
			player.Move(FBMovement * Time.deltaTime);
			player.Move(RLMovement * Time.deltaTime);
		}

		if(!player.isGrounded)
		{
			player.Move(FBMovement * Time.deltaTime);


			player.Move(RLMovement * Time.deltaTime);
		}


	}


}
