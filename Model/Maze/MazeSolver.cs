using System;
using System.Collections.Generic;
using System.Linq;
using CityCourier.Model.Types;

namespace CityCourier.Model;

public class MazeSolver
{
    private static readonly int[] Dx = [1, 0, -1, 0];
    private static readonly int[] Dy = [0, 1, 0, -1];
    
    private int _rows;
    private int _cols;
    private int _maxParcelsToCollect;
    private int _maxTargetsToDeliver;
    
    private readonly Dictionary<(int, int, int, int), int> _distanceCache = new(1024);

    public int GetShortestPathLength(CellType[,] grid, int startX = 1, int startY = 1)
    {
        _rows = grid.GetLength(0);
        _cols = grid.GetLength(1);

        List<(int, int)> parcels = [];
        List<(int, int)> deliveryTargets = [];
        List<(int, int)> walls = [];

        for (var y = 0; y < _rows; y++)
        for (var x = 0; x < _cols; x++)
            switch (grid[y, x])
            {
                case CellType.Parcel:
                    parcels.Add((x, y));
                    break;
                case CellType.DeliveryTarget:
                    deliveryTargets.Add((x, y));
                    break;
                case CellType.House: 
                case CellType.Fence:
                    walls.Add((x, y));
                    break;
            }

        var sortedParcels = parcels.OrderBy(p => p.Item1).ThenBy(p => p.Item2).ToArray();
        var sortedTargets = deliveryTargets.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToArray();
        var sortedWalls = walls.OrderBy(h => h.Item1).ThenBy(h => h.Item2).ToArray();
        
        _maxParcelsToCollect = sortedParcels.Length;
        _maxTargetsToDeliver = sortedTargets.Length;
        if (_maxParcelsToCollect > _maxTargetsToDeliver) return -1;

        var initialState = new State(startX, startY, 0, sortedParcels, sortedTargets, sortedWalls);
        
        var openSet = new MinHeap();
        var bestCostToState = new Dictionary<State, int>(1024);
        
        openSet.Enqueue(new Node(initialState, 0, CalculateHeuristic(initialState)));
        bestCostToState[initialState] = 0;
        
        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            var currentState = current.State;
            
            if (currentState.RemainingParcels.Length == 0 && currentState.ParcelsCarrying == 0)
                return current.Cost;
            
            if (bestCostToState.TryGetValue(currentState, out var bestCost) && current.Cost > bestCost) continue;
            
            for (var dir = 0; dir < 4; dir++)
            {
                var newX = currentState.X + Dx[dir];
                var newY = currentState.Y + Dy[dir];
                
                if (newX < 0 || newX >= _cols || newY < 0 || newY >= _rows)
                    continue;
                
                var cellType = GetCellType(newX, newY, currentState);
                if (cellType == CellType.House) continue;
                
                ProcessMove(current, newX, newY, cellType, openSet, bestCostToState);
            }
        }
        
