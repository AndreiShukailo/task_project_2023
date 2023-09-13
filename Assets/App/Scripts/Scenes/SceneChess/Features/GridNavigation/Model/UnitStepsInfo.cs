using System;
using System.Collections.Generic;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Model
{
    [Serializable]
    public class UnitStepsInfo
    {
        public List<int> X = new();
        public List<int> Y = new();
    }
}