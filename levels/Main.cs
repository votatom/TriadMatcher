using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triad_Matcher.objects;
using Triad_Matcher.utility;

namespace Triad_Matcher
{
    public class Main
    {
        /// <summary>
        /// Used only once to save files with informations about levels, then this file will be deleted from a project
        /// </summary>
        /// <returns>
        /// Level (no longer used)
        /// </returns>
        public static Level MakeLevels()
        {
            GameObject mushroom = new GameObject("bq");
            GameObject cat = new GameObject("rq");
            GameObject ghost = new GameObject("yq");
            GameObject[] objectPlan=new GameObject[9];
            objectPlan[0] = cat;
            objectPlan[1] = mushroom;
            objectPlan[2] = mushroom;
            objectPlan[3] = cat;
            objectPlan[4] = cat;
            objectPlan[5] = mushroom;
            objectPlan[6] = mushroom;
            objectPlan[7] = cat;
            objectPlan[8] = cat;
            Level level = new Level(objectPlan, "level1_background.jpg", "Backpack");
            SerializationUtility.SerializeLevel(level);
            objectPlan = new GameObject[16];
            objectPlan[0] = cat;
            objectPlan[1] = mushroom;
            objectPlan[2] = mushroom;
            objectPlan[3] = cat;
            objectPlan[4] = cat;
            objectPlan[5] = mushroom;
            objectPlan[6] = mushroom;
            objectPlan[7] = cat;
            objectPlan[8] = cat;
            objectPlan[9] = cat;
            objectPlan[10] = ghost;
            objectPlan[11] = mushroom;
            objectPlan[12] = cat;
            objectPlan[13] = ghost;
            objectPlan[14] = mushroom;
            objectPlan[15] = ghost;
            level = new Level(objectPlan, "level2_background.png", "Backpack");
            SerializationUtility.SerializeLevel(level);
            return level;
            //SerializationUtility.WriteJson<Level>(level, "level1");

        }
    }
}
