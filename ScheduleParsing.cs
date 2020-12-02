using System;
    static class ScheduleParsing
    {
        static public string Parse(Newtonsoft.Json.Linq.JObject rootObject, string weekday, string weeknumber)
        {
        if (weekday.CompareTo("0")==0) return "No classes today :)";
            string schedule = "";
            schedule += rootObject["schedules"][0]["schedule"][0]["studentGroupInformation"][0].ToString().Substring(21);
            schedule += "\n";
            foreach (var day in rootObject["schedules"])
            {
                if (day["weekDay"].ToString() == weekday)
                {
                    foreach (var number in day["schedule"])
                    {
                        foreach (var weekN in number["weekNumber"])
                        {
                            if (weekN.ToString().CompareTo(weeknumber) == 0)
                            {
                                if (number["numSubgroup"].ToString().CompareTo("0") != 0)
                                    schedule += "Подгруппа №" + number["numSubgroup"] + ". ";
                                schedule += number["lessonTime"] + " ";
                                schedule += number["subject"];
                                schedule += "(" + number["lessonType"] + ").";
                                if (number["subject"].ToString().CompareTo("СпецПодг") != 0 && number["subject"].ToString().CompareTo("ФизК") != 0)
                                {
                                    schedule += " Аудитория: ";
                                    schedule += number["auditory"][0] + ". ";
                                    schedule += number["employee"][0]["fio"];
                                    schedule += "\n";
                                }
                                else schedule += "\n";
                            }
                        }
                    }
                }
            }
        if (schedule == rootObject["schedules"][0]["schedule"][0]["studentGroupInformation"][0].ToString().Substring(21) + "\n") 
            throw new Exception("You used the /getschedule command incorrectly.\nTo see the use cases, use the /help command");
        return schedule;
        }
    }