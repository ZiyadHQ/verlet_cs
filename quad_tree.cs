// using System.Diagnostics;
// using System.Numerics;
// using Raylib_cs;

// public class QuadTreeRoot
// {

//     public QuadTree head;
//     public List<QuadTree> leaves = [];

//     public QuadTreeRoot(List<VerletObject> list, float minX, float maxX, float minY, float maxY, int depth = 4)
//     {
//         this.head = new(list, minX, maxX, minY, maxY, leaves, depth);
//     }

//     public override string ToString()
//     {
//         return head.ToString();
//     }

//     public void draw()
//     {
//         head.draw();
//     }

//     public void draw(int drawDepth)
//     {
//         head.draw(drawDepth);
//     }

//     public void drawMidPoints()
//     {
//         foreach (QuadTree subTree in leaves)
//         {
//             Raylib.DrawCircleV(subTree.getMidPoint(), 5f, Color.Red);
//         }
//     }

// }

// public class QuadTree
// {
//     public List<VerletObject> particles = new List<VerletObject>();
//     public QuadTree[] quadTree = new QuadTree[4];

//     public float minX, maxX, minY, maxY;

//     public int depth;

//     public QuadTree(List<VerletObject> list, float minX, float maxX, float minY, float maxY, List<QuadTree> leaves, int depth = 4)
//     {
//         this.depth = depth;
//         this.minX = minX;
//         this.minY = minY;
//         this.maxX = maxX;
//         this.maxY = maxY;

//         // Base case: If this is a leaf node, store the particles and return
//         if (depth == 0 || list.Count <= 1)
//         {
//             particles.AddRange(list);
//             leaves.Add(this);
//             return;
//         }

//         if (minX >= maxX || minY >= maxY)
//         {
//             throw new Exception("ERROR, minX or minY is larger than or equal to maxX or maxY!");
//         }

//         // Temporary storage of particles for distribution
//         float midX = (minX + maxX) / 2;
//         float midY = (minY + maxY) / 2;

//         List<VerletObject> topLeft = new List<VerletObject>();
//         List<VerletObject> topRight = new List<VerletObject>();
//         List<VerletObject> bottomLeft = new List<VerletObject>();
//         List<VerletObject> bottomRight = new List<VerletObject>();

//         foreach (VerletObject particle in list)
//         {
//             if (particle.position_current.X < midX)
//             {
//                 if (particle.position_current.Y < midY)
//                 {
//                     topLeft.Add(particle);
//                 }
//                 else
//                 {
//                     bottomLeft.Add(particle);
//                 }
//             }
//             else
//             {
//                 if (particle.position_current.Y < midY)
//                 {
//                     topRight.Add(particle);
//                 }
//                 else
//                 {
//                     bottomRight.Add(particle);
//                 }
//             }
//         }

//         // Create child QuadTrees
//         quadTree[0] = new QuadTree(topLeft, minX, midX, minY, midY, leaves, depth - 1);
//         quadTree[1] = new QuadTree(topRight, midX, maxX, minY, midY, leaves, depth - 1);
//         quadTree[2] = new QuadTree(bottomLeft, minX, midX, midY, maxY, leaves, depth - 1);
//         quadTree[3] = new QuadTree(bottomRight, midX, maxX, midY, maxY, leaves, depth - 1);

//         // Clear particles in the parent node after distributing them to children
//         particles.Clear();
//     }

//     public List<QuadTree> FindPerfectNeighbors()
//     {
//         List<QuadTree> neighbors = new List<QuadTree>();

//         // Start from the root and recursively find neighbors
//         FindNeighborsRecursive(this, neighbors);

//         // Remove the current node from the neighbor list (it will always include itself)
//         neighbors.Remove(this);

//         return neighbors;
//     }

//     public void FindNeighborsRecursive(QuadTree target, List<QuadTree> neighbors)
//     {
//         // If this node is a leaf, check if it's adjacent to the target
//         if (IsLeaf() && IsAdjacent(target))
//         {
//             neighbors.Add(this);
//             return;
//         }

//         // Otherwise, recursively check child nodes
//         foreach (var subTree in quadTree)
//         {
//             if (subTree != null)
//             {
//                 subTree.FindNeighborsRecursive(target, neighbors);
//             }
//         }
//     }

//     public bool IsLeaf()
//     {
//         return quadTree.All(q => q == null); // A node is a leaf if it has no children
//     }

