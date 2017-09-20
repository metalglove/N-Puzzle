using System.Collections.Generic;

namespace Sliding_Puzzle.Classes.Solvers
{
    public class Node
    {
        public List<int> PuzzleState { get; set; }//The "PuzzleState", the puzzlepieces are represented as numbers in a list
        public int Length { get; set; }//The length from the starting node to this node
        public int Distance { get; set; }//The "Distance" from this node to the solution
        public int Value { get; set; }//The value of this node (lower the better). Value = Length + Distance (also known as the cost to travel)
        public Direction Direction { get; set; }//The direction the node went after it was generated.
        public Node ParentNode { get; set; }//The parent of the current node so you are able to backtrack the nodes to get the solving moves.
        public bool IsMoveable { get; set; }//Determines if the node is able to move from the current position.
    }
}
