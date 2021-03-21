using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

    static class MyDataBase
    {
        public static string NumberOfGroup;
        public static async System.Threading.Tasks.Task SQLReader()                                                      // Метод, устанавливающий номер группы, в случае если пользователь установил её ранее
        {
            HttpClient httpClient = new HttpClient();                                                                    // Создание экземпляра класса HttpClient для выполнения запроса
            string request = "https://api.telegram.org/bot1331699621:AAGdIFGkECZe-jbElPy8ikN8hefs9IqCbbk/getUpdates";
            HttpResponseMessage response = (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var rootObject = JObject.Parse(responseBody);
            
            string connstr = $"server=localhost;user=root;database=TelegramBot;password={Environment.GetEnvironmentVariable("password")};";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            string chat_id = rootObject["result"][rootObject["result"].ToArray().Length - 1]["message"]["chat"]["id"].ToString(); // Получения chat id человека, написавшего боту последним
            string sql1 = $"SELECT EXISTS(SELECT group_num FROM users WHERE chat_id = {chat_id} LIMIT 1);";
            MySqlCommand command1 = new MySqlCommand(sql1, conn);
            string NumberOfGroup1 = command1.ExecuteScalar().ToString();
            if (NumberOfGroup1 == "1")                                                                        // Блок кода, выполняющийся при установленной пользователем группы
            {
                string sql = $"SELECT group_num FROM users WHERE chat_id = {chat_id} LIMIT 1;";                   // Команда в MySQL, возвращающая установленную пользователем группу
                MySqlCommand command = new MySqlCommand(sql, conn);
                NumberOfGroup = command.ExecuteScalar().ToString();
                conn.Close();
            }
            else
            {
                conn.Close();
                throw new Exception("We don't know your group number. Please, specify your group number or set it using the command /setgroup");   // В случае, если пользователь не установил свою группу, отправляем ему это сообщение
            }
        }

        public static async System.Threading.Tasks.Task SQLWriter(string group)                                   // Метод записи/изменения группы в базе данных
        {
            if (!IsGroup(group)) throw new Exception("You entered the group number incorrectly.Check the entered data and try again.");
            await ScheduleQuery.SQuery("Понедельник", "1", group);
            HttpClient httpClient = new HttpClient();
            string request = "https://api.telegram.org/bot1331699621:AAGdIFGkECZe-jbElPy8ikN8hefs9IqCbbk/getUpdates"; // Делаем запрос для получения сообщения, которое отправил пользователь
            HttpResponseMessage response = (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();                
            string responseBody = await response.Content.ReadAsStringAsync();
            var rootObject = JObject.Parse(responseBody);

            string connstr = $"server=localhost;user=root;database=TelegramBot;password={Environment.GetEnvironmentVariable("password")};";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            string chat_id = rootObject["result"][rootObject["result"].ToArray().Length - 1]["message"]["chat"]["id"].ToString();
            string sql1 = $"SELECT EXISTS(SELECT group_num FROM users WHERE chat_id = {chat_id} LIMIT 1);";     // Команда для проверки, известна ли группа пользователя, написавшего боту
            MySqlCommand command1 = new MySqlCommand(sql1, conn);
            string NumberOfGroup1 = command1.ExecuteScalar().ToString();
            if (NumberOfGroup1 != "1")                                                                          // Блок кода, выполняющийся при неустановленной пользователем группы
            {
                string sql = $"INSERT users(chat_id, group_num, name) VALUES({chat_id}, {group}, '{rootObject["result"][rootObject["result"].ToArray().Length - 1]["message"]["chat"]["first_name"]}'); ";  // Команда привязки номера группы к chat id текущего пользователя и telegram-бота
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteScalar();
                conn.Close();
            }
            else if(NumberOfGroup1 == "1")                                                                  // Блок кода, выполняющийся в случае перезаписи номера группы пользователем
            {
                string sql = $"UPDATE `telegrambot`.`users` SET `group_num` = '{group}' WHERE (`chat_id` = '{chat_id}');";  // Строка с командой перезаписи группы в базе данных
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteScalar();
                conn.Close();
            }
            else throw new Exception("We don't know your group number. Please, specify your group number or set it using the command /setgroup"); // Исключение, в случае неустановленной пользователем группы
        }

        public static bool IsGroup(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
        if (str.Length != 6) return false;
        return true;
        }
}