using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

	public static BirdController instance;

	public float bounceForce=-4; //độ nảy của quả bóng

	private Rigidbody2D myBody; //khai báo vật lý
	private Animator anim; // khai báo animation

	[SerializeField] //để hiển thị thuộc tính,ví dụ thuộc tính vẫn là private 
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip flyClip,pingClip,diedClip;

	private bool isAlive; // kiểm tra ball còn tồn tại trong gameplay thì flap dc, ball die thì didflap không dùng được
	private bool didFlap;

	private GameObject spawner;

	public float flag=0;
	public int score=0;

	// Use this for initialization
	void Awake () { // hàm khởi tạo các đối tượng ban đầu  hàm Start là hàm bắt đầu game
		isAlive = true;
		myBody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		_MakeInstance ();
		 this.bounceForce=-4; 
		 myBody.gravityScale=-1;
		spawner = GameObject.Find ("Spawner Pipe");
	}

	void _MakeInstance(){
		if (instance == null) {
			instance = this;
		}
	}

	// Update is called once per frame
	void FixedUpdate () { // truyền vận tốc thì dùng FixedUpdate ngược lại dùng Update
		_BirdMoveMent ();
	}

	void _BirdMoveMent(){

		if (isAlive) {
			if (didFlap) {
				didFlap = false; // người dùng nhấn duy nhất 1 lần,nếu k có hàm này quả bóng sẽ tự đi lên không thể điều khiển được
				myBody.velocity = new Vector2 (myBody.velocity.x, bounceForce); // vận tốc
				audioSource.PlayOneShot (flyClip);
			}
		}
		if (myBody.velocity.y > 0) { //nhảy lên thì cái đầu quả bóng chúi xuống
			float angel = 0; // tính góc
			angel = Mathf.Lerp (0, 90, myBody.velocity.y / 7); //phép toán nội suy giữa a và b
			transform.rotation = Quaternion.Euler (0, 0, angel); // xoay
		}else if (myBody.velocity.y == 0) {
			transform.rotation = Quaternion.Euler (0, 0, 0);
		}else if (myBody.velocity.y < 0) {
			float angel = 0;
			angel = Mathf.Lerp (0, -90, -myBody.velocity.y / 7);
			transform.rotation = Quaternion.Euler (0, 0, angel);
		}
	}

	public void FlapButton(){ // sự kiện Button chơi khi nhấn vào
		didFlap = true;
	}

	void OnTriggerEnter2D(Collider2D target){
		if (target.tag == "PipeHolder") {
			score++;
			if (GamePlayController.instance != null) {
				GamePlayController.instance._SetScore (score);
			}
			audioSource.PlayOneShot (pingClip);
		}
	}

	void OnCollisionEnter2D(Collision2D target){
		if (target.gameObject.tag == "Pipe" || target.gameObject.tag == "Ground") {
			flag = 1;
			if (isAlive) {
				isAlive = false;
				Destroy (spawner);
				audioSource.PlayOneShot (diedClip);
				myBody.gravityScale=2;
				anim.SetTrigger ("Died");
			}
			if (GamePlayController.instance != null) {
				GamePlayController.instance._BirdDiedShowPanel (score);
			}
		}
	}
}
