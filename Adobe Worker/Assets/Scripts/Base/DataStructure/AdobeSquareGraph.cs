using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     동적으로 할당 가능한 그래프입니다.
/// </summary>
public class AdobeSquareGraph<T>
{
    public class Node<T>
    {
        public Node<T>[] near;
        public T value;

        public int xPos;
        public int yPos;
    }

    List<Node<T>> nodes;
    Node<T> start;

    public AdobeSquareGraph()
    {
        nodes = new List<Node<T>>();
        start = new Node<T>();
        nodes.Add(start);
    }

    public T this[int x, int y]
    {
        get
        {
            return Find(x, y).value;
        }
        set
        {
            Find(x, y).value = value;
        }
    }

    private Node<T> Find(int x, int y)
    {
        // 너비 우선 탐색을 기반으로 찾습니다.

        return null;
    }
}
