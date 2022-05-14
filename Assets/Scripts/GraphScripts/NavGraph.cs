using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//implementing NavGraph as a sparse graph, because at most, a node will have 6 connections and this would lead to
//storing a lot of INFINITE values in an adjecency table
public class NavGraph
{
    //member variables:
    private int tileWidth, tileHeight;
    private List<GraphNode> nodes;
    private List<List<GraphEdge>> edges;

    //constructors:
    NavGraph(){}

    //this constructor takes 2 csv files:
    //the first with a column of node indices and a column of 0/1 indicating if they're free or not
    //the second with a list of connections to be removed in format: from | to 
    //from and to are interchangeable.
    public NavGraph(int tileWidth, int tileHeight, string nodePath, string removeEdgePath)
    {
        this.tileWidth = tileWidth;
        this.tileHeight = tileHeight;

        this.nodes = new List<GraphNode>();
        this.edges = new List<List<GraphEdge>>();

        //reading the first csv file
        ReadActiveNodes(nodePath);

        //this loop will add an edge with cost 1.0 for each neighbor of a node that is free, given that the 
        //parameter node is also free.
        //obstacles have to be implemented below by removing the edges between the nodes surrounding an obstacle
        //same goes for different travel costs.
        foreach(GraphNode node in this.nodes)
        {
            AddEdgeToNeighbors(node);
        }

        //reading the second csv file
        ReadEdgesToRemove(removeEdgePath);
    }

    //private methods:

    //reads a csv file and adds all nodes that are specified as active (1)
    //format:   node0index | active0
    //          node1index | active1...
    private void ReadActiveNodes(string nodePath)
    {
        string[] nodeLines = System.IO.File.ReadAllLines(nodePath);
        foreach(string nodeLine in nodeLines)
        {
            string[] nodeColumns = nodeLine.Split(',');
            int nodeIndex = int.Parse(nodeColumns[0]);
            int intIsFree = int.Parse(nodeColumns[1]);
            bool isFree;
            if(intIsFree == 0) isFree = false;
            else isFree = true;

            //add node:
            GraphNode node = new GraphNode(nodeIndex, GeneratePositionForNode(nodeIndex), false, isFree);
            this.nodes.Add(node);
            List<GraphEdge> edgeList = new List<GraphEdge>();
            this.edges.Add(edgeList);
        }
    }

    //reads a csv file and removes both edges between specified nodes (by index)
    //format:   node0index | node1index
    //          node2index | node3index...
    private void ReadEdgesToRemove(string removeEdgePath)
    {
        string[] removeEdgeLines = System.IO.File.ReadAllLines(removeEdgePath);
        foreach(string removeEdgeLine in removeEdgeLines)
        {
            string[] removeEdgeColumns = removeEdgeLine.Split(',');
            int from = int.Parse(removeEdgeColumns[0]);
            int to = int.Parse(removeEdgeColumns[1]);

            RemoveEdge(from, to);
            RemoveEdge(to, from);
        }
    }

    //calculates the position in world space of the center of a hex tile,
    //based on the width and height of the map and standard width of a hex = 1.
    private Vector3 GeneratePositionForNode(int index)
    {
        float R = 0.577350269f; //assuming width of a point-top regular hexagon is .5
        int column = index % this.tileWidth;
        int row = index / this.tileWidth;

        float xOffset;
        if(row % 2 == 0)
        {
            xOffset = 0.5f;
        }
        else
        {
            xOffset = 1f;
        }

        float xPos = column + xOffset;
        float yPos = R + (1.5f * row * R);

        return new Vector3(xPos, yPos, 0f);
    }

    //dijkstra's shortest path algorithm:
    private List<Vector3> GetShortestPath(int from, int to)
    {
        PriorityQueue<GraphNode> pq = new PriorityQueue<GraphNode>();

        this.nodes[from].SetDistanceFromSource(0f);
        pq.Enqueue(0f, this.nodes[from]);
        
        while(!pq.IsEmpty())
        {
            GraphNode currNode = pq.Dequeue();
            foreach(GraphEdge edge in this.edges[currNode.GetIndex()])
            {
                GraphNode neighbor = this.nodes[edge.GetDestinationIndex()];
                if(neighbor.GetIsFree())
                {
                    float alt = currNode.GetDistanceFromSource() + edge.GetCost();
                
                    if(alt < neighbor.GetDistanceFromSource())
                    {
                        neighbor.SetDistanceFromSource(alt);
                        neighbor.SetPreviousNode(currNode);
                        
                        if(!pq.IsInQueue(neighbor) && !neighbor.GetVisited())
                        {
                            //Don't want to enqueue already visited nodes to avoid cycles, but we do want to update their distance if there is a better path
                            pq.Enqueue(alt, neighbor);
                        }
                    }
                }
            }
            currNode.SetVisited(true);
        }
        
        List<Vector3> output = new List<Vector3>();
        
        if(!this.nodes[to].GetVisited())
        {
            //return empty list if there is no path to destination
            return output;
        }

        GraphNode endNode = this.nodes[to];
        while(endNode.GetIndex() != this.nodes[from].GetIndex())
        {
            output.Insert(0, endNode.GetPosition());
            endNode = endNode.GetPreviousNode();
        }

        //have to clear the nodes so the next path can be calculated.
        ClearNodes();

        //output will have the source node at "from" followed by the path
        return output;
    }

