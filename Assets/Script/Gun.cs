using UnityEngine;
using System.Collections;
using Niantic.ARDKExamples.Pong;

public class Gun : MonoBehaviour
{
    public GameObject flag;
    public GameObject gunHolder;
    public Animator gunAnimator; // Reference to the Animator component
	public Animator flagAnimator;
	private GameObject balloon;
	public GameObject lineObjectToDisable;

    [Header ("Flares")]
    public GameObject clownFlares;
    public GameObject gunFlares;
    public Transform parentTransform;
	public GameObject popFlares;
	public GameObject appearFlares;

	[Header("Sounds")]
	private AudioSource audioSource;
	private AudioSource audioSource2;
	public AudioClip gunshot;
	public AudioClip confetti;
	public AudioClip balloonpop;
	public AudioClip pickup;
	public AudioClip poof;
	public AudioClip reload;
	public AudioClip putdown;

	bool pickedup = false;
	private void Start()
	{
		// Get the AudioSource component attached to the same GameObject
		audioSource = GetComponent<AudioSource>();
		audioSource2 = GetComponent<AudioSource>();
	}

	//private void Update()
	//{
	//    gunHolder.transform.rotation = Camera.main.transform.rotation;
	//}

	public void GunAppear()
	{
		gunHolder.SetActive(true);

		// Play the assigned audio clip
		audioSource.PlayOneShot(pickup);
	}

	//   public void GunDisappear()
	//   {
	//       gunHolder.SetActive(false);
	//   }

	public void FlagOut()
    {
        // Show flag
        flag.SetActive(true);

		//play flag animation
		gunAnimator.Play("gunflag");

		// Play the assigned audio clip
		audioSource.PlayOneShot(confetti);

		//instantiate on the flag area
		Instantiate(clownFlares, parentTransform);

        StartCoroutine(ReloadFlag());
    }

    public void FireAnim()
    {
        // Trigger the "Fire" animation
        gunAnimator.Play("fire");

		// Play the assigned audio clip
		audioSource.PlayOneShot(gunshot);

		//delay the reload sound
		StartCoroutine(ReloadSoundDelay());

		//instantiate on the flag area
		Instantiate(gunFlares, parentTransform);

		//pop the balloon
		balloon = GameObject.Find("OpponentBalloon");
		Instantiate(popFlares, balloon.transform);
		balloon.GetComponentInChildren<MeshRenderer>().enabled = false;
		lineObjectToDisable.SetActive(false);

		StartCoroutine(PopBalloon());
    }

	public void fireOpponentAnim()
	{
		// Trigger the "Fire" animation
		gunAnimator.Play("fire");

		//instantiate on the flag area
		Instantiate(gunFlares, parentTransform);

		//pop the balloon
		balloon = GameObject.Find("Balloon");
		Instantiate(popFlares, balloon.transform);
		balloon.GetComponentInChildren<MeshRenderer>().enabled = false;

		// Play the assigned audio clip
		audioSource.PlayOneShot(balloonpop);

		//pop balloon and reset gun on table
		StartCoroutine(PopBalloon());
	}

	IEnumerator ReloadSoundDelay()
	{
		yield return new WaitForSeconds(0.9f);
		// Play the assigned audio clip
		audioSource2.PlayOneShot(reload);
	}

	IEnumerator ReloadFlag()
    {
        yield return new WaitForSeconds(3);
        // Show flag
        flag.SetActive(false);

		//play put down sound
		audioSource.PlayOneShot(putdown);

		//put back the gun on the table
		gunHolder.SetActive(false);

		pickedup = false;

		GameObject.Find("Tabletop(Clone)").GetComponent<table>().GunAppear();

        //let the game controller know you dropped up the gun
        GameObject.Find("GameManager").GetComponent<GameController>().holdingGun = false;
    }

	public void PutDownGun()
	{
		gunAnimator.Play("reload");

		StartCoroutine(DisableGun());
	}

	IEnumerator PopBalloon()
	{
		yield return new WaitForSeconds(2);
		gunHolder.SetActive(false);

		pickedup = false;

		//play put down sound
		audioSource.PlayOneShot(putdown);

		//put back the gun on the table
		GameObject.Find("Tabletop(Clone)").GetComponent<table>().GunAppear();

		//let the game controller know you dropped up the gun
		GameObject.Find("GameManager").GetComponent<GameController>().holdingGun = false;

		//reset balloon
		Instantiate(appearFlares, balloon.transform);
		balloon.GetComponentInChildren<MeshRenderer>().enabled = true;
		lineObjectToDisable.SetActive(true);
	}

	IEnumerator DisableGun()
	{
		yield return new WaitForSeconds(2);
		gunHolder.SetActive(false);

		pickedup = false;

		//play put down sound
		audioSource.PlayOneShot(putdown);

		//put back the gun on the table
		GameObject.Find("Tabletop(Clone)").GetComponent<table>().GunAppear();

		//let the game controller know you dropped up the gun
		GameObject.Find("GameManager").GetComponent<GameController>().holdingGun = false;

		//reset balloon
		if (balloon != null)
		{
			balloon.SetActive(true);
		}
	}

	//private void OnTriggerEnter(Collider other)
	//{
	//	if (other.CompareTag("Gun") && !pickedup)
	//	{
	//		Debug.Log("Gun.cs" + other.name);
	//		pickedup = true;

	//		other.GetComponent<Gun>().GunAppear();
	//		////let the game controller know you picked up the gun
	//		//var gameController = GameObject.Find("GameManager").GetComponent<GameController>();

	//		////if game over, do not allow player to take gun
	//		//if (gameController._isGameStarted)
	//		//{
	//		//	//disable the gun at the table as a pick up anim
	//		//	gameObject.SetActive(false);

	//		//	//enable gun at the player's screen
	//		//	GameObject.Find("GameManager").GetComponent<GameController>().holdingGun = true;
	//		//}
	//	}
	//}
}
