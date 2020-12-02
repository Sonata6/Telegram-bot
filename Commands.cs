using System;
static class Commands
{
    public static string[] parts = new string[4];
    public static async System.Threading.Tasks.Task GetSchedule(string command)
    {
        string[] partsOfMessage = command.Split(' ');
        DateTime dt;
        DateTime d2 = new DateTime(DateTime.Now.Year, 9, 1);
        int currentWeek;
        DateTime d1 = DateTime.Today;
        currentWeek = (int)((((d1 - d2).TotalDays - 1) / 7) % 4) + 1;

        switch (partsOfMessage.Length)
        {
            case 4:
                for (int i = 0; i < partsOfMessage.Length; i++) parts[i] = partsOfMessage[i];
                break;

            case 3:
                if (DateTime.TryParse(partsOfMessage[1], out dt))
                {
                    string weekday = dt.ToString("dddd");
                    parts[1] = weekday.Substring(0, 1).ToUpper() + weekday.Substring(1);
                    currentWeek = (int)((((dt - d2).TotalDays - 1) / 7) % 4) + 1;
                    parts[2] = currentWeek.ToString();
                    parts[3] = partsOfMessage[2];
                }
                else
                {
                    await MyDataBase.SQLReader();
                    for (int i = 0; i < partsOfMessage.Length; i++) parts[i] = partsOfMessage[i];
                    parts[3] = MyDataBase.NumberOfGroup;
                }
                break;

            case 2:
                if (DateTime.TryParse(partsOfMessage[1], out dt))
                {
                    string weekday = dt.ToString("dddd");
                    parts[1] = weekday.Substring(0, 1).ToUpper() + weekday.Substring(1);
                    currentWeek = (int)((((dt - d2).TotalDays - 1) / 7) % 4) + 1;
                    parts[2] = currentWeek.ToString();
                    await MyDataBase.SQLReader();
                    parts[3] = MyDataBase.NumberOfGroup;
                }
                else if (partsOfMessage[1].CompareTo("tomorrow") == 0)
                {
                    DateTime today = DateTime.Now;
                    DateTime tommorow = today.AddDays(1);
                    string weekday = tommorow.ToString("dddd");
                    parts[1] = weekday.Substring(0, 1).ToUpper() + weekday.Substring(1);
                    currentWeek = (int)((((dt - d2).TotalDays - 1) / 7) % 4) + 1;
                    parts[2] = currentWeek.ToString();
                    await MyDataBase.SQLReader();
                    parts[3] = MyDataBase.NumberOfGroup;
                }
                else if (partsOfMessage[1].Length == 6 && MyDataBase.IsGroup(partsOfMessage[1]))
                {
                    parts[1] = ((Days)((byte)DateTime.Today.DayOfWeek)).ToString();
                    parts[2] = currentWeek.ToString();
                    parts[3] = partsOfMessage[1];
                }
                else
                {
                    await MyDataBase.SQLReader();
                    for (int i = 0; i < partsOfMessage.Length; i++) parts[i] = partsOfMessage[i];
                    parts[2] = currentWeek.ToString();
                    parts[3] = MyDataBase.NumberOfGroup;
                }
                break;

            case 1:
                await MyDataBase.SQLReader();
                parts[1] = ((Days)((byte)DateTime.Today.DayOfWeek)).ToString();
                parts[2] = currentWeek.ToString();
                parts[3] = MyDataBase.NumberOfGroup;
                break;

            default: throw new Exception("Incorrect data input");
        }


    }
    public static async System.Threading.Tasks.Task SetGroup(string command)
    {
        if (command.Split(' ').Length != 2) throw new Exception("You must specify your group number using \"/setgroup\"");
        string group = command.Split(' ')[1];
        await MyDataBase.SQLWriter(group);
    }

    public static async System.Threading.Tasks.Task Start()
    {
        parts[0] = "Welcome to BsuirScheduleBot!\n\n" +
            "Using this bot you can get the schedule of your or another group.\n\n" +
            "For information on available commands use /help";
    }
    public static async System.Threading.Tasks.Task Help()
    {
        parts[0] = "/setgroup GroupNumber. After that you do not have to specify your group number in the /getschedule command.\n" +
            "Example - /setgroup 950506.\nYou can also change the group number with this same command\n\n" +
            "/getschedule: you can use you can use the following forms:\n" +
            "1. /getschedule - returns today's schedule\n" +
            "2. /getschedule tomorrow - returns tomorrow's schedule\n" +
            "3. /getschedule \"CurrentDate\".\nExpample - /getschedule 01.11.2020\n" +
            "4. /getschedule \"WeekDay\".\nExpample /getschedule Пятница\n" +
            "5. /getschedule \"WeekDay\" \"WeekNumber\".\nExpample - /getschedule Пятница 4\n" +
            "You can also specify number of group at the end of the command\nExample - /getschedule 01.11.2020 950506";
    }

    public enum Days : byte
    {
        Понедельник = 1,
        Вторник = 2,
        Среда = 3,
        Четверг = 4,
        Пятница = 5,
        Суббота = 6,
        Воскресенье = 7
    }
    enum Dayseng : byte
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }
}