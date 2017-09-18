using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Windows.System.Threading;
using System.Threading;
namespace Sliding_Puzzle.Classes.Solvers.Special
{
    class Node
    {
        #region Properties
        private int _NodeId;
        public int NodeID { get { return _NodeId; } set { _NodeId = value; /*Debug.WriteLine("Node created: " + value);*/ } }
        private bool ReachedEnd = false;
        private Direction LastDirection = Direction.None;
        public Tree Parent { get; set; }
        public List<int> StartingState { get; set; }
        private List<int> FinalState = new List<int>
        {
            0,  1,  2,  3,  4,
            5,  6,  7,  8,  9,
            10, 11, 12, 13, 14,
            15, 16, 17, 18, 19,
            20, 21, 22, 23, 24
        };
        private List<Node> Children = new List<Node>();
        public List<Direction> Moves { get; set; }
        #endregion Properties

        #region Constructors
        public Node(List<int> StartingState, Tree Parent, bool ReachedEnd)
        {
            this.StartingState = StartingState;
            this.Parent = Parent;
            this.NodeID = Parent.GetUniqueId();
            this.Moves = new List<Direction>();
            if (ReachedEnd == false)
            {
                //PrintState(StartingState);
                GenerateChildren(StartingState, Direction.None);
                //Task.Run(() => GenerateChildren(StartingState, Direction.None));
            }
        }
        public Node(List<int> StartingState, Tree Parent, bool ReachedEnd, Direction LastDirection, List<Direction> Moves)
        {
            this.StartingState = StartingState;
            this.Parent = Parent;
            this.NodeID = Parent.GetUniqueId();
            this.Moves = Moves;
            this.ReachedEnd = ReachedEnd;
            this.LastDirection = LastDirection;
            Moves.Add(LastDirection);
            //if (ReachedEnd == false)
            //{
                //PrintState(StartingState);
                //GenerateChildren(StartingState, LastDirection);
                ////Task.Run(() => GenerateChildren(StartingState, LastDirection));
           // }
        }
        #endregion Constructors

        public void RunNode()
        {
            if (ReachedEnd == false)
            {
                //PrintState(StartingState);
                GenerateChildren(StartingState, LastDirection);
                //Task.Run(() => GenerateChildren(StartingState, LastDirection));
             }
        }
        private void GenerateChildren(List<int> StartingState, Direction LastDirection)
        {
            lock (this)
            {
                List<int> LeftState = CopyState(StartingState);
                List<int> RightState = CopyState(StartingState);
                List<int> UpState = CopyState(StartingState);
                List<int> DownState = CopyState(StartingState);
                switch (LastDirection)
                {
                    case Direction.Left:
                        Left(LeftState);
                        Up(UpState);
                        Down(DownState);
                        break;
                    case Direction.Right:
                        Right(RightState);
                        Up(UpState);
                        Down(DownState);
                        break;
                    case Direction.Up:
                        Left(LeftState);
                        Right(RightState);
                        Up(UpState);
                        break;
                    case Direction.Down:
                        Left(LeftState);
                        Right(RightState);
                        Down(DownState);
                        break;
                    case Direction.None:
                        Left(LeftState);
                        Right(RightState);
                        Up(UpState);
                        Down(DownState);
                        break;
                    default:
                        Debug.WriteLine("Switch 1 case default");
                        break;
                }
                Children.ForEach(node => Parent.AddNodeToQueue(node));
            }
        }

        #region Events
        private bool ReachedFinalState(List<int> State)
        {
            bool bReachedFinalState = false;
            for (int i = 0; i < State.Count; i++)
            {
                if (State[i] ==  FinalState[i])
                {
                    bReachedFinalState = true;
                }
                else
                {
                    return false;
                }
            }
            return bReachedFinalState;
        }
        private List<int> CopyState(List<int> State)
        {
            List<int> NewState = State.ToList();//new List<int>();
            //foreach (int item in State)
            //{
            //    NewState.Add(item);
            //}
            return NewState;
        }
        private List<Direction> CopyMoves(List<Direction> moves)
        {
            List<Direction> CopyOfMoves = moves.ToList();// new List<Direction>();
            //foreach (Direction Move in moves)
            //{
            //   CopyOfMoves.Add(Move);
            //}
            return CopyOfMoves;
        }
        private void PrintState(List<int> State)
        {
            int i = 0;
            Debug.WriteLine("Printed state: \n");
            State.ForEach(item => i = PrintItem(item, i));
        }
        private int PrintItem(int item, int i)
        {
            Debug.Write(item + " ");
            Debug.WriteLineIf(i == 4 || i == 9 || i == 14 || i == 19 || i == 24,"");
            return i + 1;
        }
        #endregion Events