        return -1;
    }
    
    private static CellType GetCellType(int x, int y, State state)
    {
        if (Array.BinarySearch(state.Walls, (x, y)) >= 0)
            return CellType.House;
        
        if (Array.BinarySearch(state.RemainingDeliveryTargets, (x, y)) >= 0)
            return CellType.DeliveryTarget;
        
        if (Array.BinarySearch(state.RemainingParcels, (x, y)) >= 0)
            return CellType.Parcel;
        
        return CellType.Empty;
    }

    private void ProcessMove(
        Node currentNode, 
        int newX, int newY, 
        CellType cellType,
        MinHeap openSet, 
        Dictionary<State, int> bestCostToState)
    {
        var currentState = currentNode.State;
        var newCost = currentNode.Cost + 1;

        switch (cellType)
        {
            case CellType.Empty:
                var newState = new State(
                    newX, newY, 
                    currentState.ParcelsCarrying,
                    currentState.RemainingParcels,
                    currentState.RemainingDeliveryTargets,
                    currentState.Walls
                );
                
                AddNodeIfBetter(newState, newCost, openSet, bestCostToState);
                break;
                
            case CellType.Parcel:
                var parcelIndex = Array.BinarySearch(currentState.RemainingParcels, (newX, newY));
                if (parcelIndex >= 0)
                {
                    var newRemainingParcels = RemoveItemAtIndex(currentState.RemainingParcels, parcelIndex);
                    
                    var parcelState = new State(
                        newX, newY, 
                        currentState.ParcelsCarrying + 1,
                        newRemainingParcels,
                        currentState.RemainingDeliveryTargets,
                        currentState.Walls
                    );
                    
                    AddNodeIfBetter(parcelState, newCost, openSet, bestCostToState);
                }
                break;
                
            case CellType.DeliveryTarget:
                if (currentState.ParcelsCarrying > 0)
                {
                    var targetIndex = Array.BinarySearch(currentState.RemainingDeliveryTargets, (newX, newY));
                    if (targetIndex >= 0)
                    {
                        var newRemainingTargets = RemoveItemAtIndex(currentState.RemainingDeliveryTargets, targetIndex);
                        var newHouses = AddItemToSortedArray(currentState.Walls, (newX, newY));
                        
                        var deliveryState = new State(
                            currentState.X, currentState.Y,
                            currentState.ParcelsCarrying - 1,
                            currentState.RemainingParcels,
                            newRemainingTargets,
                            newHouses
                        );
                        
                        AddNodeIfBetter(deliveryState, newCost, openSet, bestCostToState);
                    }
                }
                break;
        }
    }
    
    private static (int, int)[] RemoveItemAtIndex((int, int)[] array, int index)
    {
        var length = array.Length;
        if (length == 1) return [];
        
        var result = new (int, int)[length - 1];
            
        if (index > 0)
            Array.Copy(array, 0, result, 0, index);
        if (index < length - 1)
            Array.Copy(array, index + 1, result, index, length - index - 1);
            
        return result;
    }
    
    
    private static (int, int)[] AddItemToSortedArray((int, int)[] array, (int, int) item)
    {
        var length = array.Length;
        var result = new (int, int)[length + 1];
        
        var insertIndex = 0;
        while (insertIndex < length 
               && (array[insertIndex].Item1 < item.Item1 
                   || (array[insertIndex].Item1 == item.Item1 
                       && array[insertIndex].Item2 < item.Item2)))
        {
            result[insertIndex] = array[insertIndex];
            insertIndex++;
        }
        
        result[insertIndex] = item;
        
        while (insertIndex < length)
        {
            result[insertIndex + 1] = array[insertIndex];
            insertIndex++;
        }
        
        return result;
    }

    private void AddNodeIfBetter(State state, int cost, MinHeap openSet, Dictionary<State, int> bestCostToState)
    {
        if (!bestCostToState.TryGetValue(state, out var bestCost) || cost < bestCost)
        {
            bestCostToState[state] = cost;
            openSet.Enqueue(new Node(state, cost, CalculateHeuristic(state)));
        }
    }

    
    private int CalculateHeuristic(State state)
    {
        var estimate = 0;
        
        if (state.RemainingParcels.Length > 0)
        {
            var minDistanceToParcel = state.RemainingParcels
                .Select(parcel => GetManhattanDistance(state.X, state.Y, parcel.Item1, parcel.Item2))
                .Min();

            estimate += minDistanceToParcel + state.RemainingParcels.Length;
        }
        
        if (state.ParcelsCarrying > 0)
        {
            var minDistanceToTarget = state.RemainingDeliveryTargets
                .Select(target => GetManhattanDistance(state.X, state.Y, target.Item1, target.Item2))
                .Min();

            estimate += minDistanceToTarget;
        }
        
        return estimate;
    }
    
    private int GetManhattanDistance(int x1, int y1, int x2, int y2)
    {
        var key = (x1, y1, x2, y2);
        
        if (!_distanceCache.TryGetValue(key, out int distance))
        {
            distance = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            _distanceCache[key] = distance;
        }
        
        return distance;
    }

    private readonly struct Node(State state, int cost, int heuristic) : IComparable<Node>
    {
        public readonly State State = state;
        public readonly int Cost = cost;
        
        private readonly int _totalCost = cost + heuristic;

        public int CompareTo(Node other) => _totalCost.CompareTo(other._totalCost);
    }
    
    private readonly struct State : IEquatable<State>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int ParcelsCarrying;
        
        public readonly (int, int)[] RemainingParcels;
        public readonly (int, int)[] RemainingDeliveryTargets;
        public readonly (int, int)[] Walls;
        
        private readonly int _hashCode;

        public State(int x, int y, int parcelsCarrying, 
                     (int, int)[] remainingParcels, 
                     (int, int)[] remainingDeliveryTargets,
                     (int, int)[] walls,
                     bool computeHash = true)
        {
            X = x;
            Y = y;
            ParcelsCarrying = parcelsCarrying;
            RemainingParcels = remainingParcels;
            RemainingDeliveryTargets = remainingDeliveryTargets;
            Walls = walls;
            
            _hashCode = computeHash ? ComputeHashCode() : 0;
        }

        private int ComputeHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + X;
                hash = hash * 31 + Y;
                hash = hash * 31 + ParcelsCarrying;
                
                hash = RemainingParcels
                    .Aggregate(hash, (current, t) => current * 31 + t.GetHashCode());
                hash = RemainingDeliveryTargets
                    .Aggregate(hash, (current, t) => current * 31 + t.GetHashCode());
                return Walls.Aggregate(hash, (current, t) => current * 31 + t.GetHashCode());
            }
        }

        public override int GetHashCode() => _hashCode;

        public bool Equals(State other)
        {
            if (X != other.X || Y != other.Y 
                             || ParcelsCarrying != other.ParcelsCarrying
                             || RemainingParcels.Length != other.RemainingParcels.Length 
                             || RemainingDeliveryTargets.Length != other.RemainingDeliveryTargets.Length
                             || Walls.Length != other.Walls.Length) return false;
            
            if (RemainingParcels.Where((t, i) => !t.Equals(other.RemainingParcels[i])).Any())
                return false;
            
            for (var i = RemainingDeliveryTargets.Length - 1; i >= 0; i--)
                if (!RemainingDeliveryTargets[i].Equals(other.RemainingDeliveryTargets[i])) return false;
            
            for (var i = Walls.Length - 1; i >= 0; i--)
                if (!Walls[i].Equals(other.Walls[i])) return false;
            
            return true;
        }

        public override bool Equals(object obj) => obj is State other && Equals(other);
    }

    // Custom priority queue that's more efficient than the standard one
    private class MinHeap(int capacity = 1024)
    {
        private Node[] _nodes = new Node[capacity];

        public int Count { get; private set; }

        public void Enqueue(Node node)
        {
            if (Count == _nodes.Length)
                Array.Resize(ref _nodes, _nodes.Length * 2);
                
            _nodes[Count] = node;
            SiftUp(Count++);
        }
        
        public Node Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("The heap is empty");
                
            var result = _nodes[0];
            _nodes[0] = _nodes[--Count];
            if (Count > 0) SiftDown(0);
                
            return result;
        }
        
        private void SiftUp(int index)
        {
            var node = _nodes[index];
            var parentIndex = (index - 1) / 2;
            
            while (index > 0 && node.CompareTo(_nodes[parentIndex]) < 0)
            {
                _nodes[index] = _nodes[parentIndex];
                index = parentIndex;
                parentIndex = (index - 1) / 2;
            }
            
            _nodes[index] = node;
        }
        
        private void SiftDown(int index)
        {
            var childIndex = index * 2 + 1;
            var node = _nodes[index];
            
            while (childIndex < Count)
            {
                if (childIndex + 1 < Count && _nodes[childIndex + 1].CompareTo(_nodes[childIndex]) < 0)
                    childIndex++;
                    
                if (node.CompareTo(_nodes[childIndex]) <= 0)
                    break;
                    
                _nodes[index] = _nodes[childIndex];
                index = childIndex;
                childIndex = index * 2 + 1;
            }
            
            _nodes[index] = node;
        }
    }
}