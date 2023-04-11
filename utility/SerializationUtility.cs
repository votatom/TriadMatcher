using System;
using System.Collections.Generic;
using System.IO;
using Triad_Matcher.objects;

namespace Triad_Matcher.utility
{
    /// <summary>
    /// Utility to serialize Levels and GameObjects to a specific file.
    /// </summary>
    public class SerializationUtility
    {
        /// <summary>
        /// path
        /// Determinates base path where to find and save a Level file.
        /// </summary>
        private static string path = "levels/level{0}.txt";

        /// <summary>
        /// Saves informations about Level in a txt file
        /// </summary>
        /// <param name="level">
        /// Specific Level to be saved
        /// </param>
        public static void SerializeLevel(Level level)
        {
            String filePath = String.Format(path, level.Id);
            FileStream file = File.OpenWrite(filePath);
            StreamWriter writer = new StreamWriter(file);
            writer.Write(level.Serialize());
            writer.Close();
            file.Close();
        }

        /// <summary>
        /// Reads informations in a Level file and creates an instance of Level
        /// </summary>
        /// <param name="levelId">
        /// Id of a selected Level
        /// </param>
        /// <returns>
        /// Instance of class Level
        /// </returns>
        /// <exception cref="InsufficientDataException">
        /// If data in file isn't valid, throws InsufficientDataExcception
        /// </exception>
        public static Level DeserializeLevel(int levelId)
        {
            String filePath = String.Format(path, levelId.ToString());
            StreamReader reader = new StreamReader(filePath);
            if(reader.ReadLine() == "level" + levelId + ".txt")
            {
                String? line = "";
                List<GameObject> objects = new List<GameObject>();
                List<List<GameObject>> gameplan = new List<List<GameObject>>();
                do
                {
                    line = reader.ReadLine();
                    if(line.Split("|").Length <= 1){
                        break;
                    }
                    List<GameObject> lineObj = new List<GameObject>();
                    foreach (string character in line.Split("|"))
                    {
                        if(!objects.Contains(new GameObject(character))){
                            objects.Add(new GameObject(character));
                        }
                        lineObj.Add(FindGameObject(character,objects));
                    }
                    gameplan.Add(lineObj);
                } while (line.Split("|").Length > 1);
                String background = line;
                line = reader.ReadLine();
                String item = line;
                GameObject[] gameObjects = GetArray(gameplan);
                return new Level(gameObjects, background, levelId, item);
            }
            else
            {
                throw new InsufficientDataException("Data in file "+(new Uri(filePath).AbsolutePath)+" are not valid");
            }
        }
        /// <summary>
        /// Finds GameObject in array for further uses
        /// </summary>
        /// <param name="symbol">
        /// String representation of Symbol
        /// </param>
        /// <param name="listGameObjects">
        /// List of GameObjects to find a specific GameObject
        /// </param>
        /// <returns>
        /// Specific GameObject or null if it doesnt find anything
        /// </returns>
        public static GameObject FindGameObject(string symbol, List<GameObject> listGameObjects)
        {
            foreach (GameObject gameObject in listGameObjects)
            {
                if(gameObject.Symbol.ToString() == symbol)
                {
                    return gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates Array of GameObjects form a list of GameObjects
        /// </summary>
        /// <param name="list">
        /// List of GameObjects
        /// </param>
        /// <returns>
        /// Array of GameObjects
        /// </returns>
        public static GameObject[] GetArray(List<List<GameObject>> list)
        {
            GameObject[] mainArray = new GameObject[list.Count*list.Count];
            int index = 0;
            foreach (List<GameObject> gameObjects in list)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    mainArray[index] = gameObject;
                    index++;
                    if(index >= list.Count * list.Count)
                    {
                        break;
                    }
                }
            }
            return mainArray;
        }
    }
}
