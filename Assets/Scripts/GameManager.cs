using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //membere vriables:
    public GameManager instance = null;

    private List<GameObject> enemies, towers;
    private NavGraph navGraph;

    private int[] spawnLocations = {2, 3, 4, 5, 31, 47};
    //private int maxEnemiesOnScreen = 6;
    //private int totalEnemies = 12;
    private int goal = 71;

    private int enemiesKilled = 0;
    private int enemiesEscaped = 0;

    private float spawnFreq = 1.6f;

    private bool beganSpawning = false;

    void Awake()
    {
        //make singleton:
        if(instance == null) instance = this;
        else if(instance != this) Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        this.enemies = new List<GameObject>();
        this.towers = new List<GameObject>();

        this.navGraph = new NavGraph(8, 9, "Assets/Maps/TestMap1/nodes.csv", "Assets/Maps/TestMap1/nonEdges.csv");
    }

    void Update()
    {
        if(this.beganSpawning)
        {
            SpawnNewEnemy();
            RemoveEnemiesAtGoal();
        } 
    }


    //private methods:
    private Object LoadEnemy(string enemyType) => Resources.Load("Enemy/" + enemyType);

    private Object LoadTower(string towerType) => Resources.Load("Tower/" + towerType);

    public GraphNode GetTileAt(int index) => this.navGraph.GetNodeAt(index);

    public GraphNode GetTileAt(Vector3 position) => this.navGraph.GetNodeAt(position);

    private void SpawnObjectOnTile(Object asset, GraphNode tile)
    {
        if(tile.GetIsFree() && asset != null)
        {
            GameObject newObject = (GameObject) Instantiate(asset, tile.GetPosition(), Quaternion.identity);
            
            if(newObject.gameObject.CompareTag("Enemy")) this.enemies.Add(newObject);
            else if(newObject.gameObject.CompareTag("Tower"))
            {
                this.towers.Add(newObject);
                this.navGraph.NodeIsFree(tile.GetIndex(), false);
            } 
        }
    }

    public void SpawnRailGunOnTile(GraphNode tile)
    {
        SpawnObjectOnTile(LoadTower("RailGun"), tile);
    }

    public void SendEnemyTo(int enemyIndex, int target)
    {
        List<Vector3> newPath = this.navGraph.GetShortestPath(this.enemies[enemyIndex].transform.position, this.navGraph.GetNodeAt(target).GetPosition());
        NavAgent enemyNavAgent = this.enemies[enemyIndex].GetComponent<NavAgent>();
        enemyNavAgent.SetPath(newPath);
    }
    
    public NavGraph GetNavGraph() => this.navGraph;

    public void RegisterLocation(GameObject enemy)
    {
        foreach(GameObject tower in this.towers)
        {
            GraphNode enemyNode = this.navGraph.GetNodeAt(enemy.transform.position);
            Tower towerController = tower.GetComponent<Tower>();
            List<GraphNode> towerTiles = towerController.GetTilesInRange();
            if(towerTiles.Contains(enemyNode))
            {
                towerController.AddTarget(enemy);
            }
            else
            {
                towerController.RemoveTarget(enemy);
            }
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        RemoveTargetFromTowers(enemy);
        this.enemies.Remove(enemy);
    }

    private void RemoveTargetFromTowers(GameObject enemy)
    {
        foreach(GameObject tower in this.towers)
        {
            Tower towerController = tower.GetComponent<Tower>();
            towerController.RemoveTarget(enemy);
        }
    }

    private bool IsEnemyHeadingTo(int enemyIndex, int tileIndex)
    {
        NavAgent enemyNavAgent = this.enemies[enemyIndex].GetComponent<NavAgent>();
        if(this.navGraph.GetNodeAt(enemyNavAgent.GetDestination()).GetIndex() == tileIndex) return true;
        else return false;
    }

    private bool HasReachedGoal(GameObject enemy)
    {
        Vector3 enemyPosition = enemy.transform.position;
        if(this.navGraph.GetNodeAt(enemyPosition).GetIndex() == this.goal) return true;
        else return false;
    }

    private void SpawnNewEnemy()
    {
        if(this.spawnFreq > 0)
        {
            this.spawnFreq -= Time.deltaTime;
        } 
        else
        {
            this.spawnFreq = 1.6f / (1 + (this.enemiesKilled % 25));
            SpawnObjectOnTile(LoadEnemy("Ant"), GetTileAt(this.spawnLocations[Random.Range(0, this.spawnLocations.Length)]));
            if(!IsEnemyHeadingTo(this.enemies.Count - 1, this.goal)) SendEnemyTo(this.enemies.Count - 1, this.goal);
        }
    }

    private void RemoveEnemiesAtGoal()
    {
        List<GameObject> enemiesToRemove = new List<GameObject>();
        foreach(GameObject enemy in this.enemies)
        {
            if(HasReachedGoal(enemy))
            {
                enemiesToRemove.Add(enemy);
                this.enemiesEscaped++;
            }
        }

        foreach(GameObject enemy in enemiesToRemove)
        {
            RemoveEnemy(enemy);
            Destroy(enemy, .25f);
        }
    }

    public void EnemyKilled()
    {
        this.enemiesKilled++;
    }

    public void BeginSpawning()
    {
        this.beganSpawning = true;
    }
}
