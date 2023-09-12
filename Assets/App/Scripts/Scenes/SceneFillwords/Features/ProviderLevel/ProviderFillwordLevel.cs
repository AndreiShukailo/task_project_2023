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

        public ProviderFillwordLevel()
        {
            LoadConfigs();
        }

        public GridFillWords LoadModel(int index)
        {
            var  currentLevelId = index - 1;
            if (InvalidLevelId(currentLevelId))
                throw new Exception();
            
            var levelConfig = GetLevelConfig(currentLevelId);
            if(levelConfig == null)
                return LoadNextModel(currentLevelId);
            
            var size = GetSize(levelConfig);

            if (!TryBuildGrid(size, out var gridSize)) 
                return LoadNextModel(currentLevelId);

            return CreateModel(size, gridSize, levelConfig, currentLevelId);
        }

        private GridFillWords CreateModel(int size, int gridSize, string[] levelConfig, int currenLevelId)
        {
            var rez = new GridFillWords(new Vector2Int(gridSize, gridSize));
            for (int i = 0; i < levelConfig.Length - 1; i++)
            {
                var word = GetWord(levelConfig[i]);
                var letterPositions = GetLetterPositions(levelConfig[i+1]);
                if (WorldLengthNotEqualConfig(word, letterPositions))
                    return LoadNextModel(currenLevelId);

                for (int j = 0; j < word.Length; j++)
                {
                    var letterPosition = int.Parse(letterPositions[j]);
                    if (LetterOutOfRange(size, letterPosition))
                        return LoadNextModel(currenLevelId);

                    var gridPosition = ConvertPositionToGrid(letterPosition, gridSize);
                    if (GridHasLetter(rez, gridPosition))
                        return LoadNextModel(currenLevelId);

                    rez.Set(gridPosition.x, gridPosition.y, new CharGridModel(word[j]));
                }
                i++;
            }
            
            return rez;
        }

        private static string[] GetLetterPositions(string levelConfig) =>
            levelConfig.Split(';');

        private static bool WorldLengthNotEqualConfig(string word, string[] letterPositions) =>
            word.Length != letterPositions.Length;

        private static bool GridHasLetter(GridFillWords rez, Vector2Int gridPosition) =>
            rez.Get(gridPosition.x, gridPosition.y) != null;

        private static bool LetterOutOfRange(int size, int letterPosition) =>
            letterPosition >= size || letterPosition < 0;

        private GridFillWords LoadNextModel(int currentLevelId)
        {
            Debug.LogError($"Invalid level: ID - {currentLevelId}");
            var nextLevelIndex = currentLevelId + 2;
            
            return LoadModel(nextLevelIndex);
        }

        private bool InvalidLevelId(int index) => 
            index >= _levelsConfig.Length || index < 0;

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
            _dictionary = ParseFile(DictionaryPath);
            _levelsConfig = ParseFile(LevelsConfigPath);
        }

        private Vector2Int ConvertPositionToGrid(int position, int gridSize)
        {
            var x = Mathf.CeilToInt(position / gridSize);
            var y = position - x * gridSize;
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
            if (textFile == null)
                throw new Exception();
            var rez = textFile.text.Split("\r\n");
            return rez;
        }
    }
}