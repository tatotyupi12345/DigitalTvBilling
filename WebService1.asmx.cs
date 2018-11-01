using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace DigitalTVBilling
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public List<TrapStat> getStatConfig()
        //{
        //    List<TrapStat> ids_ = null;
        //    string cs = ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(cs))
        //    {

        //        SqlCommand cmd = new SqlCommand("select top 1 ID, TowerIDs FROM [statConfig] order by id desc", con);
        //        con.Open();

        //        SqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            string dat = reader["TowerIDs"].ToString();
        //            var json = new JavaScriptSerializer();
        //            ids_ = (List<TrapStat>)json.Deserialize<List<TrapStat>>(dat);
        //        }
        //        reader.Close();

        //    }

        //    return ids_;
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getAbonentNum()
        {
            string id = "";
            string cs = ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {

                SqlCommand cmd = new SqlCommand("generateRandomAbonentNum", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string dat = reader["generated_random_id"].ToString();
                    id = dat;
                }
                reader.Close();

            }

            return id;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getDocNum()
        {
            string id = "";
            string cs = ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {

                SqlCommand cmd = new SqlCommand("generateRandomDocNum", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string dat = reader["generated_random_id"].ToString();
                    id = dat;
                }
                reader.Close();

            }

            return id;
        }
    }
}
