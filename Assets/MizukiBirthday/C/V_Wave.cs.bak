using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class V_Wave : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        [Tooltip("此波次中，各敵人的生成數量。第 0 項對應 Enemy Prefabs 第 0 項，以此類推。")]
        public List<int> enemyCounts = new List<int>();
    }

    [Header("========== 敵人設定 ==========")]

    [Tooltip("所有可能生成的敵人預置物。")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("========== 波次設定 ==========")]

    [Tooltip("每一項代表一個波次。")]
    public List<Wave> waves = new List<Wave>();

    [Header("========== 生成設定 ==========")]

    [Tooltip("場上最多同時存在多少隻由此生成器產生的敵人。")]
    [Min(1)]
    public int maxAliveEnemies = 5;

    [Tooltip("每生成一隻敵人後，等待幾秒才生成下一隻。")]
    [Min(0f)]
    public float spawnInterval = 2f;

    [Header("========== 完成設定 ==========")]

    [Tooltip("全部波次完成後啟用的物件。")]
    public GameObject objectToEnable;

    [Header("========== UI 進度條 ==========")]

    [Tooltip("手動指定一個 Image。Image Type 必須設定為 Filled。")]
    public Image progressBar;

    // --------------------------------------------------
    // 內部資料
    // --------------------------------------------------

    // 當前正在處理的波次
    private int currentWaveIndex = 0;

    // 當前波次已經生成的敵人數量
    private int currentWaveSpawnedCount = 0;

    // 當前波次總共需要生成的敵人數量
    private int currentWaveTotalCount = 0;

    // 全部波次總敵人數
    private int totalEnemyCount = 0;

    // 已經死亡／完成的敵人數量
    private int defeatedEnemyCount = 0;

    // 當前波次實際已經生成但還活著的敵人
    private int aliveEnemyCount = 0;

    // 紀錄當前波次各種類型還剩多少隻需要生成
    private List<int> remainingEnemyCounts = new List<int>();

    // 是否正在生成
    private bool isSpawning = false;

    // 是否已經全部完成
    private bool isFinished = false;
	
	private Level levelManager;
	
    // --------------------------------------------------
    // Unity
    // --------------------------------------------------

    private void Start()
    {
		levelManager = FindFirstObjectByType<Level>();
		
        Initialize();
    }

    // --------------------------------------------------
    // 初始化
    // --------------------------------------------------

    private void Initialize()
    {
        currentWaveIndex = 0;
        currentWaveSpawnedCount = 0;
        defeatedEnemyCount = 0;
        aliveEnemyCount = 0;
        isSpawning = false;
        isFinished = false;

        // 計算全部波次的敵人總數
        CalculateTotalEnemyCount();

        // 初始化進度條
        UpdateProgressBar();

        // 如果沒有設定波次
        if (waves == null || waves.Count == 0)
        {
            FinishAllWaves();
            return;
        }

        // 如果沒有敵人預置物
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("V_Wave：沒有設定任何敵人預置物。");
            FinishAllWaves();
            return;
        }

        // 開始第一波
        StartWave(currentWaveIndex);
    }

    // --------------------------------------------------
    // 計算全部敵人數量
    // --------------------------------------------------

    private void CalculateTotalEnemyCount()
    {
        totalEnemyCount = 0;

        if (waves == null)
            return;

        foreach (Wave wave in waves)
        {
            if (wave == null || wave.enemyCounts == null)
                continue;

            for (int i = 0; i < wave.enemyCounts.Count; i++)
            {
                totalEnemyCount += Mathf.Max(0, wave.enemyCounts[i]);
            }
        }
    }

    // --------------------------------------------------
    // 開始一個波次
    // --------------------------------------------------

    private void StartWave(int waveIndex)
    {
        if (isFinished)
            return;

        if (waveIndex >= waves.Count)
        {
            FinishAllWaves();
            return;
        }

        currentWaveIndex = waveIndex;
        currentWaveSpawnedCount = 0;
        aliveEnemyCount = 0;

        remainingEnemyCounts = new List<int>();

        Wave wave = waves[currentWaveIndex];

        if (wave == null || wave.enemyCounts == null)
        {
            currentWaveTotalCount = 0;
        }
        else
        {
            currentWaveTotalCount = 0;

            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                int count = 0;

                if (i < wave.enemyCounts.Count)
                {
                    count = Mathf.Max(0, wave.enemyCounts[i]);
                }

                remainingEnemyCounts.Add(count);
                currentWaveTotalCount += count;
            }
        }

        // 沒有敵人的波次直接進下一波
        if (currentWaveTotalCount <= 0)
        {
            StartCoroutine(WaitForEmptyWave());
            return;
        }

        // 開始生成
        if (!isSpawning)
        {
            StartCoroutine(SpawnWave());
        }
    }

    // --------------------------------------------------
    // 空波次等待
    // --------------------------------------------------

    private IEnumerator WaitForEmptyWave()
    {
        yield return null;

        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            FinishAllWaves();
        }
        else
        {
            StartWave(currentWaveIndex);
        }
    }

    // --------------------------------------------------
    // 生成當前波次
    // --------------------------------------------------

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        while (currentWaveSpawnedCount < currentWaveTotalCount)
        {
            // 場上達到最大敵人數量
            // 等待敵人死亡
            if (aliveEnemyCount >= maxAliveEnemies)
            {
                yield return new WaitUntil(() =>
                    aliveEnemyCount < maxAliveEnemies || isFinished
                );

                if (isFinished)
                    yield break;
            }

            // 找出目前還需要生成的敵人種類
            int selectedEnemyIndex = GetRandomRemainingEnemyIndex();

            // 理論上不應該發生，但作為安全判斷
            if (selectedEnemyIndex == -1)
            {
                Debug.LogWarning("V_Wave：找不到還需要生成的敵人。");
                break;
            }

            // 生成敵人
            SpawnEnemy(selectedEnemyIndex);

            // 更新資料
            remainingEnemyCounts[selectedEnemyIndex]--;

            currentWaveSpawnedCount++;
            aliveEnemyCount++;

            // 更新 UI
            UpdateProgressBar();

            // 如果還有敵人需要生成，等待指定時間
            if (currentWaveSpawnedCount < currentWaveTotalCount)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        isSpawning = false;

        // 所有敵人都已經生成
        // 但還要等待場上敵人全部死亡
        yield return new WaitUntil(() =>
            aliveEnemyCount <= 0 || isFinished
        );

        if (isFinished)
            yield break;
		
		if (levelManager != null)
		{
		    levelManager.WaveCompleted();
		}
		
        // 進入下一波
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            FinishAllWaves();
        }
        else
        {
            StartWave(currentWaveIndex);
        }
    }

    // --------------------------------------------------
    // 隨機選擇還需要生成的敵人
    // --------------------------------------------------

    private int GetRandomRemainingEnemyIndex()
    {
        int totalRemaining = 0;

        for (int i = 0; i < remainingEnemyCounts.Count; i++)
        {
            if (remainingEnemyCounts[i] > 0)
            {
                totalRemaining += remainingEnemyCounts[i];
            }
        }

        if (totalRemaining <= 0)
            return -1;

        // 使用加權隨機。
        // 剩餘數量越多，被抽中的機率越高。
        int randomValue = Random.Range(0, totalRemaining);

        int accumulated = 0;

        for (int i = 0; i < remainingEnemyCounts.Count; i++)
        {
            if (remainingEnemyCounts[i] <= 0)
                continue;

            accumulated += remainingEnemyCounts[i];

            if (randomValue < accumulated)
            {
                return i;
            }
        }

        return -1;
    }

    // --------------------------------------------------
    // 生成一隻敵人
    // --------------------------------------------------

    private void SpawnEnemy(int enemyIndex)
    {
        if (enemyIndex < 0 || enemyIndex >= enemyPrefabs.Count)
            return;

        GameObject prefab = enemyPrefabs[enemyIndex];

        if (prefab == null)
        {
            Debug.LogWarning(
                "V_Wave：Enemy Prefabs 第 " +
                enemyIndex +
                " 項為空。"
            );

            return;
        }

        // 在生成器的位置生成
        GameObject enemy = Instantiate(
            prefab,
            transform.position,
            transform.rotation,
            transform
        );

        // 找到敵人的生命／死亡通知元件
        EnemySpawnTracker tracker = enemy.GetComponent<EnemySpawnTracker>();

        if (tracker == null)
        {
            tracker = enemy.AddComponent<EnemySpawnTracker>();
        }

        tracker.Initialize(this);
    }

    // --------------------------------------------------
    // 敵人死亡通知
    // --------------------------------------------------

    public void NotifyEnemyDefeated(GameObject enemy)
    {
        if (enemy == null)
            return;

        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        defeatedEnemyCount++;

        UpdateProgressBar();
    }

    // --------------------------------------------------
    // 更新進度條
    // --------------------------------------------------

    private void UpdateProgressBar()
    {
        if (progressBar == null)
            return;

        if (totalEnemyCount <= 0)
        {
            progressBar.fillAmount = 1f;
            return;
        }

        // 這裡以「已經被消滅的敵人數量」
        // 作為整體進度。
        float progress =
            (float)defeatedEnemyCount /
            totalEnemyCount;

        progressBar.fillAmount = Mathf.Clamp01(progress);
    }

    // --------------------------------------------------
    // 全部波次完成
    // --------------------------------------------------

    private void FinishAllWaves()
    {
        if (isFinished)
            return;

        isFinished = true;
        isSpawning = false;

        // 確保進度條到 100%
        defeatedEnemyCount = totalEnemyCount;
        UpdateProgressBar();

        // 啟用指定物件
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        Debug.Log("V_Wave：所有波次完成！");
    }

    // --------------------------------------------------
    // 編輯器資訊
    // --------------------------------------------------

    private void OnValidate()
    {
        if (maxAliveEnemies < 1)
            maxAliveEnemies = 1;

        if (spawnInterval < 0f)
            spawnInterval = 0f;
    }
}


// ======================================================
// 敵人生成追蹤器
// ======================================================
//
// 這個類別仍然在同一個 .cs 檔案內。
// 不需要另外建立腳本。
// Spawn 出來的敵人如果沒有這個元件，
// V_Wave 會自動 AddComponent。
// ======================================================

public class EnemySpawnTracker : MonoBehaviour
{
    private V_Wave spawner;

    private bool hasNotifiedDeath = false;

    public void Initialize(V_Wave owner)
    {
        spawner = owner;
        hasNotifiedDeath = false;
    }

    private void OnDestroy()
    {
        // 防止同一個敵人重複通知
        if (hasNotifiedDeath)
            return;

        hasNotifiedDeath = true;

        if (spawner != null)
        {
            spawner.NotifyEnemyDefeated(gameObject);
        }
    }
}