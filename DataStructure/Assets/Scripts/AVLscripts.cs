using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AVLscripts<TKey, TValue> : BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
{
    public AVLscripts() : base()
    {

    }


    protected override TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        node = base.Add(node, key, value);
        return Balance(node);
    }

    protected override TreeNode<TKey, TValue> AddOrUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        node = base.AddOrUpdate(node, key, value);
        return Balance(node);
    }

    protected int BalanceFactor(TreeNode<TKey, TValue> node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);   
    }

    protected override TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        node = Remove(node, key);
        if (node == null)
            return node;

        return Balance(node);
    }

    
    protected TreeNode<TKey, TValue> RotateRight(TreeNode<TKey, TValue> node)
    {
        // 교재에 있는 순서
        var leftChild = node.Left;
        var rightSubtreeOfLeftChild = leftChild.Right;

        leftChild.Right = node;
        node.Left = rightSubtreeOfLeftChild;

        UpdateHeight(node);
        UpdateHeight(leftChild);

        return leftChild;

    }
    protected TreeNode<TKey, TValue> RotateLeft(TreeNode<TKey, TValue> node)
    {
        // 교재에 있는 순서
        var rightChild = node.Right;
        var LeftSubtreeOfLeftChild = rightChild.Left;

        rightChild.Left = node;
        node.Right = LeftSubtreeOfLeftChild;

        UpdateHeight(node);
        UpdateHeight(rightChild);

        return rightChild;

    }


    protected TreeNode<TKey, TValue> Balance(TreeNode<TKey, TValue> node)
    {
        int bf = BalanceFactor(node);

        if(bf > 1)
        {
            //LR
            if(BalanceFactor(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
            }

            //LL - 두 유형 다 끝은 좌회전
            return RotateRight(node);
        }

        else if(bf < -1)
        {
            // RL
            if (BalanceFactor(node.Right) < 0)
            {
                node.Right = RotateRight(node.Right);
            }

            // RR - 두 유형 다 끝은 우회전
            return RotateLeft(node);

        }

        return node;
    }


    public bool IsBalanced()
    {
        return IsBalanced(root);
    }

    public bool IsBalanced(TreeNode<TKey, TValue> node)
    {

        if (node == null)
            return true;

        int bf = BalanceFactor(node);

        if (Mathf.Abs(bf) > 1)
            return false;


        return IsBalanced(node.Left) && IsBalanced(node.Right);
    }
}
