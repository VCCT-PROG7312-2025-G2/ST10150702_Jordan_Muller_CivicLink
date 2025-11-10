// Advanced data structures for Part 3: Service Request Status tracking
// Implements BST, AVL Tree, Min-Heap, Graph, and MST for efficient request management
using CivicLink.Models;

namespace CivicLink.DataStructures
{
    /* 
     * Summary: Contains advanced data structures required for Part 3.
     * Implements Binary Search Trees (BST and AVL) for efficient searching,
     * Min-Heap for priority management, and Graph structures for representing
     * relationships between service requests including MST algorithms.
     */

    // Node structure for Binary Search Tree
    public class BSTNode
    {
        public Issue Data { get; set; }
        public BSTNode Left { get; set; }
        public BSTNode Right { get; set; }

        public BSTNode(Issue issue)
        {
            Data = issue;
            Left = null;
            Right = null;
        }
    }

    // Binary Search Tree for efficient issue lookup by ID
    // This allows O(log n) search in balanced cases compared to O(n) linear search
    public class IssueSearchTree
    {
        private BSTNode root;
        public int Count { get; private set; }

        public IssueSearchTree()
        {
            root = null;
            Count = 0;
        }

        // Insert an issue into the BST based on ID
        public void Insert(Issue issue)
        {
            root = InsertRecursive(root, issue);
            Count++;
        }

        // Recursive insertion maintaining BST property
        private BSTNode InsertRecursive(BSTNode node, Issue issue)
        {
            if (node == null)
            {
                return new BSTNode(issue);
            }

            if (issue.Id < node.Data.Id)
            {
                node.Left = InsertRecursive(node.Left, issue);
            }
            else if (issue.Id > node.Data.Id)
            {
                node.Right = InsertRecursive(node.Right, issue);
            }

            return node;
        }

        // Search for an issue by ID using BST properties
        public Issue Search(int id)
        {
            return SearchRecursive(root, id);
        }

        // Recursive search with O(log n) average complexity
        private Issue SearchRecursive(BSTNode node, int id)
        {
            if (node == null)
            {
                return null;
            }

            if (id == node.Data.Id)
            {
                return node.Data;
            }
            else if (id < node.Data.Id)
            {
                return SearchRecursive(node.Left, id);
            }
            else
            {
                return SearchRecursive(node.Right, id);
            }
        }

        // In-order traversal to get sorted list of issues
        public List<Issue> GetAllSorted()
        {
            var result = new List<Issue>();
            InOrderTraversal(root, result);
            return result;
        }

