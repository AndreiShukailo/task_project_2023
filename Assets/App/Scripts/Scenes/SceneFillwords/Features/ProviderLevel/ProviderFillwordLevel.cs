using System;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private const string LevelsConfigPath = "Fillwords/pack_0";
        private const string DictionaryPath = "Fillwords/words_list";
        
        private string[] _dictionary;
        private string[] _levelsConfig;
        
        public GridFillWords LoadModel(int index)
        {
            LoadConfigs();
            
            var  currentLevelId = index - 1;
            if (InvalidLevelId(currentLevelId))
                throw new Exception();
            
            var nextIndex = index + 1;
            
            var levelConfig = GetLevelConfig(currentLevelId);
            if(levelConfig == null)
                return LoadModel(nextIndex);
            
            var size = GetSize(levelConfig);

            if (!TryBuildGrid(size, out var gridSize)) 
                return LoadModel(nextIndex);

            var rez = new GridFillWords(new Vector2Int(gridSize, gridSize));
            for (int i = 0; i < levelConfig.Length - 1; i++)
            {
                var word = GetWord(levelConfig[i]);
                var letterPositions = levelConfig[i + 1].Split(';');
                
                if (word.Length != letterPositions.Length) 
                    return LoadModel(nextIndex);

                for (int j = 0; j < word.Length; j++)
                {
                    var letterPosition = int.Parse(letterPositions[j]);
                    
                    if (letterPosition >= size) 
                        return LoadModel(nextIndex);
                    
                    var gridPosition = ConvertPositionToGrid(letterPosition, gridSize);
                    
                    if (rez.Get(gridPosition.x, gridPosition.y) != null) 
                        return LoadModel(nextIndex);
                    
                    rez.Set(gridPosition.x, gridPosition.y, new CharGridModel(word[j]));
                }
                i++;
            }

            return rez;
        }

        private bool InvalidLevelId(int index)
        {
            return index > _levelsConfig.Length || index < 0;
        }

        private string[] GetLevelConfig(int levelId)
        {
            var config = _levelsConfig[levelId].Split(' ');
            return config.Length < 2 ? null : config;
        }

        private int GetSize(string[] levelConfig)
        {
            var size = 0;
            for (var i = 0; i < levelConfig.Length - 1; i++)
            {
                var word = GetWord(levelConfig[i]);
                size += word.Length;
                i++;
            }

            return size;
        }

        private string GetWord(string index)
        {
            var wordIndex = int.Parse(index);
            var word = _dictionary[wordIndex];
            return word;
        }

        private void LoadConfigs()
        {
            if (_dictionary != null || _levelsConfig != null) return;
            _dictionary = ParseFile(DictionaryPath);
            _levelsConfig = ParseFile(LevelsConfigPath);
        }

        private Vector2Int ConvertPositionToGrid(int position, int gridSize)
        {
            var x = Mathf.CeilToInt(position / gridSize);
            var y = position - (x * gridSize);
            return new Vector2Int(x, y);
        }

        private bool TryBuildGrid(int size, out int grid)
        {
            var gridSize = Mathf.Sqrt(size);
            var ceilSize = Mathf.CeilToInt(gridSize);
            grid = ceilSize;
            return gridSize / ceilSize <= 1;
        }

        private string[] ParseFile(string path)
        {
            var textFile = Resources.Load<TextAsset>(path);
            var rez = textFile.text.Split("\r\n");
            return rez;
        }
    }
}