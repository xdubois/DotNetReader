    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetReader.Utilities
{

    public static class util
    {


        public static Dictionary<int,string> getDisplayTypeDictonnary(){

             Dictionary<int, string> DisplayType = new Dictionary<int, string>();
                DisplayType.Add(0, "Only unread items"); //Affiche que les articles non-lu
                DisplayType.Add(1, "All items"); //Affiche tous les articles


            return DisplayType;
        }

        public static Dictionary<int,string> getSychroDictonary(){

            Dictionary<int, string> SynchroType = new Dictionary<int, string>();
            SynchroType.Add(0, "Full"); //Synchro full de tous les flux via cron sur fonction ou appel manuel
            SynchroType.Add(1, "Partial"); //Synchro les flux au click de celui-çi
            SynchroType.Add(2, "Manual");  //Synchro uniquement manuelle, empeche appel cron

            return SynchroType;

        }
               

    }

    enum SynchroType
    {
        Full,
        Partial,
        Manual
    }

    enum DisplayType
    {
        UnreadItems,
        AllItems

    }


}