        // Recursive in-order traversal
        private void InOrderTraversal(BSTNode node, List<Issue> result)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left, result);
                result.Add(node.Data);
                InOrderTraversal(node.Right, result);
            }
        }
    }

    // AVL Tree node with height tracking for self-balancing
    public class AVLNode
    {
        public Issue Data { get; set; }
        public AVLNode Left { get; set; }
        public AVLNode Right { get; set; }
        public int Height { get; set; }

        public AVLNode(Issue issue)
        {
            Data = issue;
            Left = null;
            Right = null;
            Height = 1;
        }
    }

    /* 
     * ============================================
     * AVL Tree implementation assisted by Claude 2025-11-10
     * also used https://www.geeksforgeeks.org/dsa/introduction-to-avl-tree/ for examples
     * I still didnt quite understand the difference between a AVL and a BST tree so I had to research a bit
     * it is crazy using data structures with a big O notation of O(Log n)
     * ============================================
     */
    // Self-balancing AVL Tree for guaranteed O(log n) operations
    // More complex than BST but maintains balance automatically
    public class AVLSearchTree
    {
        private AVLNode root;
        public int Count { get; private set; }

        public AVLSearchTree()
        {
            root = null;
            Count = 0;
        }

        // Get height of a node
        private int GetHeight(AVLNode node)
        {
            return node?.Height ?? 0;
        }

        // Calculate balance factor
        private int GetBalance(AVLNode node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }

        // Update height of node
        private void UpdateHeight(AVLNode node)
        {
            if (node != null)
            {
                node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
            }
        }

        // Right rotation for balancing
        private AVLNode RotateRight(AVLNode y)
        {
            AVLNode x = y.Left;
            AVLNode T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            UpdateHeight(y);
            UpdateHeight(x);

            return x;
        }

        // Left rotation for balancing
        private AVLNode RotateLeft(AVLNode x)
        {
            AVLNode y = x.Right;
            AVLNode T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            UpdateHeight(x);
            UpdateHeight(y);

            return y;
        }

        // Insert with automatic balancing
        public void Insert(Issue issue)
        {
            root = InsertRecursive(root, issue);
            Count++;
        }

        // Recursive insert with AVL balancing
        private AVLNode InsertRecursive(AVLNode node, Issue issue)
        {
            if (node == null)
            {
                return new AVLNode(issue);
            }

            if (issue.Id < node.Data.Id)
            {
                node.Left = InsertRecursive(node.Left, issue);
            }
            else if (issue.Id > node.Data.Id)
            {
                node.Right = InsertRecursive(node.Right, issue);
            }
            else
            {
                return node;
            }

            UpdateHeight(node);

            int balance = GetBalance(node);

            // Left-Left case
            if (balance > 1 && issue.Id < node.Left.Data.Id)
            {
                return RotateRight(node);
            }

            // Right-Right case
            if (balance < -1 && issue.Id > node.Right.Data.Id)
            {
                return RotateLeft(node);
            }

            // Left-Right case
            if (balance > 1 && issue.Id > node.Left.Data.Id)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right-Left case
            if (balance < -1 && issue.Id < node.Right.Data.Id)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        // Search operation with O(log n) guaranteed complexity
        public Issue Search(int id)
        {
            return SearchRecursive(root, id);
        }

        private Issue SearchRecursive(AVLNode node, int id)
        {
            if (node == null)
            {
                return null;
            }

            if (id == node.Data.Id)
            {
                return node.Data;
            }
            else if (id < node.Data.Id)
            {
                return SearchRecursive(node.Left, id);
            }
            else
            {
                return SearchRecursive(node.Right, id);
            }
        }

        // Get all issues in sorted order
        public List<Issue> GetAllSorted()
        {
            var result = new List<Issue>();
            InOrderTraversal(root, result);
            return result;
        }

        private void InOrderTraversal(AVLNode node, List<Issue> result)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left, result);
                result.Add(node.Data);
                InOrderTraversal(node.Right, result);
            }
        }
    }

    // Min-Heap for priority-based service request management
    // Allows O(1) access to highest priority request and O(log n) insertion
    public class ServiceRequestMinHeap
    {
        private List<Issue> heap;

        public int Count => heap.Count;

        public ServiceRequestMinHeap()
        {
            heap = new List<Issue>();
        }

        // Get parent index
        private int GetParentIndex(int index) => (index - 1) / 2;

        // Get left child index
        private int GetLeftChildIndex(int index) => 2 * index + 1;

        // Get right child index
        private int GetRightChildIndex(int index) => 2 * index + 2;

        // Check if has parent
        private bool HasParent(int index) => GetParentIndex(index) >= 0;

        // Check if has left child
        private bool HasLeftChild(int index) => GetLeftChildIndex(index) < heap.Count;

        // Check if has right child
        private bool HasRightChild(int index) => GetRightChildIndex(index) < heap.Count;

        // Get parent value
        private Issue GetParent(int index) => heap[GetParentIndex(index)];

        // Get left child value
        private Issue GetLeftChild(int index) => heap[GetLeftChildIndex(index)];

        // Get right child value
        private Issue GetRightChild(int index) => heap[GetRightChildIndex(index)];

        // Swap two elements
        private void Swap(int index1, int index2)
        {
            var temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }

        // Compare two issues by priority (Critical > High > Medium > Low)
        private bool HasHigherPriority(Issue issue1, Issue issue2)
        {
            return issue1.Priority > issue2.Priority;
        }

        // Insert new issue and maintain heap property
        public void Insert(Issue issue)
        {
            heap.Add(issue);
            HeapifyUp(heap.Count - 1);
        }

        // Move element up to maintain min-heap property
        private void HeapifyUp(int index)
        {
            while (HasParent(index) && HasHigherPriority(heap[index], GetParent(index)))
            {
                Swap(index, GetParentIndex(index));
                index = GetParentIndex(index);
            }
        }

        // Get highest priority issue without removing
        public Issue Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }
            return heap[0];
        }

        // Remove and return highest priority issue
        public Issue ExtractMin()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }

            var item = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return item;
        }

        // Move element down to maintain heap property
        private void HeapifyDown(int index)
        {
            while (HasLeftChild(index))
            {
                int higherPriorityChildIndex = GetLeftChildIndex(index);

                if (HasRightChild(index) && HasHigherPriority(GetRightChild(index), GetLeftChild(index)))
                {
                    higherPriorityChildIndex = GetRightChildIndex(index);
                }

                if (HasHigherPriority(heap[index], heap[higherPriorityChildIndex]))
                {
                    break;
                }

                Swap(index, higherPriorityChildIndex);
                index = higherPriorityChildIndex;
            }
        }

        // Get all issues in priority order
        public List<Issue> GetAllByPriority()
        {
            var result = new List<Issue>();
            var tempHeap = new List<Issue>(heap);

            while (heap.Count > 0)
            {
                result.Add(ExtractMin());
            }

            heap = tempHeap;
            return result;
        }
    }

    // Graph edge representing relationship between service requests
    public class ServiceRequestEdge
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public double Weight { get; set; }
        public string RelationshipType { get; set; }

        public ServiceRequestEdge(int fromId, int toId, double weight, string relationshipType)
        {
            FromId = fromId;
            ToId = toId;
            Weight = weight;
            RelationshipType = relationshipType;
        }
    }

    /* 
     * ============================================
     * Graph implementation assisted by Claude 2025-11-10
     * Implements graph traversal and MST algorithms
     * again, got stuck trying to implement this for ages and claude helped fix my completely broken algorithm :(
     * ============================================
     */
    // Graph structure for representing relationships between service requests
    // Edges represent similarity (location, category, time) between requests
    public class ServiceRequestGraph
    {
        private Dictionary<int, List<ServiceRequestEdge>> adjacencyList;
        private Dictionary<int, Issue> issues;

        public ServiceRequestGraph()
        {
            adjacencyList = new Dictionary<int, List<ServiceRequestEdge>>();
            issues = new Dictionary<int, Issue>();
        }

        // Add a service request as a vertex
        public void AddVertex(Issue issue)
        {
            if (!issues.ContainsKey(issue.Id))
            {
                issues[issue.Id] = issue;
                adjacencyList[issue.Id] = new List<ServiceRequestEdge>();
            }
        }

        // Add edge between two service requests
        public void AddEdge(int fromId, int toId, double weight, string relationshipType)
        {
            if (!adjacencyList.ContainsKey(fromId))
            {
                adjacencyList[fromId] = new List<ServiceRequestEdge>();
            }

            adjacencyList[fromId].Add(new ServiceRequestEdge(fromId, toId, weight, relationshipType));
        }

        // Breadth-First Search traversal starting from a request
        // Used to find all related requests
        public List<Issue> BFSTraversal(int startId)
        {
            var result = new List<Issue>();
            var visited = new HashSet<int>();
            var queue = new Queue<int>();

            queue.Enqueue(startId);
            visited.Add(startId);

            while (queue.Count > 0)
            {
                int currentId = queue.Dequeue();
                if (issues.ContainsKey(currentId))
                {
                    result.Add(issues[currentId]);
                }

                if (adjacencyList.ContainsKey(currentId))
                {
                    foreach (var edge in adjacencyList[currentId])
                    {
                        if (!visited.Contains(edge.ToId))
                        {
                            visited.Add(edge.ToId);
                            queue.Enqueue(edge.ToId);
                        }
                    }
                }
            }

            return result;
        }

        // Depth-First Search traversal starting from a request
        public List<Issue> DFSTraversal(int startId)
        {
            var result = new List<Issue>();
            var visited = new HashSet<int>();
            DFSRecursive(startId, visited, result);
            return result;
        }

        // Recursive DFS helper
        private void DFSRecursive(int currentId, HashSet<int> visited, List<Issue> result)
        {
            visited.Add(currentId);
            if (issues.ContainsKey(currentId))
            {
                result.Add(issues[currentId]);
            }

            if (adjacencyList.ContainsKey(currentId))
            {
                foreach (var edge in adjacencyList[currentId])
                {
                    if (!visited.Contains(edge.ToId))
                    {
                        DFSRecursive(edge.ToId, visited, result);
                    }
                }
            }
        }

        // Get all edges connected to a specific request
        public List<ServiceRequestEdge> GetEdges(int issueId)
        {
            return adjacencyList.ContainsKey(issueId) ? adjacencyList[issueId] : new List<ServiceRequestEdge>();
        }

        // Get all edges in the graph
        public List<ServiceRequestEdge> GetAllEdges()
        {
            var allEdges = new List<ServiceRequestEdge>();
            foreach (var edges in adjacencyList.Values)
            {
                allEdges.AddRange(edges);
            }
            return allEdges;
        }

        // Kruskal's algorithm for Minimum Spanning Tree
        // Finds the most efficient connections between related requests
        public List<ServiceRequestEdge> GetMinimumSpanningTree()
        {
            var mst = new List<ServiceRequestEdge>();
            var allEdges = GetAllEdges();

            // Sort edges by weight
            allEdges.Sort((e1, e2) => e1.Weight.CompareTo(e2.Weight));

            var parent = new Dictionary<int, int>();
            foreach (var id in issues.Keys)
            {
                parent[id] = id;
            }

            // Find operation for Union-Find
            int Find(int x)
            {
                if (parent[x] != x)
                {
                    parent[x] = Find(parent[x]);
                }
                return parent[x];
            }

            // Union operation for Union-Find
            void Union(int x, int y)
            {
                int rootX = Find(x);
                int rootY = Find(y);
                if (rootX != rootY)
                {
                    parent[rootX] = rootY;
                }
            }

            // Process edges in order of weight
            foreach (var edge in allEdges)
            {
                if (parent.ContainsKey(edge.FromId) && parent.ContainsKey(edge.ToId))
                {
                    int rootFrom = Find(edge.FromId);
                    int rootTo = Find(edge.ToId);

                    if (rootFrom != rootTo)
                    {
                        mst.Add(edge);
                        Union(edge.FromId, edge.ToId);
                    }
                }
            }

            return mst;
        }

        // Get requests related to a given request
        public List<Issue> GetRelatedRequests(int issueId, int maxResults = 5)
        {
            var related = new List<Issue>();
            var visited = new HashSet<int> { issueId };

            if (adjacencyList.ContainsKey(issueId))
            {
                var edges = adjacencyList[issueId].OrderBy(e => e.Weight).Take(maxResults);
                foreach (var edge in edges)
                {
                    if (issues.ContainsKey(edge.ToId))
                    {
                        related.Add(issues[edge.ToId]);
                    }
                }
            }

            return related;
        }
    }
}