using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class StayEnemy : MonoBehaviour
{
    //プレイヤーのポジション
    [SerializeField] private GameObject playerObject;

    Rigidbody rigidbody;

    private bool deadFlag;

    [SerializeField] private int hp = 5;

    [SerializeField] private float speed = 0.1f;

    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject firingPoint;
    [SerializeField] private GameObject homingMissile;
    [SerializeField] private float bulletSpeed = 60.0f;
    const int shotDelayMaxTime = 360;
    private int shotDelayTime = shotDelayMaxTime;

    //爆発エフェクト
    [SerializeField] GameObject explosion;
    [SerializeField] private Vector3 explosionSize = new Vector3(1.0f, 1.0f, 1.0f);

    //ドロップする武器
    [SerializeField]
    private GameObject rocketLauncherItem;
    [SerializeField]
    private GameObject sniperRifleItem;
    [SerializeField]
    private GameObject shotGunItem;

    bool stop;
    [SerializeField]
    GameObject pauseObject;
    //ダメージ時se
    AudioSource damageAudioSource;
    [SerializeField]
    AudioClip damageAudioClip;
    // Start is called before the first frame update
    void Start()
    {
        stop = false;
        deadFlag = false;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.drag = 50;
        damageAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseObject.activeSelf)
        {
            stop = false;
        }
        else
        {
            stop = true;
        }

        transform.LookAt(playerObject.transform);

        if (stop)
        {
            //弾の発射処理
            gun.transform.position = gun.transform.position;
            if (shotDelayTime > 0)
            {
                shotDelayTime--;
            }
            else
            {
                //弾の発射処理
                Shot();
                shotDelayTime = shotDelayMaxTime;
            }

            transform.position += transform.forward * speed;

            if (hp <= 0)
            {
                deadFlag = true;
            }

            StageOutProcessing();

            if (deadFlag)
            {
                DropWeapon();
                GameObject newExplosion = Instantiate(explosion, this.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                newExplosion.transform.localScale = explosionSize;
                Destroy(newExplosion, 1.0f);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string gameObjectName = collision.gameObject.tag;
        if (gameObjectName != Constants.normalBulletName.ToString() && gameObjectName != Constants.rocketBombName.ToString() && gameObjectName != Constants.sniperBulletName.ToString() && gameObjectName == Constants.enemyBulletName.ToString()) { return; }

        if (gameObjectName == Constants.normalBulletName.ToString())
        {
            hp -= Constants.normalBulletDamage;
            damageAudioSource.PlayOneShot(damageAudioClip);
        }
        else if (gameObjectName == Constants.rocketBombName.ToString())
        {
            hp -= Constants.rocketBombDamage;
            damageAudioSource.PlayOneShot(damageAudioClip);
        }
        else if (gameObjectName == Constants.sniperBulletName.ToString())
        {
            hp -= Constants.sniperBulletDamage;
            damageAudioSource.PlayOneShot(damageAudioClip);
        }
    }
    /// <summary>
    ///ステージ外に出てしまった際のポジション修正処理
    /// </summary>
    private void StageOutProcessing()
    {
        //ステージ外に出た時にポジションを正しい位置に戻す処理
        var currentPosition = gameObject.transform.position;

        if (currentPosition.z > Constants.stageMaxPositionZ)
        {
            currentPosition.z = Constants.stageMaxPositionZ;
        }
        if (currentPosition.z < Constants.stageMinPositionZ)
        {
            currentPosition.z = Constants.stageMinPositionZ;
        }
        if (currentPosition.x > Constants.stageMaxPositionX)
        {
            currentPosition.x = Constants.stageMaxPositionX;
        }
        if (currentPosition.x < Constants.stageMinPositionX)
        {
            currentPosition.x = Constants.stageMinPositionX;
        }

        gameObject.transform.position = currentPosition;
    }
    /// <summary>
    /// 弾の発射処理
    /// </summary>
    private void Shot()
    {
        // 弾を発射する場所を取得
        var bulletPosition = firingPoint.transform.position;
        // 上で取得した場所に、"grenade"のPrefabを出現させる
        GameObject newBall = Instantiate(homingMissile, bulletPosition, gun.transform.rotation);
        //// 出現させたボールのforward(z軸方向)
        //var direction = newBall.transform.up;
        //// 弾の発射方向にnewBallのz方向(ローカル座標)を入れ、弾オブジェクトのrigidbodyに衝撃力を加える
        //newBall.GetComponent<Rigidbody>().AddForce(direction * bulletSpeed, ForceMode.Impulse);
        Invoke("Explode", Constants.missileLife); // ミサイル弾を発射してから1.5秒後に爆発させる
        // 出現させたボールの名前を"bullet"に変更
        newBall.name = homingMissile.name;
    }
    /// <summary>
    /// 爆破演出
    /// </summary>
    void Explode()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag(Constants.enemyName.ToString()); //「Enemy」タグのついたオブジェクトを全て検索して配列にいれる

        if (cubes.Length == 0) return; // 「Enemy」タグがついたオブジェクトがなければ何もしない。

        foreach (GameObject cube in cubes) // 配列に入れた一つひとつのオブジェクト
        {
            if (cube.GetComponent<Rigidbody>()) // Rigidbodyがあれば、グレネードを中心とした爆発の力を加える
            {
                cube.GetComponent<Rigidbody>().AddExplosionForce(30f, transform.position, 30f, 5f, ForceMode.Impulse);
            }
        }
    }
    /// <summary>
    /// 武器のドロップ処理
    /// </summary>
    private void DropWeapon()
    {
        //出現させる敵をランダムに選ぶ
        int randomValue = Random.Range(2, 6);

        int playerGunType = playerObject.GetComponent<FPSController>().GetGunType();

        if (randomValue == playerGunType)
        {
            return;
        }

        if (randomValue == 2)
        {
            rocketLauncherItem.SetActive(true);
            rocketLauncherItem.transform.position = this.transform.position;
        }
        else if (randomValue == 3)
        {
            sniperRifleItem.SetActive(true);
            sniperRifleItem.transform.position = this.transform.position;
        }
        else if (randomValue == 4)
        {
            shotGunItem.SetActive(true);
            shotGunItem.transform.position = this.transform.position;
        }
    }
}