using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Presentation;
using GeneralUtils;
using UnityEngine;
using Pair = System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,
    _Game.Scripts.ModelV4.ECS.IReadOnlyComponent<_Game.Scripts.BurnMark.Game.Data.Components.TerrainData>>;

namespace _Game.Scripts.BurnMark.Game.Pathfinding {
    public class AStar : IPathFindingAlgorithm{
        private FieldAccessor _accessor;

        public void SetAccessor(FieldAccessor fieldAccessor) {
            _accessor = fieldAccessor;
        }

        public Vector2Int[] CalculatePath(Vector2Int from, Vector2Int to) {
            var source = Pair(from);
            var destination = Pair(to);

            var pathMap = new Dictionary<Pair, Step> { { source, GetFirstStep(source, destination) } };
            var foundPath = false;
            while (true) {
                var (currentNode, currentStep) = pathMap
                    .Aggregate((minPair, nodePair) =>
                        !nodePair.Value.Explored && (nodePair.Value.TotalWeight < minPair.Value.TotalWeight ||
                                                     minPair.Value.Explored)
                            ? nodePair
                            : minPair);

                if (currentNode.Key == destination.Key) {
                    foundPath = true;
                    break;
                }

                if (currentStep.Explored) {
                    break;
                }

                currentStep.Explored = true;
                UpdateWeights(currentNode, destination, pathMap);
            }

            if (!foundPath) {
                return null;
            }

            var path = new List<Vector2Int>();
            Pair? pathNode = destination;
            while (pathNode is {} node) {
                path.Add(node.Key);

                var currentStep = pathMap[node];
                pathNode = currentStep.PreviousPoint is { } point
                    ? Pair(point)
                    : (Pair?) null;
            }

            path.Reverse();
            return path.ToArray();
        }

        private void UpdateWeights(Pair around, Pair destination, Dictionary<Pair, Step> pathMap) {
            var aroundStep = pathMap[around];
            var adjacent = GetAdjacent(around);
            foreach (var node in adjacent) {
                if (aroundStep.PreviousPoint == node.Key) {
                    continue;
                }

                var newStep = GetStep(around, node, destination, pathMap);
                if (!pathMap.TryGetValue(node, out var existingStep)) {
                    pathMap[node] = newStep;
                    continue;
                }

                if (newStep.PathWeight >= existingStep.PathWeight) {
                    continue;
                }

                newStep.Explored = existingStep.Explored;
                pathMap[node] = newStep;

                if (newStep.Explored) {
                    UpdateWeights(node, destination, pathMap);
                }
            }
        }

        private static Step GetFirstStep(Pair node, Pair destination) {
            return new Step(0, GetHeuristicWeight(node, destination), null);
        }

        private static Step GetStep(Pair from, Pair to, Pair destination, IReadOnlyDictionary<Pair, Step> pathMap) {
            return new Step(pathMap[from].PathWeight + GetPathWeight(from, to), GetHeuristicWeight(from, destination), from.Key);
        }

        private static float GetHeuristicWeight(Pair from, Pair to) {
            var delta = Map(to.Key) - Map(from.Key);
            return delta.magnitude;

            Vector2 Map(Vector2Int position) => PositionMapper.Map(position, 1f);
        }

        private static float GetPathWeight(Pair from, Pair to) {
            return GetHeuristicWeight(from, to);
        }

        private IEnumerable<Pair> GetAdjacent(Pair node) {
            return DirectionUtils.Directions
                .Select(dir => dir.ToVector2Int(node.Key))
                .Select(OffsetNode)
                .Where(p => p != null)
                .Select(p => p.Value);

            Pair? OffsetNode(Vector2Int offset) => _accessor.Terrain.TryGetValue(node.Key + offset, out var value)
                ? new Pair(node.Key + offset, value)
                : (Pair?) null;
        }

        private Pair Pair(Vector2Int position) => new Pair(position, _accessor.Terrain[position]);

        private class Step {
            public readonly float PathWeight;
            public readonly float HeuristicWeight;
            public float TotalWeight => PathWeight + HeuristicWeight;

            public readonly Vector2Int? PreviousPoint;

            public bool Explored;

            public Step(float pathWeight, float heuristicWeight, Vector2Int? previousPoint) {
                PathWeight = pathWeight;
                HeuristicWeight = heuristicWeight;
                PreviousPoint = previousPoint;
                Explored = false;
            }
        }
    }
}