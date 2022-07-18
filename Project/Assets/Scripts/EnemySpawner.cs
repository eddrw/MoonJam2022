using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _requiredKillCount = 30;
    [SerializeField] private int _maxAlive = 5;

    [SerializeField] private UnityEvent _killCompletionEvent;

    [SerializeField] private int _killCount = 0;
    [SerializeField] private int _spawnCount = 0;
    [SerializeField] private bool _waitToSpawn = false;

    private bool _hasStartedSpawning = false;

    private void Start()
    {
        if (_player == null)
        {
            _player = GameObject.Find("Player").transform;
        }

        if (!_waitToSpawn)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (!_hasStartedSpawning)
        {
            _hasStartedSpawning = true;

            for (int i = 0; i < _maxAlive; i++)
            {
                SpawnNewEnemy();
            }
        }
    }

    private void SpawnNewEnemy()
    {
        var spawnPositions = _spawnPoints.Select(x => x.position).ToArray();
        //float maxDistSqrd = 0.0f;
        //var furthestPos = _player.position;
        //foreach (var pos in spawnPositions)
        //{
        //    var distSqrd = (pos - _player.position).sqrMagnitude;

        //    if (distSqrd > maxDistSqrd)
        //    {
        //        maxDistSqrd = distSqrd;
        //        furthestPos = pos;
        //    }
        //}

        var spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count() - 1)];

        // Spawn enemy at furthest position
        var go = Instantiate<GameObject>(_enemyPrefab);
        go.transform.position = spawnPos;
        var enemy = go.GetComponent<EnemyController>();
        enemy.SetDeathCallback(this);

        _spawnCount++;
    }

    public void OnEnemyKilled()
    {
        _killCount++;
        if (_spawnCount < _requiredKillCount)
        {
            SpawnNewEnemy();
        }

        if (_killCount >= _requiredKillCount)
        {
            _killCompletionEvent.Invoke();
        }

    }


    private void OnDrawGizmos()
    {
        if (_spawnPoints != null)
        {
            foreach (Transform point in _spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(point.position, Vector3.one * 0.2f);
                }
            }
        }
    }

}
