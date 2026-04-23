using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PQTest : MonoBehaviour
{
    public int nodeCount = 5;
    public int minValue = 0;
    public int maxValue = 100;

    public GameObject Node;

    private BinarySearchTree<int, int> bst = new BinarySearchTree<int, int>();
    private readonly Dictionary<object, GameObject> nodeObjects = new();
    private readonly Dictionary<object, Vector3> nodePositions = new();

    private void Start()
    {

        for (int i = 0; i < nodeCount; i++)
        {
            int randomValue = Random.Range(minValue, maxValue);

            // 다른 key값이 다른 value 가 될 수 있으니
            bst[randomValue] = randomValue;

        }

        if (bst.root == null) return;

        CreateNode(bst.root);
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
}
