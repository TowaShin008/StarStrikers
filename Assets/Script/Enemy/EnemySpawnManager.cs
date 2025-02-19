using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawnManager : MonoBehaviour
{
    //出現させる敵のオブジェクト
    [SerializeField] GameObject[] enemys;
    //waveごとの次に敵が出現するまでの時間
    [SerializeField] float[] spawnNextTime = { 10.0f,5.0f };
    //現在の次に敵が出現するまでの時間
    private float nowSpawnNextTime;
    //出現するwave毎の敵の最大数
    [SerializeField] int[] maxEnemyCount = {5,10};
    //出現する現在の敵の最大数
    private int nowMaxEnemyCount;
    //出現する位置
    [SerializeField] GameObject spawnPoint;
    //現在のwaveで敵を出現させた総数
    private int enemyCount;
    //敵を出現させた総数
    private int enemyAllCount;
    //次の敵を出現させるまでの待ち時間
    private float elapsedTime;

    //現在のwaveを保存するための変数
    int nowWave;

    bool moveFlag = false;

    bool randomSpawnFlag = true;
    int enemyType = 0;


    void Start()
    {
        enemyCount = 0;
        elapsedTime = 0.0f;
        this.nowWave = WaveManager.nowWave;

        if (nowWave <= spawnNextTime.Length)
        {
            nowSpawnNextTime = spawnNextTime[nowWave - 1];
        }
        else
        {
            float index = spawnNextTime[spawnNextTime.Length - 1];
            nowSpawnNextTime = index;
        }

        if (nowWave <= maxEnemyCount.Length)
        {
            nowMaxEnemyCount = maxEnemyCount[nowWave - 1];
        }
        else
        {
            nowMaxEnemyCount = maxEnemyCount[maxEnemyCount.Length - 1];
        }

        moveFlag = false;
        randomSpawnFlag = true;
    }
   

    void Update()
    {
		if (moveFlag == false) { return; }

        //if(gameObject.GetComponent<UFO>().GetEntryFlag()==false)
        //{//　経過時間を足す
            elapsedTime += Time.deltaTime;
        //}
    }
    /// <summary>
    /// 敵の出現処理
    /// </summary>
    void SpawnEnemy()
    {
        if(randomSpawnFlag)
		{
            //出現させる敵をランダムに選ぶ
            var randomValue = Random.Range(0, enemys.Length);
            //敵の向きをランダムに決定
            var randomRotationY = Random.value * 360f;

            GameObject.Instantiate(enemys[randomValue], spawnPoint.transform.position, Quaternion.Euler(0f, randomRotationY, 0f));
        }
        else
		{
            //敵の向きをランダムに決定
            var randomRotationY = Random.value * 360f;
            GameObject.Instantiate(enemys[enemyType], spawnPoint.transform.position, Quaternion.Euler(0f, randomRotationY, 0f));
        }

        enemyCount++;
        enemyAllCount++;
        elapsedTime = 0.0f;
    }
    /// <summary>
    /// 次のウェーブへの切り替え処理
    /// </summary>
    /// <returns></returns>
    public bool ChangeNextWave()
	{
        //wave変更時の処理
        if (nowWave != WaveManager.nowWave)
        {
            nowWave = WaveManager.nowWave;

            if (nowWave <= spawnNextTime.Length)
            {
                nowSpawnNextTime = spawnNextTime[nowWave - 1];
            }
            else
            {
                nowSpawnNextTime = spawnNextTime[spawnNextTime.Length - 1];
            }

            if (nowWave <= maxEnemyCount.Length)
            {
                nowMaxEnemyCount = maxEnemyCount[nowWave - 1];
            }
            else
            {
                nowMaxEnemyCount = maxEnemyCount[maxEnemyCount.Length - 1];
            }

            enemyCount = 0;

            return true;
        }

        return false;
    }
    /// <summary>
    /// 停止処理
    /// </summary>
    /// <returns></returns>
    public bool StopProcessing()
	{
        //　この場所から出現する最大数を超えてたら何もしない
        if (enemyCount >= nowMaxEnemyCount)
        {
            //waveManager.WaveChangeFlagOn();
            moveFlag = false;

            return true;
        }
        return false;
    }
    /// <summary>
    /// 敵の投下処理
    /// </summary>
    /// <returns></returns>
    public bool LauncherProcessing()
	{
        //　経過時間が経ったら
        if (elapsedTime > nowSpawnNextTime)
        {
            elapsedTime = 0.0f;
            //敵の出現処理
            SpawnEnemy();

            return true;
        }
        return false;
	}

    public void SetMoveFlag(bool arg_moveFlag)
	{
        moveFlag = arg_moveFlag;
	}

    public void DecrimentEnemyCount()
	{
        enemyCount--;
	}

    public int GetMaxEnemyCount()
	{
        return nowMaxEnemyCount;
	}

    public int GetEnemyCount()
	{
        return enemyCount;
	}

    public void Initialize(bool arg_randomSpawnFlag,int arg_enemtType)
	{
        enemyCount = 0;
        randomSpawnFlag = arg_randomSpawnFlag;
        enemyType = arg_enemtType;
	}
}
