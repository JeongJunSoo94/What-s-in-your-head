using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {


	//The Hook Object
		public GameObject GrappleHookPrefab;

	//The Line Renderer That Renders The Rope
		public LineRenderer RopeRenderer;

	//The Maximum Distance Your Grappling Hook can Reach
		public int MaximumReach;

	//How Fast The Grappling Hook Reels You In
		public int SpeedofTravel;

	//The X Y Z Coordinates of Where The Hook Was Shot
	//	Vector3 HookPoint;

	//The Point That The Hook Hit a Gameobject
		RaycastHit Hit;

	//The Gameobject That Acts As a Starting Point For The Rope (Currently The Hand Object Located Under The Main Camera)
		public GameObject LineStartPoint;

	//True When The Hook Is Attached To Something
	public bool HasLockedOn;

	//The PlayerMove Class
		public PlayerMove Player;

	//True When The Player Is Currently Grappling
	public bool IsGrappling;


	public bool ErrorHook;
		

	public Transform HookSpawnPoint;

	//The Grapple Hook
		GameObject TheHook;

	//How fast the hook gets from your hand to its destination
	public float HookTravelSpeed = 15;

	//The hook in your hand, its only purpose is looking good
	public GameObject FakeHook;


	//How much the player jumps after the grapple
	public float DismountJumpMultiplier = 1.5F;

	//The three audio sources for playing sounds from different locations
	public  AudioSource SourceCamera;
	public AudioSource SourceHook;
	public AudioSource SourceRope;

	public int GrappleCooldownTimer;
	public int GrappleCooldown;

	void Start()
	{
		//Set Booleans To Default State
			HasLockedOn = false;
			RopeRenderer.enabled = false;

		//Setting Camponents & Audio sources
			FakeHook.GetComponent<MeshRenderer>().enabled = true;

			
				
	}


	void Update()
	{

		if(GrappleCooldownTimer < GrappleCooldown)GrappleCooldownTimer++;

	//Getting The Audio Source for the hook
		SourceHook = GameObject.Find("Rope").GetComponent<AudioSource>();


		SourceCamera = Camera.main.GetComponent<AudioSource>();


		SourceRope = gameObject.GetComponent<AudioSource>();



	//Playing the "Reeling in" sound effect
		if(IsGrappling && SourceRope.isPlaying == false)
		{
			SourceRope.Play();
		}
		else if(IsGrappling == false)
		{
			SourceRope.Stop();
		}

	//Hook object always faces the correct direction
		if(TheHook != null)
		{
			TheHook.transform.LookAt(Camera.main.transform.position);

		}

	//Setting the Gameobject "TheHook" for use throughout the script
		if(GameObject.Find("DaHook") !=  null)
		{
			TheHook = GameObject.Find("DaHook");
		}

		if(TheHook != null && Hit.point != null)
		{
			if(Vector3.Distance(transform.position, Hit.point) < 1 && IsGrappling)
			{

			FakeHook.GetComponent<MeshRenderer>().enabled = true;
			IsGrappling = false;
			HasLockedOn = false;
			ErrorHook = false;
			RopeRenderer.enabled = false;
			Player.CanPlayerMove = true;
			Destroy(GameObject.Find("DaHook"));
			Player.VerticalVelocity = Player.JumpForce * DismountJumpMultiplier;


			}
		}

		if(TheHook != null && Hit.point != null)
		{
			if(Vector3.Distance(TheHook.transform.position, Hit.point) < 15 && ErrorHook == true && SourceHook.isPlaying == false)SourceHook.Play();
				
			if(Vector3.Distance(TheHook.transform.position, Hit.point) < 10 && ErrorHook == true)
			{
				
					
					FakeHook.GetComponent<MeshRenderer>().enabled = true;
					IsGrappling = false;
					HasLockedOn = false;
					ErrorHook = false;
					RopeRenderer.enabled = false;
					Player.CanPlayerMove = true;
					Destroy(GameObject.Find("DaHook"));


			}

		}


	//Set The Beginning of The Rope To The Correct Position
		if(RopeRenderer.enabled == true && TheHook != null)
		{

				RopeRenderer.SetPosition(0, LineStartPoint.transform.position);
				RopeRenderer.SetPosition(1, TheHook.transform.position);

		}

	//If Your Hook Has Attached & pressing right mouse, Reel The Player In
		if(Input.GetMouseButton(1) && HasLockedOn)
		{
			ReelMeIn();
		}

	//Stop Reeling in if you let go of right mouse
		if(Input.GetMouseButtonUp(1) && HasLockedOn)
		{
			FakeHook.GetComponent<MeshRenderer>().enabled = true;
			IsGrappling = false;
			HasLockedOn = false;
			RopeRenderer.enabled = false;
			Player.CanPlayerMove = true;
			ErrorHook = false;
			Destroy(GameObject.Find("DaHook"));
			Player.VerticalVelocity = Player.JumpForce * DismountJumpMultiplier;


		}

		//When Left Mouse Button Is Pressed, Shoot Out The Hook (Can Be Canged To Almost Any Button)
			if(Input.GetMouseButtonDown(0))
			{
				
				ActuallyShootHook();
			}


	//If The hook has a target, shoot out the hook towards it
		if(HasLockedOn == true || ErrorHook == true)
			{
			MoveTheHookTowardsTarget();
			}


		}




	void MoveTheHookTowardsTarget()
	{
      //Lerp The hook object towards the target for a cool effect
		if(TheHook != null)TheHook.transform.position = Vector3.Lerp(TheHook.transform.position, Hit.point, HookTravelSpeed * 1F * Time.deltaTime);
	}

	void ActuallyShootHook ()
	{

		//Shoot Out From Where The Mouse Is
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, MaximumReach))
		{
			

		//Spawning the hook, setting it up, activating rope & setting booleans

			if(GameObject.Find("DaHook") == null && GrappleCooldownTimer == GrappleCooldown && Hit.transform.gameObject != null)
			{
				SourceCamera.Play();
				GameObject DaHook = Instantiate(GrappleHookPrefab) as GameObject;
				FakeHook.GetComponent<MeshRenderer>().enabled = false;
				RopeRenderer.enabled = true;
				RopeRenderer.SetPosition(0, LineStartPoint.transform.position);
				RopeRenderer.SetPosition(1, DaHook.transform.position);

				DaHook.name = "DaHook";

				DaHook.transform.position = HookSpawnPoint.transform.position;

				if(Hit.transform.gameObject.tag == "CanGrapple")HasLockedOn = true;

				if(Hit.transform.gameObject.tag != "CanGrapple")ErrorHook = true;

				GrappleCooldownTimer = 0;



			}
			//Destroy pre-existing hook, Spawning the new hook, setting it up, activating rope & setting booleans
			else if(GameObject.Find("DaHook") != null && GrappleCooldownTimer == GrappleCooldown && Hit.transform.gameObject != null)
			{
				Destroy(GameObject.Find("DaHook"));
				SourceCamera.Play();
				GameObject DaHook = Instantiate(GrappleHookPrefab) as GameObject;
				FakeHook.GetComponent<MeshRenderer>().enabled = false;
				RopeRenderer.enabled = true;
				RopeRenderer.SetPosition(0, LineStartPoint.transform.position);
				RopeRenderer.SetPosition(1, DaHook.transform.position);


				DaHook.name = "DaHook";

				DaHook.transform.position = HookSpawnPoint.transform.position;


				if(Hit.transform.gameObject.tag == "CanGrapple")HasLockedOn = true;

				if(Hit.transform.gameObject.tag != "CanGrapple")ErrorHook = true;

				GrappleCooldownTimer = 0;
			}



		}



	}






	void ReelMeIn()
	{
		//Move The Player Towards The Hook Point While Updating The Rope Position
			



			HasLockedOn = true;
			RopeRenderer.SetPosition(0, LineStartPoint.transform.position);
			RopeRenderer.SetPosition(1, Hit.point);
			Player.CanPlayerMove = false;
		transform.position = Vector3.Lerp(gameObject.transform.position, Hit.point, SpeedofTravel * Time.deltaTime/Vector3.Distance(gameObject.transform.position, Hit.point));
			IsGrappling = true;
	}

}
