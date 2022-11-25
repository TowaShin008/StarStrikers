using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBomb : MonoBehaviour
{
    //爆発エフェクト
    [SerializeField] GameObject explosion;

	public AudioClip explosionSound;
	// Start is called before the first frame update
	void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {
		//Instantiate(explosion, this.gameObject.transform.position, Quaternion.Euler(0, 0, 0)); // ★追加

		GameObject newExplosion = Instantiate(explosion, this.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(newExplosion, 1.0f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy") { return; }

        AudioSource.PlayClipAtPoint(explosionSound, new Vector3(0, 0, 0));

        Destroy(this);
    }
}