    //method for resetting some node member variables to their default values.
    private void ClearNodes()
    {
        foreach(GraphNode node in this.nodes)
        {
            node.SetVisited(false);
            node.SetPreviousNode(null);
            node.SetDistanceFromSource(float.MaxValue);
        }
    }

    //this method goes over all the nodes and adds an edge to each of the neighbors if they're free
    //this should probably be improved to a more efficient solution
    private void AddEdgeToNeighbors(GraphNode inNode)
    {
        if(inNode.GetIsFree())
        {
            int earlyExitCount = 0;
            foreach(GraphNode node in this.nodes)
            {
                if(node.GetIsFree() && node.IsNeighborOf(inNode) && node.GetIndex() != inNode.GetIndex())
                {
                    GraphEdge edge = new GraphEdge(inNode.GetIndex(), node.GetIndex(), 1f);
                    AddEdge(edge);
                    earlyExitCount++;
                    if(earlyExitCount >= 6) break;
                }
            }
        }
    }

    //public methods:

    //getters:

    //returns an edge from an inex to an index if it exists and returns null if not:
    public GraphEdge GetEdge(int from, int to)
    {
        List<GraphEdge> nodeEdges = this.edges[from];
        foreach(GraphEdge edge in nodeEdges)
        {
            if(from == edge.GetSourceIndex() && to == edge.GetDestinationIndex())
            {
                return edge;
            }
        }
        //return null if edge doesn't exist:
        return null;
    }

    //wrapper for the shortest path algorithm that uses vectors for start and target:
    public List<Vector3> GetShortestPath(Vector3 from, Vector3 to)
    {
        int fromIndex = GetNearestNodeIndex(from);
        int toIndex = GetNearestNodeIndex(to);

        return GetShortestPath(fromIndex, toIndex);
    }

    //returns all tiles within a certain radius of the index-identified tile.
    public List<GraphNode> GetTilesInRadius(int index, int radius)
    {
        GraphNode centerNode = this.nodes[index];
        List<GraphNode> output = new List<GraphNode>();
        output.Add(centerNode);

        for(int i = 1; i <= radius; i++)
        {
            int count = 0;
            foreach(GraphNode node in this.nodes)
            {
                if(node.IsWithinDistanceFrom(i, centerNode))
                {
                    output.Add(node);
                    count++;
                }
                if(count >= i * 6) break;
            }
        }

        return output;
    }

    //this should probably be improved to a more efficient solution
    public int GetNearestNodeIndex(Vector3 input)
    {
        int minIndex = 0;
        float minDistance = Vector3.Distance(input, this.nodes[0].GetPosition());
        foreach(GraphNode node in this.nodes)
        {
            float currDistance = Vector3.Distance(input, node.GetPosition());
            if(currDistance < minDistance)
            {
                minDistance = currDistance;
                minIndex = node.GetIndex();
            }
        }
        return minIndex;
    }

    public GraphNode GetNodeAt(int index) => this.nodes[index];

    public GraphNode GetNodeAt(Vector3 position) => this.nodes[GetNearestNodeIndex(position)];

    //setters:
    public void AddEdge(GraphEdge edge)
    {
        int edgeFromIndex = edge.GetSourceIndex();
        this.edges[edgeFromIndex].Add(edge);
    }

    //removes the edge from "from" to "to
    public void RemoveEdge(int from, int to)
    {
        GraphEdge toRemove = null;
        foreach(GraphEdge edge in this.edges[from])
        {
            if(edge.GetDestinationIndex() == to)
            {
                toRemove = edge;
                break;
            }
        }
        if(toRemove != null)
        {
            this.edges[from].Remove(toRemove);
        }
    }

    public void NodeIsFree(int index, bool isFree)
    {
        this.nodes[index].SetIsFree(isFree);
    }
}
