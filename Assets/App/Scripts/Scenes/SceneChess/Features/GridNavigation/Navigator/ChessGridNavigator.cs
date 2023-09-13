using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using App.Scripts.Scenes.SceneChess.Features.GridNavigation.Model;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        private const string UnitSteps = "Chess/UnitSteps/";
        private const string PonSteps = "Pon";
        private const string KingSteps = "King";
        private const string QueenSteps = "Queen";
        private const string RookSteps = "Rook";
        private const string KnightSteps = "Knight";
        private const string BishopSteps = "Bishop";

        private readonly Vector2Int _nullVector = new(-1, -1);
        
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            var unitSteps = LoadUnitStepInfo(unit);

            var path = GetTracks(unit, from, to, grid, unitSteps);

            var result = GetPath(to, path);

            return result;
        }

        private List<Vector2Int> GetPath(Vector2Int to, Dictionary<Vector2Int, Vector2Int> path)
        {
            var pathItem = to;
            var result = new List<Vector2Int>();
            while (pathItem != _nullVector)
            {
                result.Add(pathItem);
                if (!path.ContainsKey(pathItem))
                    return null;
                pathItem = path[pathItem];
            }

            result.Reverse();
            result.Remove(result[0]);
            return result;
        }

        private Dictionary<Vector2Int, Vector2Int> GetTracks(ChessUnitType unit, Vector2Int from, Vector2Int to,
            ChessGrid grid, UnitStepsInfo unitSteps)
        {
            var track = new Dictionary<Vector2Int, Vector2Int>();
            track[from] = _nullVector;
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(from);
            while (queue.Count != 0)
            {
                var point = queue.Dequeue();
                var dictionary = new SortedDictionary<float, Vector2Int>();
                for (var i = 0; i < unitSteps.X.Count; i++)
                {
                    var newStep = point + new Vector2Int(unitSteps.X[i], unitSteps.Y[i]);
                    if (track.ContainsKey(newStep)) continue;

                    if (IsStepValid(point, newStep, grid, unit))
                    {
                        track[newStep] = point;
                        if (track.ContainsKey(to)) break;
                        var distance = Vector2Int.Distance(to, newStep);
                        dictionary[distance] = newStep;
                    }
                }

                foreach (var item in dictionary)
                    queue.Enqueue(item.Value);
            }

            return track;
        }

        private bool IsStepValid(Vector2Int from, Vector2Int to, ChessGrid grid, ChessUnitType unitType)
        {
            var size = grid.Size.x;

            return IsStepInRange(to, size) && IsWayIsEmpty(from, to, grid, unitType);
        }

        private static bool IsStepInRange(Vector2Int to, int size) =>
            (to.x >= 0 && to.x < size) && (to.y >= 0 && to.y < size);


        private bool IsWayIsEmpty(Vector2Int from, Vector2Int to, ChessGrid grid, ChessUnitType unitType)
        {
            if (unitType is ChessUnitType.Knight or ChessUnitType.King or ChessUnitType.Pon)
                return IsCellIsEmpty(to, grid);

            var dir = to - from;
            if (dir != Vector2Int.zero)
            {
                var multiplier = Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
                var normalDir = new Vector2Int(dir.x / multiplier, dir.y / multiplier);
                var newFrom = from;
                for (int i = 0; i < multiplier; i++)
                {
                    newFrom += normalDir;
                    if (!IsCellIsEmpty(newFrom, grid))
                        return false;
                }
            }

            return true;
        }

        private static bool IsCellIsEmpty(Vector2Int cell, ChessGrid grid) =>
            grid.Get(cell.y, cell.x) == null;

        private UnitStepsInfo LoadUnitStepInfo(ChessUnitType unit)
        {
            var unitPath = GetUnitPath(unit);
            var textFile = Resources.Load<TextAsset>(UnitSteps + unitPath);
            var rez = JsonUtility.FromJson<UnitStepsInfo>(textFile.text);
            if (rez == null)
                throw new Exception();

            return rez;
        }

        private string GetUnitPath(ChessUnitType unit)
        {
            var unitPath = unit switch
            {
                ChessUnitType.Pon => PonSteps,
                ChessUnitType.King => KingSteps,
                ChessUnitType.Queen => QueenSteps,
                ChessUnitType.Rook => RookSteps,
                ChessUnitType.Knight => KnightSteps,
                ChessUnitType.Bishop => BishopSteps,
                _ => ""
            };
            return unitPath;
        }
    }
}