using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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



    private BinarySearchTree<int, int> bst = new BinarySearchTree<int, int>();
    private int height;

    private float x = 1f;
    private void Start()
    {

        for(int i = 0; i < nodeCount; i++)
        {
            bst[i] = Random.Range(minValue, maxValue);
            Debug.Log(bst[i]);
        }


        height = bst.root.Height;

        Pow(bst.root);

    }

    private void OnEnable()
    {
        if (currentSetting == Batch.Pow)
        {

            Pow(bst.root);
        }

        else if (currentSetting == Batch.LevelOrder)
        {
            LevelOrder();
        }

        else if (currentSetting == Batch.InOrder)
        {
            InOrder();
        }
    }



    private void Pow(TreeNode<int, int> node)
    {

        

        SetNode(node);

    }

    private void SetNode(TreeNode<int, int> node)
    {
        if (node == null)
        {
            return;
        }

        var queue = new Queue<TreeNode<int, int>>();
        
        x = 2 ^ nodeCount;
        float xLeft = 0;
        float xRight = 0;

        bool isLeft = false;
        queue.Enqueue(node);

        while (queue.Count > 0)
        {

            var current = queue.Dequeue();

            GameObject go = Instantiate(Node, new Vector3(x, height, 0), transform.rotation);
            go.GetComponent<TextMeshProUGUI>().text = $"{current.Value}";

            if (current.Left != null)
            {
                queue.Enqueue(current.Left);
                xLeft = x / 2;
            }


            if (current.Right != null)
            {
                queue.Enqueue(current.Right);
                xRight = x * 2;
            }

            isLeft = !isLeft;
            x = isLeft ? xLeft : xRight;

        }

    }


    private void LevelOrder()
    {

    }
    private void InOrder()
    {

    }
}
