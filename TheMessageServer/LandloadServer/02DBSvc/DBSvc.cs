//数据库服务
using MySql.Data.MySqlClient;
using PEProtocol;
using System;
using System.Collections.Generic;
using System.Text;


public class DBSvc
{
    private static DBSvc instance = null;
    public static DBSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBSvc();

            }
            return instance;
        }
    }
    private MySqlConnection conn;
    public void Init()
    {
        conn = new MySqlConnection("Data Source = localhost;user Id = root;password = root;Database = Landlord;Charset = utf8");
        conn.Open();
    }

    public void Update()
    {

    }

    public PlayerData QueryAcctDataByAcctPass(string acct, string pass)
    {
        PlayerData playerData = null;
        MySqlDataReader reader = null;
        bool acctExist = true;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where acct=@acct", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            reader = cmd.ExecuteReader();

            
            if (reader.Read())
            {
                string _pass = reader.GetString("pass");
                if (_pass.Equals(pass))
                {
                    int _id = reader.GetInt32("id");
                    string _name = reader.GetString("name");
                    int _lv = reader.GetInt32("lv");
                    int _exp = reader.GetInt32("exp");
                    int _coin = reader.GetInt32("coin");
                    int _diamond = reader.GetInt32("diamond");
                    int _win = reader.GetInt32("win");
                    int _lose = reader.GetInt32("lose");
                    int _winlast = reader.GetInt32("winlast");
                    int _iconIndex = reader.GetInt32("icon_index");

                    playerData = new PlayerData
                    {
                        id = _id,
                        name = _name,
                        lv = _lv,
                        exp = _exp,
                        coin = _coin,
                        diamond = _diamond,
                        win = _win,
                        lose = _lose,
                        winlast = _winlast,
                        iconIndex = _iconIndex

                    };
                }
            }
            else
            {
                acctExist = false;
            }
        }
        catch(Exception e)
        {
            this.Error("Query AcctData By Acct&Pass Error:{0}", e);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if(acctExist == false)
            {
                playerData = new PlayerData
                {
                    id = -1,
                    name = "default:"+acct,
                    lv = 1,
                    exp = 35,
                    coin = 5000,
                    diamond = 50,
                    win = 0,
                    lose = 0,
                    winlast = 0,
                    iconIndex = 0
                };
                int _id = InsertNewAcctData(acct, pass, playerData);
                playerData.id = _id;

            }
        }
        return playerData;
    }

    private int InsertNewAcctData(string acct,string pass,PlayerData playerData)
    {
        int id = -1;
        try{
            MySqlCommand cmd = new MySqlCommand("insert into account set acct =@acct,pass=@pass,name=@name,lv=@lv,exp=@exp,coin=@coin,diamond=@diamond,win=@win,lose=@lose,winlast=@winlast,icon_index=@icon_index",conn);

            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("lv", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("win", playerData.win);
            cmd.Parameters.AddWithValue("lose", playerData.lose);
            cmd.Parameters.AddWithValue("winlast", playerData.winlast);
            cmd.Parameters.AddWithValue("icon_index", playerData.iconIndex);

            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;

        }
        catch(Exception e)
        {
            this.Error("Insert New AcctData Error:{0}", e);
        }
        return id;
    }
}
