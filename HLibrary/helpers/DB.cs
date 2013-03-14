using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using HLibrary.interfaces;
namespace HLibrary.helpers
{
    [Serializable]
    public class DB : MarshalByRefObject
    {
        private static MySqlConnection con;
        public static IServer server;
        private string prefix = "hr_";
        public Boolean connected;
        public static string latestError = "";
        public static string latestQuery = "";

        public static DB instance(IServer server)
        {
            DB _db = new DB();
            DB.server = server;
            return _db;
        }
        public static DB instance()
        {
            return new DB();
        }
        public void connect(string username, string password, string database)
        {
            if (connected)
            {
                connected = false;
            }
            string connectionString = "server=localhost; database=" + database + "; uid=" + username + "; password=" + password;
            DB.con = new MySqlConnection(connectionString);
            try
            {
                DB.con.Open();
            }
            catch (MySqlException exs)
            {
                latestError = exs.Message;
                connected = false;
                return;
            }
            connected = true;

        }
        public double getTotal(string table, List<string> conditions, string column)
        {

            double total = 0;
            string sql = "select SUM(" + column + ") from " + prefix + table;
            if (conditions.Count != 0)
            {
                sql += " WHERE ";
                for (int i = 0; i < conditions.Count; i++)
                {
                    sql += conditions[i];
                    if (i + 1 < conditions.Count)
                    {
                        sql += " AND ";
                    }
                }
            }
            MySqlCommand cmd = new MySqlCommand(sql, DB.con);
            try
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    try
                    {
                        total = rdr.GetDouble(0);
                    }
                    catch
                    {
                        total = 0;
                    }
                }
                rdr.Close();
            }
            catch
            {
                return 0;
            }
            return total;
        }
        public Dictionary<string, object> load_ORM(string table)
        {
            string cmdTxt = "select * from " + prefix + table;
            return load(cmdTxt);
        }
        public Dictionary<string, object> load(string query)
        {
            string cmdTxt = query;

            Dictionary<string, object> data = null;
            MySqlCommand cmd = new MySqlCommand(cmdTxt, DB.con);
            try
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                if (rdr.HasRows)
                {
                    data = new Dictionary<string, object>();
                    int columns = rdr.FieldCount;
                    for (int i = 0; i < columns; i++)
                    {
                        data.Add(rdr.GetName(i), rdr.GetValue(i));
                    }

                }
                rdr.Close();
            }
            catch
            {
            }
            return data;
        }
        public Dictionary<string, object> load_ORM(string table, string[] conditions)
        {
            string cmdTxt = "select * from " + prefix + table + " ";
            for (int i = 0; i < conditions.Length; i++)
            {
                if (i == 0)
                {
                    cmdTxt += " where ";
                }
                else if (i != 0 && i < conditions.Length)
                {
                    cmdTxt += " and ";
                }
                if (i < conditions.Length)
                {

                    cmdTxt += conditions[i];
                }

            }
            return load(cmdTxt);
        }
        public Dictionary<string, object> load_ORM(string table, List<string> conditions, string order_by)
        {
            string cmdTxt = "select * from " + prefix + table + " ";
            for (int i = 0; i < conditions.Count; i++)
            {
                if (i == 0)
                {
                    cmdTxt += " where ";
                }
                else if (i != 0 && i < conditions.Count)
                {
                    cmdTxt += " and ";
                }
                if (i < conditions.Count)
                {

                    cmdTxt += conditions[i];
                }

            }
            cmdTxt += " " + order_by;
            return load(cmdTxt);
        }
        public static string escape(String s)
        {
            s = s.Replace("\'", "\\'");
            return s;
        }
        public List<Dictionary<string, object>> findAllORM(string table, List<String> conditions, string order_by)
        {
            List<Dictionary<string, object>> datas = null;
            string query = "select * from " + prefix + table;
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (i == 0)
                    {
                        query += " where ";
                    }
                    else if (i != 0 && i < conditions.Count)
                    {
                        query += " and ";
                    }
                    if (i < conditions.Count)
                    {

                        query += conditions[i];
                    }

                }
            }
            query += order_by;
            DB.latestQuery = query;
            MySqlCommand cmd = new MySqlCommand(query, DB.con);
            try
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                bool hasRows = rdr.HasRows;
                if (hasRows)
                {
                    datas = new List<Dictionary<string, object>>();
                    int columnCount = rdr.FieldCount;
                    while (rdr.Read())
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        for (int i = 0; i < columnCount; i++)
                        {
                            data.Add(rdr.GetName(i), rdr.GetValue(i));
                        }
                        datas.Add(data);
                    }
                }
                rdr.Close();
            }
            catch { }
            return datas;
        }
        public void query(String query)
        {
            MySqlCommand cmd = new MySqlCommand(query, DB.con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch { }
        }
        public List<Dictionary<string, object>> find_all_ORM(string table, string[] conditions)
        {
            List<Dictionary<string, object>> datas = null;
            string query = "select * from " + prefix + table;
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (i == 0)
                    {
                        query += " where ";
                    }
                    else if (i != 0 && i < conditions.Length)
                    {
                        query += " and ";
                    }
                    if (i < conditions.Length)
                    {

                        query += conditions[i];
                    }

                }
            }
            MySqlCommand cmd = new MySqlCommand(query, DB.con);
            try
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                bool hasRows = rdr.HasRows;
                if (hasRows)
                {
                    datas = new List<Dictionary<string, object>>();
                    int columnCount = rdr.FieldCount;
                    while (rdr.Read())
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        for (int i = 0; i < columnCount; i++)
                        {
                            data.Add(rdr.GetName(i), rdr.GetValue(i));
                        }
                        datas.Add(data);
                    }
                }
                rdr.Close();
            }
            catch { }
            return datas;
        }
        public Boolean update(string table, string pk, Dictionary<string, object> data)
        {
            Boolean update = false;
            string query = "update " + prefix + table + " ";
            if (data != null)
            {
                int ctr = 0;
                foreach (var value in data)
                {
                    if (ctr < data.Count && ctr != 0)
                    {
                        query += ", ";
                    }
                    if (ctr == 0)
                    {
                        query += " set ";
                    }
                    if (value.Value.GetType() != typeof(DateTime))
                    {
                        query += value.Key + " = '" + DB.escape(value.Value.ToString()) + "'";
                    }
                    else
                    {
                        DateTime date = (DateTime)value.Value;
                        query += value.Key + " = '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }
                    ctr++;
                }
                query += " where " + pk + "='" + data[pk] + "'";
                MySqlCommand cmd = new MySqlCommand(query, DB.con);
                cmd.ExecuteNonQuery();
            }
            latestQuery = query;
            return update;
        }
        public Dictionary<string, object> insert_ORM(Dictionary<string, object> data, string table)
        {
            bool insert = false;
            string cmd = "insert into " + prefix + table;
            cmd += " (";
            int ctr = 1;
            foreach (var value in data)
            {
                cmd += DB.escape(value.Key);
                if (ctr < data.Keys.Count)
                {
                    cmd += ", ";
                }
                ctr++;
            }
            cmd += ") values (";

            ctr = 1;
            foreach (var value in data)
            {
                cmd += "'" + DB.escape(value.Value.ToString()) + "'";
                if (ctr < data.Count)
                {
                    cmd += ", ";
                }
                ctr++;
            }
            cmd += ")";
            DB.latestQuery = cmd;
            MySqlCommand query = new MySqlCommand(cmd, DB.con);
            query.ExecuteNonQuery();
            data["id"] = query.LastInsertedId;
            return data;
        }

        public void delete(string table, string pk, string value)
        {
            string query = "delete from " + prefix + table + " where " + pk + "= '" + value + "'";
            MySqlCommand cmd = new MySqlCommand(query, DB.con);
            cmd.ExecuteNonQuery();
        }
        public List<object> distinct(string table, string column, string[] conditions)
        {
            List<Object> objects = new List<object>();
            string query = "select distinct " + column + " from " + prefix + table + " ";
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (i == 0)
                    {
                        query += " where ";
                    }
                    else if (i != 0 && i < conditions.Length)
                    {
                        query += " and ";
                    }
                    if (i < conditions.Length)
                    {

                        query += conditions[i];
                    }

                }
            }
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, DB.con);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    objects.Add(rdr.GetValue(0));
                }
                rdr.Close();
                DB.latestQuery = query;
            }
            catch { }
            return objects;
        }


    }
}
