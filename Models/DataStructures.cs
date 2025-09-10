using CivicLink.Models;

namespace CivicLink.DataStructures
{
    // Custom LinkedList implementation for storing issues
    public class IssueLinkedList
    {
        private IssueNode head;
        private int count;

        public int Count => count;

        public void Add(Issue issue)
        {
            var newNode = new IssueNode(issue);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                var current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
            count++;
        }

        public Issue GetById(int id)
        {
            var current = head;
            while (current != null)
            {
                if (current.Data.Id == id)
                    return current.Data;
                current = current.Next;
            }
            return null;
        }

        public IEnumerable<Issue> GetAll()
        {
            var issues = new List<Issue>();
            var current = head;
            while (current != null)
            {
                issues.Add(current.Data);
                current = current.Next;
            }
            return issues;
        }

        public bool Remove(int id)
        {
            if (head == null) return false;

            if (head.Data.Id == id)
            {
                head = head.Next;
                count--;
                return true;
            }

            var current = head;
            while (current.Next != null)
            {
                if (current.Next.Data.Id == id)
                {
                    current.Next = current.Next.Next;
                    count--;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
    }

    public class IssueNode
    {
        public Issue Data { get; set; }
        public IssueNode Next { get; set; }

        public IssueNode(Issue issue)
        {
            Data = issue;
            Next = null;
        }
    }

    // Priority Queue for handling issues by priority
    public class IssuePriorityQueue
    {
        private readonly List<Issue> heap;

        public IssuePriorityQueue()
        {
            heap = new List<Issue>();
        }

        public int Count => heap.Count;

        public void Enqueue(Issue issue)
        {
            heap.Add(issue);
            HeapifyUp(heap.Count - 1);
        }

        public Issue Dequeue()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            var result = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
                HeapifyDown(0);

            return result;
        }

        public Issue Peek()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Queue is empty");
            return heap[0];
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[index].Priority <= heap[parentIndex].Priority)
                    break;

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int largest = index;

                if (leftChild < heap.Count && heap[leftChild].Priority > heap[largest].Priority)
                    largest = leftChild;

                if (rightChild < heap.Count && heap[rightChild].Priority > heap[largest].Priority)
                    largest = rightChild;

                if (largest == index)
                    break;

                Swap(index, largest);
                index = largest;
            }
        }

        private void Swap(int i, int j)
        {
            var temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        public IEnumerable<Issue> GetAll()
        {
            return heap.OrderByDescending(i => i.Priority);
        }
    }

    // Stack for tracking recent activities
    public class ActivityStack
    {
        private readonly Stack<string> activities;
        private readonly int maxSize;

        public ActivityStack(int maxSize = 50)
        {
            this.maxSize = maxSize;
            activities = new Stack<string>();
        }

        public void Push(string activity)
        {
            if (activities.Count >= maxSize)
            {
                // Convert to array, remove oldest, convert back
                var items = activities.ToArray().Reverse().Skip(1).Reverse();
                activities.Clear();
                foreach (var item in items)
                {
                    activities.Push(item);
                }
            }
            activities.Push($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {activity}");
        }

        public string Pop()
        {
            return activities.Count > 0 ? activities.Pop() : null;
        }

        public string Peek()
        {
            return activities.Count > 0 ? activities.Peek() : null;
        }

        public IEnumerable<string> GetRecent(int count = 10)
        {
            return activities.Take(count);
        }

        public int Count => activities.Count;
    }
}