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
            var textFile = Resources.Load<TextAsset>(LevelsPath + levelIndex.ToString());
            if (textFile == null) 
                throw new Exception();
            
            var rez = JsonUtility.FromJson<LevelInfo>(textFile.text);
            return rez;
        }
    }
}