        #region Moves
        private void Left(List<int> StartingStateLeft)
        {
            //Debug.WriteLine("\nLeft on : " + NodeID);
            //Check for completion
            if (ReachedFinalState(StartingStateLeft) == true)
            {
                Parent.SetEndingNode(new Node(StartingStateLeft, Parent, true, Direction.None, Moves)); 
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the left position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int LeftOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateLeft)
                {
                    if (item == 24)
                    {
                        WhitePuzzlePieceLocation = i;
                        LeftOfPuzzlePieceLocation = i - 1;
                    }
                    i++;
                }
                //if the position is able to move create new node and add to children
                if (WhitePuzzlePieceLocation != 0 && WhitePuzzlePieceLocation != 5 && WhitePuzzlePieceLocation != 10 && WhitePuzzlePieceLocation != 15 && WhitePuzzlePieceLocation != 20)
                {
                    int SwapValue1 = StartingStateLeft[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateLeft[LeftOfPuzzlePieceLocation];
                    StartingStateLeft[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateLeft[LeftOfPuzzlePieceLocation] = SwapValue1;
                    Children.Add(new Node(CopyState(StartingStateLeft), Parent, false, Direction.Left, CopyMoves(Moves)));
                }
                //else dont make a new node and the left move treenode ends here
                else
                {
                    //Debug.Write("\nNode :\n" + NodeID + "\nEnded with a Left Move.");
                }
            }
        }
        private void Right(List<int> StartingStateRight)
        {
            //Debug.WriteLine("\nRight on : " + NodeID);
            //Check for completion
            if (ReachedFinalState(StartingStateRight) == true)
            {
                Parent.SetEndingNode(new Node(StartingStateRight, Parent, true, Direction.None, Moves));
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the right position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int RightOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateRight)
                {
                    if (item == 24)
                    {
                        WhitePuzzlePieceLocation = i;
                        RightOfPuzzlePieceLocation = i + 1;
                    }
                    i++;
                }
                //if the position is able to move create new node and add to children
                if (WhitePuzzlePieceLocation != 4 && WhitePuzzlePieceLocation != 9 && WhitePuzzlePieceLocation != 14 && WhitePuzzlePieceLocation != 19 && WhitePuzzlePieceLocation != 24)
                {
                    int SwapValue1 = StartingStateRight[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateRight[RightOfPuzzlePieceLocation];
                    StartingStateRight[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateRight[RightOfPuzzlePieceLocation] = SwapValue1;
                    Children.Add(new Node(CopyState(StartingStateRight), Parent, false, Direction.Right, CopyMoves(Moves)));
                }
                //else dont make a new node and the right move treenode ends here
                else
                {
                    //Debug.Write("\nNode :\n" + NodeID + "\nEnded with a Right Move.");
                }
            }
        }
        private void Up(List<int> StartingStateUp)
        {
            //Debug.WriteLine("\nUp on : " + NodeID);
            //Check for completion
            if (ReachedFinalState(StartingStateUp) == true)
            {
                Parent.SetEndingNode(new Node(StartingStateUp, Parent, true, Direction.None, Moves));
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the up position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int UpOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateUp)
                {
                    if (item == 24)
                    {
                        WhitePuzzlePieceLocation = i;
                        UpOfPuzzlePieceLocation = i - 5;
                    }
                    i++;
                }
                //if the position is able to move create new node and add to children
                if (WhitePuzzlePieceLocation != 0 && WhitePuzzlePieceLocation != 1 && WhitePuzzlePieceLocation != 2 && WhitePuzzlePieceLocation != 3 && WhitePuzzlePieceLocation != 4)
                {
                    int SwapValue1 = StartingStateUp[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateUp[UpOfPuzzlePieceLocation];
                    StartingStateUp[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateUp[UpOfPuzzlePieceLocation] = SwapValue1;
                    Children.Add(new Node(CopyState(StartingStateUp), Parent, false, Direction.Up, CopyMoves(Moves)));
                }
                //else dont make a new node and the Up move treenode ends here
                else
                {
                    //Debug.Write("\nNode :\n" + NodeID + "\nEnded with a Up Move.");
                }
            }
        }
        private void Down(List<int> StartingStateDown)
        {
            //Debug.WriteLine("\nDown on : " + NodeID);
            //Check for completion
            if (ReachedFinalState(StartingStateDown) == true)
            {
                Parent.SetEndingNode(new Node(StartingStateDown, Parent, true, Direction.None, Moves));
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the down position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int DownOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateDown)
                {
                    if (item == 24)
                    {
                        WhitePuzzlePieceLocation = i;
                        DownOfPuzzlePieceLocation = i + 5;
                    }
                    i++;
                }
                //if the position is able to move create new node and add to children
                if (WhitePuzzlePieceLocation != 20 && WhitePuzzlePieceLocation != 21 && WhitePuzzlePieceLocation != 22 && WhitePuzzlePieceLocation != 23 && WhitePuzzlePieceLocation != 24)
                {
                    int SwapValue1 = StartingStateDown[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateDown[DownOfPuzzlePieceLocation];
                    StartingStateDown[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateDown[DownOfPuzzlePieceLocation] = SwapValue1;
                    Children.Add(new Node(CopyState(StartingStateDown), Parent, false, Direction.Down, CopyMoves(Moves)));
                }
                //else dont make a new node and the Up move treenode ends here
                else
                {
                    //Debug.Write("\nNode :\n" + NodeID + "\nEnded with a Up Move.");
                }
            }
        }
        #endregion Moves
    }

    class Tree
    {
        #region Properties
        private List<int> _StartingState;
        private List<int> StartingState { get { return _StartingState; } set { _StartingState = value; GenerateStartingNode(value); } }
        private List<Node> StartingTree = new List<Node>();
        private Queue<Node> QueueNodes = new Queue<Node>();
        private Node _EndingNode;
        private bool EndingNodeSet = false;
        private Node EndingNode
        {
            get { return _EndingNode; }
            set
            {
                if (EndingNodeSet == false)
                {
                    EndingNodeSet = true;
                    _EndingNode = value;
                }
                else
                {
                    EndingNodes.Add(value);
                }
            }
        }
        private List<Node> EndingNodes = new List<Node>();
        private bool EndingNodeIsReached = false;
        private int NodesRan = 0;
        private List<int> NodeIds = new List<int>();
        private Random Random = new Random();
        //private List<List<int>> VisitedStates = new List<List<int>>();
        private List<string> VisitedStates = new List<string>();//has a flaw
        #endregion Properties
        public Tree(List<int> StartingState)
        {
            this.StartingState = StartingState;
            while (EndingNodeIsReached == false)
            {
                //lock (this)
                //{ 
                Task.Run(() =>
                {
                    if (QueueNodes.Count != 0)
                    {
                        lock (this)
                        {
                            QueueNodes.First().RunNode();
                        }
                        lock (this)
                        {
                            if (EndingNodeIsReached == true)
                            {
                                QueueNodes.Clear();
                                return;
                            }
                            QueueNodes.Dequeue();
                            NodesRan++;
                        }
                    }
                    Debug.WriteLine("<--        **** Nodes in Queue: " + QueueNodes.Count + " Nodes ran: " + NodesRan + " ****       -->");
                });
                //}

                GC.Collect();

                if (EndingNodeIsReached == true)
                {
                    break;
                }

            }
        }
        private void GenerateStartingNode(List<int> StartingState)
        {
            StartingTree.Add(new Node(StartingState, this, false));
        }
        public void SetEndingNode(Node EndingNode)
        {
            this.EndingNode = EndingNode;
            EndingNodeIsReached = true;
            Debug.WriteLine("EndingNode is reached!");
        }
        public bool CheckEndingNode()
        {
            return EndingNodeIsReached;
        }
        public void AddNodeToQueue(Node NewNode)
        {
            string State = ConvertToString(NewNode.StartingState);
            if (VisitedStates.Contains(State) == true)
            {
                Debug.WriteLine("Visited state");
            }
            else
            {
                VisitedStates.Add(State);
                NodeIds.Add(NewNode.NodeID);
                lock (this)
                {
                    QueueNodes.Enqueue(NewNode);
                }
            }
        }
        private string ConvertToString(List<int> startingState)
        {
            string state = "";
            foreach (int item in startingState)
            {
                state = state + item + ", ";
            }
            return state;
        }
        public int GetUniqueId()
        {
            int UniqueId = Random.Next();
            while (NodeIds.Contains(UniqueId) == true)
            {
                UniqueId = Random.Next();
            }
            return UniqueId;
        }
        public List<Direction> GetSolvingMoves()
        {
            return EndingNode.Moves;
        }
    }
}
