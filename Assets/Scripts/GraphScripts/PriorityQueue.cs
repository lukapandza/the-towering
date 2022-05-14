using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//implementation reference: https://www.dotnetlovers.com/article/231/priority-queue
public class PriorityQueue<T>
{
    class Node
    {
        public float priority;
        public T obj;
    }

    //object array
    private List<Node> queue = new List<Node>();
    private int heapSize = -1;

    //constructor:
    public PriorityQueue(){}

    //private methods:
    private void BuildHeapMin(int i)
    {
        while(i >= 0 && this.queue[(i - 1) / 2].priority > this.queue[i].priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }

    private void MinHeapify(int i)
    {
        int left = LeftChild(i);
        int right = RightChild(i);

        int lowest = i;

        if(left <= this.heapSize && this.queue[lowest].priority > this.queue[left].priority)
        {
            lowest = left;
        }
        if(right <= this.heapSize && this.queue[lowest].priority > this.queue[right].priority)
        {
            lowest = right;
        }

        if(lowest != i)
        {
            Swap(lowest, i);
            MinHeapify(lowest);
        }
    }

    private void Swap(int i, int j)
    {
        var temp = this.queue[i];
        this.queue[i] = this.queue[j];
        this.queue[j] = temp;
    }

    private int LeftChild(int i)
    {
        return (i * 2) + 1;
    }

    private int RightChild(int i)
    {
        return (i * 2) + 1;
    }

    //public methods:
    public void Enqueue(float priority, T obj)
    {
        Node node = new Node();
        node.priority = priority;
        node.obj = obj;

        this.queue.Add(node);
        this.heapSize++;

        BuildHeapMin(this.heapSize);
    }

    public T Dequeue()
    {
        if(this.heapSize > -1)
        {
            var returnVal = this.queue[0].obj;
            this.queue[0] = this.queue[heapSize];
            this.queue.RemoveAt(this.heapSize);
            this.heapSize--;

            MinHeapify(0);

            return returnVal;
        }
        else
        {
            throw new Exception("Queue is empty.\n");
        }
    }

    public void UpdatePriority(T obj, float priority)
    {
        for(int i = 0; i < this.heapSize; i++)
        {
            Node node = this.queue[i];
            if(object.ReferenceEquals(node.obj, obj))
            {
                node.priority = priority;
                
                BuildHeapMin(i);
                MinHeapify(i);
            }
        }
    }

    public bool IsInQueue(T obj)
    {
        foreach(Node node in this.queue)
        {
            if(object.ReferenceEquals(node.obj, obj))
            {
                return true;
            }
        }
        return false;
    }

    public float GetPriority(T obj)
    {
        foreach(Node node in this.queue)
        {
            if(object.ReferenceEquals(node.obj, obj))
            {
                return node.priority;
            }
        }
        throw new Exception("Object not in queue.\n");
    }

    public int Count() => this.heapSize + 1;

    public bool IsEmpty()
    {
        if(Count() > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}