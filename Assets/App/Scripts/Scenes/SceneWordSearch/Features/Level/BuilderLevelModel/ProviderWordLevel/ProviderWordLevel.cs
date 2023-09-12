using System;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        private const string LevelsPath = "WordSearch/Levels/";

        public LevelInfo LoadLevelData(int levelIndex)
        {
            var textFile = Resources.Load<TextAsset>(LevelsPath + levelIndex);
            var rez = JsonUtility.FromJson<LevelInfo>(textFile.text);
            if (rez == null)
                throw new Exception();
            
            return rez;
        }
    }
}