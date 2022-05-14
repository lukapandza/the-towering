using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    //member variables:
    private GameManager gameManager;
    private int range = 0;
    private List<GraphNode> tilesInRange;
    private List<GameObject> targets;
    
    protected virtual void Awake()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.tilesInRange = AssignTilesInRange(this.range);
        this.targets = new List<GameObject>();
    }

    protected virtual void Update()
    {
        if(this.targets.Count > 0)
        {
            if(this.targets[0] == null) this.targets.RemoveAt(0);
            else
            {
                Debug.DrawLine(this.transform.position, this.targets[0].transform.position, Color.red, .1f);
                TargetAction(this.targets[0]);
            }
        }
    }

    private List<GraphNode> AssignTilesInRange(int range)
    {
        NavGraph navGraph = this.gameManager.GetNavGraph();
        int myIndex = navGraph.GetNearestNodeIndex(this.transform.position);
        return navGraph.GetTilesInRadius(myIndex, range);
    }

    public List<GraphNode> GetTilesInRange() => this.tilesInRange;

    public List<GameObject> GetTargets() => this.targets;

    public void AddTarget(GameObject target)
    {
        if(!this.targets.Contains(target)) this.targets.Add(target);
    }

    public void RemoveTarget(GameObject target)
    {
        if(this.targets.Contains(target)) this.targets.Remove(target);
    }

    public void SetRange(int input)
    {
        this.range = input;
    }

    protected virtual void TargetAction(GameObject target){}

}
