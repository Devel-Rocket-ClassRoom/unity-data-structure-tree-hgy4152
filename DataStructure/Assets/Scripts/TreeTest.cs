using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
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

    private readonly Dictionary<object, GameObject> nodeObjects = new();


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

        CreateNode(bst.root);
        Pow(bst.root, Vector3.zero, bst.root.Height, Vector3.zero);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying || bst.root == null) return;

        int xIndex = 0;
        nodePositions.Clear();

        switch (currentSetting)
        {
            case Batch.Pow:
                Pow(bst.root, Vector3.zero, bst.root.Height, Vector3.zero);
                break;
            case Batch.LevelOrder:
                LevelOrder(bst.root);
                break;
            case Batch.InOrder:
                InOrder(bst.root, 0, ref xIndex, new Vector3(-1000, -1000, 0));
                break;
        }
    }

    private void CreateNode(TreeNode<int, int> node)
    {
        if (node == null) return;

        if (!nodeObjects.ContainsKey(node))
        {
            GameObject go = Instantiate(Node, Vector3.zero, Quaternion.identity, transform);
            go.GetComponentInChildren<TextMeshPro>().text = $"{node.Value}";
            nodeObjects[node] = go;
        }

        CreateNode(node.Left);
        CreateNode(node.Right);
    }

    private void SetNode(TreeNode<int, int> node, Vector3 position, Vector3 parentPos)
    {

        if (nodeObjects.TryGetValue(node, out GameObject go))
        {
            go.transform.position = position;
            nodePositions[node] = position;


            LineRenderer lr = nodeObjects[node].GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, parentPos); // 시작점: 부모 위치
                lr.SetPosition(1, position);  // 끝점: 내 위치
            }
        }
    }




    private void Pow(TreeNode<int, int> node, Vector3 position, int height, Vector3 parentPos)
    {

        if (node == null) return;

        nodePositions[node] = position;

        if (position != Vector3.zero)
        {
            SetNode(node, position, parentPos);
        }
        else
        {
            SetNode(node, position, position);
        }
        // TODO: 이번 레벨에서 자식을 좌우로 얼마나 벌릴지 계산
        float offset = horizontalSpacing * 0.5f * Mathf.Pow(2, height - 4);

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


        // 라인 그리기
        var renderQueue = new Queue<(TreeNode<int, int> node, Vector3? parentPos)>();
        renderQueue.Enqueue((root, null));

        while (renderQueue.Count > 0)
        {
            var (node, pPos) = renderQueue.Dequeue();
            Vector3 position = nodePositions[node];

            // 부모 위치 정보가 있다면 선 그리기
            if (pPos.HasValue)
            {
                SetNode(node, position, pPos.Value);     
            }
            else
            {
                // 없으면 root 이니 제자리
                SetNode(node, position, position);
            }

            
            if (node.Left != null) 
                renderQueue.Enqueue((node.Left, position));

            if (node.Right != null) 
                renderQueue.Enqueue((node.Right, position));
        }
    
    }

    private void InOrder(TreeNode<int, int> node,int depth, ref int xIndex, Vector3 parentPos)
    {
        if (node == null) return;

        // TODO: 왼쪽 서브트리 먼저 방문 (depth + 1)
        InOrder(node.Left, depth + 1, ref xIndex, Vector3.zero);

        // TODO: 자신의 좌표 기록 — x는 xIndex 기반, y는 depth 기반
        nodePositions[node] = new Vector3(xIndex * horizontalSpacing, -depth * verticalSpacing, 0f);
        Vector3 position = nodePositions[node];
        xIndex++;


        if (depth > 0) // root 제외
        {
            SetNode(node, position, parentPos);
        }
        else
        {
            // root 그리기. 선은 자기자신과 연결해서 안나오게 처리
            SetNode(node, position, position);
        }

        if (node.Left != null && nodePositions.ContainsKey(node.Left))
        {
            SetNode(node.Left, nodePositions[node.Left], position);
        }
        // TODO: 오른쪽 서브트리 방문 (depth + 1)
        InOrder(node.Right, depth + 1, ref xIndex, position);
    }
}
