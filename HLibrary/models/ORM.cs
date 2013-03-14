using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using MySql.Data.MySqlClient;
using HLibrary.helpers;
namespace HLibrary.models
{
    [Serializable]
    public class ORM
    {
        protected Dictionary<string, object> data = new Dictionary<string,object>();
        public static string table;
        public static string pk;

        public ORM(string table, string pk)
        {
            ORM.table = table;
            ORM.pk = pk;
        }

        public ORM()
        {
            ORM.table = "";
            ORM.pk = "";
        }

        public void populate(Dictionary<string, object> data)
        {
            this.data = data;
        }
        public void load()
        {
            populate(DB.instance().load_ORM(table));
        }

        public Boolean loaded()
        {
            return this.data != null;
        }
        public void load(string[] conditions)
        {
            populate(DB.instance().load_ORM(table, conditions));
        }
        public void load(List<string> conditions, string order_by)
        {
            populate(DB.instance().load_ORM(table, conditions, order_by));
        }
        
        public void load(List<String> conditions)
        {
            load(conditions.ToArray());
        }
        public static Dictionary<string, object> insert(ORM orm, string table)
        {
            return DB.instance().insert_ORM(orm.data,table);
        }
        public static List<Dictionary<string, object>> find_all(string [] conditions)
        {
            return DB.instance().find_all_ORM(ORM.table, conditions);
        }
        public static List<Dictionary<string, object>> find_all(List<string> conditions, string order_by)
        {
            return DB.instance().findAllORM(ORM.table, conditions, order_by);
        }
        public Boolean update()
        {
            return DB.instance().update(ORM.table, ORM.pk, data);
        }
        public void delete()
        {
            DB.instance().delete(ORM.table, ORM.pk, data[ORM.pk].ToString());
        }
        public string dumpValues()
        {
            string text = "";
            foreach (var value in data)
            {
                text += value.Key+"="+value.Value+"\r\n";
            }
            return text;
        }
        
      

    }
}
