using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThreeDimensionCashStack : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private GameObject stackParentObj;

    [SerializeField] private Vector3 spaceBetweenObjects;
    [SerializeField] private Vector3 maxStackedObjectCount;

    private readonly List<StackPosition> stackPositions = new();
    private readonly Queue<Transform> objPool = new();
    private readonly Stack<Transform> _stackedObjects = new();


    private void Start()
    {
        GenerateCashPool();
        GeneratePositions();
    }

    public void StackObj()
    {
        if (!objPool.TryDequeue(out Transform objTf)) return;

        Vector3 stackPos = GetAvailableStackPosition();
        _stackedObjects.Push(objTf);
        objTf.SetParent(stackParentObj.transform, true);
        objTf.gameObject.SetActive(true);
        objTf.position = stackPos;
    }

    private Vector3 GetAvailableStackPosition()
    {
        foreach (var stackPos in stackPositions)
        {
            if (stackPos.isOccupied == false)
            {
                stackPos.isOccupied = true;
                return stackPos.pos;
            }
        }

        return Vector3.zero;
    }

    #region Pooling

    private void GenerateCashPool()
    {
        for (int i = 0; i < maxStackedObjectCount.x * maxStackedObjectCount.y * maxStackedObjectCount.z; i++)
        {
            Transform cashObj = Instantiate(obj.transform);
            cashObj.gameObject.SetActive(false);
            objPool.Enqueue(cashObj);
        }
    }

    public void ReturnObjToPool(Transform tf)
    {
        tf.gameObject.SetActive(false);
        objPool.Enqueue(tf);
    }

    #endregion

    #region Preperation

    private void GeneratePositions()
    {
        for (int i = 0; i < maxStackedObjectCount.y; i++)
        {
            for (int j = 0; j < maxStackedObjectCount.z; j++)
            {
                for (int k = 0; k < maxStackedObjectCount.x; k++)
                {
                    stackPositions.Add(new StackPosition(new Vector3(j * spaceBetweenObjects.x,
                        i * spaceBetweenObjects.y,
                        k * spaceBetweenObjects.z)));
                }
            }
        }
    }

    #endregion
    

    private class StackPosition
    {
        public bool isOccupied;
        public readonly Vector3 pos;

        public StackPosition(Vector3 pos)
        {
            this.pos = pos;
        }
    }
}