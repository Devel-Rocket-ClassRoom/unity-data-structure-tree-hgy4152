using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class TreeTest : MonoBehaviour
{
    public enum Batch
    {
        None,
        Pow,
        LevelOrder,
        InOrder
    };
    public int nodeCount = 5;
    public Batch currentSetting = Batch.Pow;
    public int minValue = 0;
    public int maxValue = 100;

    public GameObject Node;

    private readonly Dictionary<object, Vector3> nodePositions = new();
    public float horizontalSpacing = 2.0f;
    public float verticalSpacing = 2.0f;

    private BinarySearchTree<int, int> bst = new BinarySearchTree<int, int>();
    private int height;

    private void Start()
    {

        for (int i = 0; i < nodeCount; i++)
        {
            int randomValue = Random.Range(minValue, maxValue);

            // 다른 key값이 다른 value 가 될 수 있으니
            bst[randomValue] = randomValue;

        }

        if (bst.root == null) return;

        height = bst.root.Height;


        //Pow(bst.root, Vector3.zero, bst.root.Height, Vector3.zero);
        LevelOrder(bst.root);
    }

    



    private void Pow(TreeNode<int, int> node, Vector3 position, int height, Vector3 parentPos)
    {

        if (node == null) return;

        nodePositions[node] = position;
        GameObject go = Instantiate(Node, position, Quaternion.identity);
        go.GetComponentInChildren<TextMeshPro>().text = $"{node.Value}";

        if (position != Vector3.zero) // root 제외
        {
            LineRenderer lr = go.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, parentPos); // 시작점: 부모 위치
                lr.SetPosition(1, position);  // 끝점: 내 위치
            }
        }
        // TODO: 이번 레벨에서 자식을 좌우로 얼마나 벌릴지 계산
        float offset = horizontalSpacing * 0.5f * Mathf.Pow(2, height - 2);

        Vector3 childBase = position + Vector3.down * verticalSpacing;

        Pow(node.Left, childBase + Vector3.left * offset, height - 1, position);
        Pow(node.Right, childBase + Vector3.right * offset, height - 1, position);

        

    }


    private void LevelOrder(TreeNode<int, int> root)
    {
        var levels = new List<List<TreeNode<int, int>>>();
        var queue = new Queue<(TreeNode<int, int> node, int depth)>();
        var parentPos = Vector3.zero;
        queue.Enqueue((root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();


            // TODO: levels 리스트 크기가 depth보다 작으면 빈 List를 추가해 늘려준다
            while (levels.Count <= depth) 
            { 
                levels.Add(new List<TreeNode<int, int>>()); 
            }

            levels[depth].Add(node);

            // TODO: 좌/우 자식을 depth + 1로 큐에 넣기
            if (node.Left != null)
                queue.Enqueue((node.Left, depth + 1));
            if (node.Right != null)
                queue.Enqueue((node.Right, depth + 1));
        }

        for (int depth = 0; depth < levels.Count; depth++)
        {
            float y = -depth * verticalSpacing;
            var row = levels[depth];
            for (int i = 0; i < row.Count; i++)
            {
                // TODO: i번째 노드의 x좌표는?
                nodePositions[row[i]] = new Vector3(i * horizontalSpacing, y, 0f);
            }
        }


        var renderQueue = new Queue<(TreeNode<int, int> node, Vector3? parentPos)>();
        renderQueue.Enqueue((root, null));

        while (renderQueue.Count > 0)
        {
            var (node, pPos) = renderQueue.Dequeue();
            Vector3 myPos = nodePositions[node];

            // 노드 생성
            GameObject go = Instantiate(Node, myPos, Quaternion.identity);
            go.GetComponentInChildren<TextMeshPro>().text = $"{node.Value}";

            // 부모 위치 정보가 있다면 선 그리기
            if (pPos.HasValue)
            {
                LineRenderer lr = go.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    lr.positionCount = 2;
                    lr.SetPosition(0, pPos.Value); // 전달받은 부모 위치
                    lr.SetPosition(1, myPos);      // 내 위치
                }
            }

            // 자식들을 큐에 넣을 때 '나의 위치'를 부모 위치로서 전달
            if (node.Left != null) renderQueue.Enqueue((node.Left, myPos));
            if (node.Right != null) renderQueue.Enqueue((node.Right, myPos));
        }
    
    }
    private void InOrder(TreeNode<int, int> node,int depth, ref int xIndex)
    {
        if (node == null) return;

        // TODO: 왼쪽 서브트리 먼저 방문 (depth + 1)
        InOrder(node.Left, depth + 1, ref xIndex);

        // TODO: 자신의 좌표 기록 — x는 xIndex 기반, y는 depth 기반
        nodePositions[node] = new Vector3(xIndex * horizontalSpacing, -depth * verticalSpacing, 0f);
        xIndex++;

        // TODO: 오른쪽 서브트리 방문 (depth + 1)
        InOrder(node.Right, depth + 1, ref xIndex);
    }
}
