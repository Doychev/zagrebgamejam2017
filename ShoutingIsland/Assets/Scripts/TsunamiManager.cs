using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiManager : MonoBehaviour {
    public static TsunamiManager Instance
    {
        get;
        private set;
    }

    public float warningShowTime;
    public float waveForce;

    public GameObject warningPrefab, wavePrefab;

    private float[] waveWaitTimeRange = { 3f, 6f };
    private bool multipleWaves = false;

    void Awake ()
    {
        TsunamiManager.Instance = this;
    }

    public IEnumerator LaunchWave()
    {
        StartCoroutine(switchWavesPhases());
        while (true)
        {
            int wavesToSpawn = 1;
            if (multipleWaves)
            {
                float rand = Random.Range(0.0f, 1.0f);
                if (rand > 0.4f && rand <= 0.75f)
                {
                    wavesToSpawn = 2;
                }
                else if (rand > 0.75f)
                {
                    wavesToSpawn = 3;
                }
            }

            for (int i = 0; i < wavesToSpawn; i++)
            {
                Debug.Log("launching wave");
                SpawnWarningAndWave();
            }

            yield return new WaitForSeconds(Random.Range(waveWaitTimeRange[0], waveWaitTimeRange[1]));
        }
    }

    private void SpawnWarningAndWave()
    {
        Vector3 screenPoint = getWaveStartPoint();
        Vector3 spawnPoint = Camera.main.ViewportToWorldPoint(screenPoint);
        spawnPoint.z = 0;

        GameObject warning = Instantiate(warningPrefab, spawnPoint, Quaternion.identity, gameObject.transform);
        StartCoroutine(DestroyObject(warning, warningShowTime));

        StartCoroutine(SpawnWave(spawnPoint, warningShowTime * 2));

    }

    IEnumerator DestroyObject(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(go);
    }

    IEnumerator SpawnWave(Vector3 spawnPoint, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject wave = Instantiate(wavePrefab, spawnPoint, Quaternion.identity, gameObject.transform);
        Vector2 direction = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f)) - spawnPoint;
        wave.GetComponent<Rigidbody2D>().AddForce(direction * waveForce);
    }

    //TODO improve
    Vector3 getWaveStartPoint()
    {
        bool fromBottom = Random.Range(0, 2) == 1;
        bool horizontal = Random.Range(0, 2) == 1;

        float x = 0, y = 0;

        if (horizontal)
        {
            x = Random.Range(0.0f, 1.0f);
            y = fromBottom ? 0 : 1;
        } else
        {
            y = Random.Range(0.0f, 1.0f);
            x = fromBottom ? 0 : 1;
        }

        return new Vector3(x, y);
    }

    public IEnumerator switchWavesPhases()
    {
        //single, 3-6 secs

        yield return new WaitForSeconds(20.0f);
        Debug.Log("switching level");
        waveWaitTimeRange[0] = 1.5f;
        waveWaitTimeRange[1] = 3f;

        yield return new WaitForSeconds(10.0f);
        Debug.Log("switching level");
        multipleWaves = true;
        waveWaitTimeRange[0] = 3f;
        waveWaitTimeRange[1] = 6f;

        yield return new WaitForSeconds(30.0f);
        Debug.Log("switching level");
        waveWaitTimeRange[0] = 1.5f;
        waveWaitTimeRange[1] = 3f;
    }
}
