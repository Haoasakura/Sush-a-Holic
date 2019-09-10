using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {

    public Transform[] spawnPositions;
    public GameObject[] enemiesPrefab;
    public GameObject[] miniBossesPrefabs;
    public GameObject wonaldPrefab;
    public Text enemyCounter;
    public Text spawnTimer;
    

    public int numberOfWaves = 8;
    public int initialEnemiesForWave = 3;
    public int enemiesForWaveCap = 10;
    public int timeForNextWave = 15;
    public int timeBetweenWaves = 5;
    [SyncVar]
    public int currentWave = 0;

    private int numberOfEnemies = 0;
    private int enemiesForWave = 2;
    private int enemiesIncrementInterval = 3;
    private int miniBossesIncrementInterval = 3;
    private float enemieshealthMultiplier = 1f;
    private float miniBosseshealthMultiplier = 1f;
    private bool spawnNextWave=false;

    void Start() {

        Text[] counters = GameObject.FindGameObjectWithTag(Tags.enemyCounter).GetComponentsInChildren<Text>();
        foreach(Text counter in counters) {
            if (counter.name==Tags.enemyCounter)
                enemyCounter = counter;
            else
                spawnTimer = counter;
        }

        StartCoroutine("SpawnEnemies");
    }

    public void SpawnEnemy(GameObject enemyPrefab, Transform spawnTransform, float healthMultiplier) {
        if (!isServer)
            return;
        var enemy = Instantiate(enemyPrefab, transform.position, enemyPrefab.transform.rotation);

        float maxHealth = enemy.GetComponent<EnemyHealth>().maxHealth * healthMultiplier;
        enemy.GetComponent<EnemyHealth>().maxHealth = maxHealth;
        enemy.GetComponent<EnemyHealth>().currentHealth = maxHealth;
        enemy.GetComponent<EnemyHealth>().healthBarSlider.maxValue = maxHealth;
        enemy.GetComponent<EnemyHealth>().healthBarSlider.value = maxHealth;


        NetworkServer.Spawn(enemy);
    }

    public IEnumerator SpawnEnemies() {
        for (int i = timeBetweenWaves; i > 0; i--) {
            enemyCounter.text = "Wave: 0\nEnemy Left : " + numberOfEnemies;
            spawnTimer.text = "Time Until Next Wave: " + i;
            yield return new WaitForSeconds(1f);

        }
        for (;currentWave<numberOfWaves;currentWave++) {
            SpawnWave(currentWave);
            yield return new WaitUntil(() => spawnNextWave);
            spawnNextWave = false;
        }
    }

    public void SpawnWave(int currentWave) {
        if (currentWave < numberOfWaves-1) {
            if (currentWave != 0 && currentWave % enemiesIncrementInterval == 0) {
                enemiesForWave = initialEnemiesForWave;
                enemieshealthMultiplier += 0.5f;
            }

            if (currentWave > 0 && currentWave % miniBossesIncrementInterval == 0)
                miniBosseshealthMultiplier += 0.3f;

            if (currentWave > 4)
                timeBetweenWaves = 10;


            GameObject[] players = GameObject.FindGameObjectsWithTag(Tags.player);
            Vector3 playersPositionMean=Vector3.zero;

            foreach (GameObject player in players) {
                playersPositionMean += player.transform.position;
            }
            playersPositionMean /= players.Length;

            int minDistanceIndex = 0;
            for (int i = 0; i < spawnPositions.Length - 1; i += 5) {
                if (Vector2.Distance(playersPositionMean, spawnPositions[i].transform.position) <= Vector2.Distance(playersPositionMean, spawnPositions[minDistanceIndex].transform.position)) {
                    minDistanceIndex = i;
                }
            }

            int spawnSet = (minDistanceIndex / 5) + 1;
            for (int i = 0; i < enemiesForWave; i++) {
                if(currentWave<5)
                    SpawnEnemy(enemiesPrefab[UnityEngine.Random.Range(0,2)], spawnPositions[5 - ((2 - spawnSet) * 5 - (i % 4))].transform, enemieshealthMultiplier);
                else if (currentWave < 7)
                    SpawnEnemy(enemiesPrefab[UnityEngine.Random.Range(0, 5)], spawnPositions[5 - ((2 - spawnSet) * 5 - (i % 4))].transform, enemieshealthMultiplier);
                else
                    SpawnEnemy(enemiesPrefab[UnityEngine.Random.Range(0, enemiesPrefab.Length)], spawnPositions[5 - ((2 - spawnSet) * 5 - (i % 4))].transform, enemieshealthMultiplier);
            }
            if(currentWave > 4)
                SpawnEnemy(miniBossesPrefabs[UnityEngine.Random.Range(0, miniBossesPrefabs.Length)], spawnPositions[9 - ((2 - spawnSet) * 5)].transform, miniBosseshealthMultiplier);
        }
        else
            SpawnEnemy(wonaldPrefab, spawnPositions[spawnPositions.Length-1].transform,1f);

        StopCoroutine("WaveDefeated");
        StartCoroutine("WaveDefeated",currentWave);

        if(enemiesForWave < enemiesForWaveCap)
            enemiesForWave++;
    }

    public IEnumerator WaveDefeated(int currentWave) {
        float timeSinceWaveSpawn = 0;
        numberOfEnemies = 0;
        numberOfEnemies = GameObject.FindGameObjectsWithTag(Tags.enemy).Length;
        while (true) {
            numberOfEnemies = GameObject.FindGameObjectsWithTag(Tags.enemy).Length;
            if ((numberOfEnemies == 0 || timeSinceWaveSpawn>=timeForNextWave) && currentWave < numberOfWaves - 1) {
                break;
            }
            enemyCounter.text = "Wave: " + (currentWave+1) + "\nEnemy Left : " + numberOfEnemies;
            spawnTimer.text = "Time Until Next Wave: " + (int)((timeForNextWave+timeBetweenWaves) - timeSinceWaveSpawn);
            yield return new WaitForSeconds(0.25f);
            timeSinceWaveSpawn+=0.25f;
        }
        for(int i=timeBetweenWaves;i>0;i--) {
            enemyCounter.text = "Wave: " + (currentWave+1) + "\nEnemy Left : " + numberOfEnemies;
            spawnTimer.text = "Time Until Next Wave: " + i;
            yield return new WaitForSeconds(1f);

        }
        spawnNextWave = true;
    }

    public void WonaldsCall(Vector3 wonaldPosition, int enemyToSpawn) {
        int minDistanceIndex = 0;
        for (int i = 0; i < spawnPositions.Length - 1; i += 5) {
            if (Vector2.Distance(wonaldPosition, spawnPositions[i].transform.position) <= Vector2.Distance(wonaldPosition, spawnPositions[minDistanceIndex].transform.position)) {
                minDistanceIndex = i;
            }
        }
        int spawnSet = (minDistanceIndex / 5) + 1;
        for (int i=0;i< enemyToSpawn; i++) {
            SpawnEnemy(enemiesPrefab[UnityEngine.Random.Range(0,enemiesPrefab.Length)], spawnPositions[5 - ((2 - spawnSet) * 5 - (i % 4))].transform,enemieshealthMultiplier);
        }
        SpawnEnemy(miniBossesPrefabs[UnityEngine.Random.Range(0, miniBossesPrefabs.Length)], spawnPositions[9 - ((2 - spawnSet) * 5)].transform,miniBosseshealthMultiplier);
    }
}