//     public bool IsAdjacent(QuadTree target)
//     {
//         // Check if the bounding boxes of this node and the target node are adjacent or overlap
//         return !(this.maxX < target.minX || this.minX > target.maxX ||
//                  this.maxY < target.minY || this.minY > target.maxY);
//     }

//     public List<QuadTree> FindSpatialNeighbors(QuadTreeRoot root)
//     {
//         List<QuadTree> neighbors = new List<QuadTree>();

//         // Define the neighboring boundaries for the target node
//         float leftBoundary = this.minX;
//         float rightBoundary = this.maxX;
//         float topBoundary = this.minY;
//         float bottomBoundary = this.maxY;

//         // Traverse the entire tree to find adjacent nodes
//         FindNeighborsInTree(root.head, neighbors, leftBoundary, rightBoundary, topBoundary, bottomBoundary);

//         // Remove the current node from the result
//         neighbors.Remove(this);

//         return neighbors;
//     }

//     private void FindNeighborsInTree(
//         QuadTree currentNode,
//         List<QuadTree> neighbors,
//         float leftBoundary,
//         float rightBoundary,
//         float topBoundary,
//         float bottomBoundary)
//     {
//         if (currentNode == null)
//             return;

//         // Check if this node is a leaf and adjacent to the target
//         if (currentNode.IsLeaf() && IsTouching(currentNode, leftBoundary, rightBoundary, topBoundary, bottomBoundary))
//         {
//             neighbors.Add(currentNode);
//             return;
//         }

//         // Otherwise, traverse the children
//         foreach (var subTree in currentNode.quadTree)
//         {
//             if (subTree != null)
//             {
//                 FindNeighborsInTree(subTree, neighbors, leftBoundary, rightBoundary, topBoundary, bottomBoundary);
//             }
//         }
//     }

//     // Check if a node's bounds touch the target's boundaries
//     private bool IsTouching(QuadTree node, float leftBoundary, float rightBoundary, float topBoundary, float bottomBoundary)
//     {
//         // Node touches if it shares an edge or corner with the target
//         return !(node.maxX <= leftBoundary || node.minX >= rightBoundary ||
//                  node.maxY <= topBoundary || node.minY >= bottomBoundary);
//     }

//     public override string ToString()
//     {
//         String buffer = "";
//         buffer += $"particles: {particles.Count}, midPoint: {getMidPoint()}, totalMass: {getTotalMass()}\n";

//         foreach (QuadTree subTree in quadTree)
//         {
//             if (subTree == null)
//                 buffer += "";
//             else
//                 buffer += subTree.ToString();
//         }

//         return buffer;
//     }

//     public void draw()
//     {
//         Raylib.DrawCircleV(getMidPoint(), 3f + MathF.Max(MathF.Log2(getTotalMass()), 0f), Color.Gold);
//         foreach (QuadTree subTree in quadTree)
//         {
//             if (subTree != null)
//                 subTree.draw();
//         }
//     }

//     public void draw(int drawDepth)
//     {
//         if (this.depth == drawDepth)
//             Raylib.DrawCircleV(getMidPoint(), 3f + particles.Count, Color.Gold);
//         else
//             foreach (QuadTree subTree in quadTree)
//             {
//                 if (subTree != null)
//                     subTree.draw(drawDepth);
//             }
//     }

//     public Vector2 getMidPoint()
//     {
//         return new Vector2((maxX + minX) / 2, (maxY + minY) / 2);
//     }

//     public float getTotalMass()
//     {
//         return particles.Sum(e => e.getMass());
//     }

//     public List<VerletObject> FindNeighbors(VerletObject target, float radius)
//     {
//         List<VerletObject> neighbors = new List<VerletObject>();

//         if (!IsWithinBounds(target.position_current))
//         {
//             return neighbors; // Particle is out of this QuadTree's bounds
//         }

//         // Check particles in the current node
//         foreach (var particle in particles)
//         {
//             if (particle != target && Vector2.Distance(target.position_current, particle.position_current) <= radius)
//             {
//                 neighbors.Add(particle);
//             }
//         }

//         // Recursively check children
//         foreach (var subTree in quadTree)
//         {
//             if (subTree != null && subTree.IsWithinBounds(target.position_current))
//             {
//                 neighbors.AddRange(subTree.FindNeighbors(target, radius));
//             }
//         }

//         return neighbors;
//     }

//     private bool IsWithinBounds(Vector2 position)
//     {
//         return position.X >= minX && position.X < maxX && position.Y >= minY && position.Y < maxY;
//     }
// }