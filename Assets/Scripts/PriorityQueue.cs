using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<T> heap = new List<T>();
    private System.Comparison<T> compare;

    public int Count => heap.Count;

    public PriorityQueue(System.Comparison<T> comparison)
    {
        compare = comparison;
    }

    public void Enqueue(T item)
    {
        heap.Add(item);
        HeapifyUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0) return default;

        T root = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);

        if (heap.Count > 0)
            HeapifyDown(0);

        return root;
    }

    public bool Contains(T item)
    {
        return heap.Contains(item);
    }

    public void UpdateItem(T item)
    {
        int index = heap.IndexOf(item);
        if (index >= 0)
        {
            HeapifyUp(index);
            HeapifyDown(index);
        }
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (compare(heap[index], heap[parentIndex]) >= 0)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallest = index;

            if (leftChild <= lastIndex && compare(heap[leftChild], heap[smallest]) < 0)
                smallest = leftChild;

            if (rightChild <= lastIndex && compare(heap[rightChild], heap[smallest]) < 0)
                smallest = rightChild;

            if (smallest == index) break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        T temp = heap[a];
        heap[a] = heap[b];
        heap[b] = temp;
    }
}
