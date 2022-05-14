using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphEdge
{
    //member variables:
    private int fromIndex;
    private int toIndex;
    private float cost;

    //constructor:
    public GraphEdge(int from, int to, float cost = float.MaxValue)
    {
        this.fromIndex = from;
        this.toIndex = to;
        this.cost = cost;
    }

    //getters:
    public int GetSourceIndex() => this.fromIndex;

    public int GetDestinationIndex() => this.toIndex;

    public float GetCost() => this.cost;

    //setters:
    public void SetSourceIndex(int input)
    {
        this.fromIndex = input;
    }

    public void SetDestinationIndex(int input)
    {
        this.toIndex = input;
    }

    public void SetCost(float input)
    {
        this.cost = input;
    }
}
