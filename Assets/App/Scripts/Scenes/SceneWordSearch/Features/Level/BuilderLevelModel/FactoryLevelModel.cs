using System.Collections.Generic;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            var dictionary = new Dictionary<char, int>();
            foreach (var word in words)
            {
                var wordDictionary = CreateWordDictionary(word);
                UpdateDictionary(wordDictionary, dictionary);
            }
            
            return CreateLisChars(dictionary);
        }

        private List<char> CreateLisChars(Dictionary<char, int> dictionary)
        {
            var rez = new List<char>();
            foreach (var item in dictionary)
                for (var i = 0; i < item.Value; i++)
                    rez.Add(item.Key);

            return rez;
        }

        private void UpdateDictionary(Dictionary<char, int> wordDictionary, Dictionary<char, int> dictionary)
        {
            foreach (var item in wordDictionary)
            {
                if (dictionary.ContainsKey(item.Key))
                {
                    var maxCount = Mathf.Max(dictionary[item.Key], item.Value);
                    dictionary[item.Key] = maxCount;
                }
                else
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }
        }

        private static Dictionary<char, int> CreateWordDictionary(string word)
        {
            var wordDictionary = new Dictionary<char, int>();
            foreach (var letter in word)
            {
                if (wordDictionary.ContainsKey(letter))
                    wordDictionary[letter]++;
                else
                    wordDictionary.Add(letter, 1);
            }
            
            return wordDictionary;
        }
    }
}