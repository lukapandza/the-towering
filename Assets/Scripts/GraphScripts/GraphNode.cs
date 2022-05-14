using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    //member variables:
    private int index; //-1 for inactive node
    private Vector3 position;
    private bool visited, isFree;
    private float distanceFromSource;
    private GraphNode previousNode;

    //constructor:
    public GraphNode(int index, Vector3 position, bool visited = false, bool isFree = false, float dist = float.MaxValue, GraphNode prev = null)
    {
        this.index = index;
        this.position = position;
        this.visited = visited;
        this.isFree = isFree;
        this.distanceFromSource = dist;
        this.previousNode = prev;
    }

    //getters:
    public int GetIndex() => this.index;

    public Vector3 GetPosition() => this.position;

    public bool GetVisited() => this.visited;

    public bool GetIsFree() => this.isFree;

    public float GetDistanceFromSource() => this.distanceFromSource;

    public GraphNode GetPreviousNode() => this.previousNode;

    //setters:
    public void SetIndex(int input)
    {
        this.index = input;
    }

    public void SetPosition(Vector3 input)
    {
        this.position = input;
    }

    public void SetVisited(bool input)
    {
        this.visited = input;
    }

    public void SetIsFree(bool input)
    {
        this.isFree = input;
    }

    public void SetDistanceFromSource(float input)
    {
        this.distanceFromSource = input;
    }

    public void SetPreviousNode(GraphNode input)
    {
        this.previousNode = input;
    }

    //public methods:

    //returns true if node is a neighbor of this.
    public bool IsNeighborOf(GraphNode node)
    {
        return IsAtDistanceFrom(1, node);
    }

    public bool IsAtDistanceFrom(int distance, GraphNode node)
    {
        float fDistance = (float) distance;
        float nodeDistance = Vector3.Distance(this.position, node.GetPosition());
        if(Mathf.Abs(fDistance - nodeDistance) < 0.001f) return true; //float.Epsilon is not enough tolerance.
        else return false;
    }

    public bool IsWithinDistanceFrom(int distance, GraphNode node)
    {
        float fDistance = (float) distance;
        float nodeDistance = Vector3.Distance(this.position, node.GetPosition());
        if(nodeDistance < fDistance) return true;
        else return false;
    }
}
