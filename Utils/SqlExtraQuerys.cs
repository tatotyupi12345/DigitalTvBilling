using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalTVBilling.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace DigitalTVBilling.Utils
{
    public class SqlExtraQuerys
    {
        public static User auth(User user)
        {
            //User user = null;
            string cs = ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                //"select * from Customers where city = @City"
                SqlCommand cmd = new SqlCommand("SELECT [id] ,[login] ,[password] ,[name] ,[type] ,[phone] ,[email] ,[hard_autorize] FROM[StereoPlusBilling].[book].[Users] WHERE[login] = @loginName AND password = @password; ", con);
                //SqlParameter param = new SqlParameter();
                //param.ParameterName = "@City";
                //param.Value = inputCity;
                cmd.Parameters.Add(new SqlParameter("@loginName", user.Login));
                cmd.Parameters.Add(new SqlParameter("@password", user.Password));
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //user = new User()
                    //{
                        user.Id = Convert.ToInt32(reader["id"]);
                        user.Login = reader["login"].ToString();
                        user.Password = reader["password"].ToString();
                        user.Name = reader["name"].ToString();
                        user.Type = Convert.ToInt32(reader["type"]);
                        user.Phone = reader["phone"].ToString();
                        user.Email = reader["email"].ToString();
                        user.HardAutorize = Convert.ToBoolean(reader["hard_autorize"]);

                    break;
                    //};
                    //string dat = reader["TowerIDs"].ToString();
                    //var json = new JavaScriptSerializer();
                    //ids_ = (List<TrapStat>)json.Deserialize<List<TrapStat>>(dat);
                }
                reader.Close();
            }

            return user;
        }
    }
}