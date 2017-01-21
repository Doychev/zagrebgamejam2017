using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiManager : MonoBehaviour {
    public static TsunamiManager Instance
    {
        get;
        private set;
    }

    public float waveWaitTime, warningShowTime;
    public float waveForce;

    public GameObject warningPrefab, wavePrefab;

    void Awake ()
    {
        TsunamiManager.Instance = this;
    }

    void Start () {
        StartCoroutine(LaunchWave());
    }

    IEnumerator LaunchWave()
    {
        while (true)
        {
            Vector3 screenPoint = getWaveStartPoint();
            Vector3 spawnPoint = Camera.main.ViewportToWorldPoint(screenPoint);
            spawnPoint.z = 0;

            GameObject warning = Instantiate(warningPrefab, spawnPoint, Quaternion.identity, gameObject.transform);
            StartCoroutine(DestroyObject(warning, warningShowTime));

            StartCoroutine(SpawnWave(spawnPoint, warningShowTime * 2));

            yield return new WaitForSeconds(waveWaitTime);
        }
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

    public void IncreaseDifficulty()
    {
        Debug.Log("waveWaitTime before: " + waveWaitTime);
        Debug.Log("warningShowTime before: " + warningShowTime);
        waveWaitTime -= waveWaitTime / 25;
        warningShowTime -= warningShowTime / 25;
        Debug.Log("waveWaitTime after: " + waveWaitTime);
        Debug.Log("warningShowTime after: " + warningShowTime);
    }
}
