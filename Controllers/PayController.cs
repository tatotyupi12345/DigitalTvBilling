using System.Linq;
using System.Text;
using System.Web.Mvc;
using DigitalTVBilling.Utils;
using DigitalTVBilling.Models;

namespace DigitalTVBilling.Controllers
{
    [RoutePrefix("Payment")]
    public class PayController : Controller
    {
        //private const string SecretKey = "PaY^#TV@&1";

        //private int Source_Id = 0;

        //[Route("Verify")]
        //[AcceptVerbs("POST")]
        //public JsonResult Verify(ClientInput input)
        //{

        //    //input = new ClientInput()
        //    //{
        //    //    CustomerId = "38067637",
        //    //    Hash = "",
        //    //    Password = "qwerty778"/*CalculateMD5Hash("qwerty778")*/,
        //    //    Username = "admin",
        //    //};

        //    //input.Password = Utils.GetMd5(input.Password);

        //    Result result = new Result();
        //    try
        //    {
        //        if (CheckInputParameters(input, true))
        //        {
        //            Source_Id = CheckUsernameAndPassword(input.Username, Utils.Utils.GetMd5(input.Password)); // Check Username/Password and Get Source ID
        //            if (Source_Id == 0 || input.Username != "novatech") // If 0, then Username or Password not correct
        //            {
        //                SetErrorCode(ref result, 2);
        //                result.Client = null;
        //                return Json(result , JsonRequestBehavior.AllowGet);
        //            }
        //            else if (CheckHash(input.Username + input.Password + SecretKey, input.Hash)) // Check Hash
        //            {
        //                SetErrorCode(ref result, 3);
        //                result.Client = null;

        //                return Json(result , JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                ClientInfo info = new ClientInfo();

        //                bool exists = GetClient(input.CustomerId, ref info); // Check Client
        //                if (!exists)
        //                {
        //                    SetErrorCode(ref result, 6);
        //                    result.Client = null;

        //                    return Json(result , JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    SetErrorCode(ref result, 0); // Verify OK
        //                    result.Client = info;
        //                    return Json(result , JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            SetErrorCode(ref result, 4); // Invalid Input Parameters
        //            result.Client = null;
        //            return Json(result , JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        SetErrorCode(ref result, 99);
        //        result.Client = null;
        //        return Json(result , JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[Route("Pay")]
        //[AcceptVerbs("POST")]
        //public JsonResult Pay(ClientInput input)
        //{
        //    //input = new ClientInput()
        //    //{
        //    //    CustomerId = "38067013",
        //    //    Hash = "",
        //    //    Password = "qwerty778"/*CalculateMD5Hash("qwerty778")*/,
        //    //    Username = "admin",
        //    //    Amount = 4,
        //    //    InvoiceId = "600012168"
        //    //};

        //    try
        //    {
        //        if (CheckInputParameters(input, false))
        //        {

        //            JsonResult jsresult = Verify(input);

        //            Result result = (Result)jsresult.Data;

        //            if (result.ErrorCode == 0) // Pay's Verify OK
        //            {
        //                if (input.Amount == 0m) // Check Amount
        //                {
        //                    SetErrorCode(ref result, 7);
        //                    result.Client = null;
        //                    return Json(result , JsonRequestBehavior.AllowGet);
        //                }
        //                else if (CheckForDublicateTransaction((long)Convert.ToInt64(input.InvoiceId)))
        //                {
        //                    SetErrorCode(ref result, 8); // Dublicate InvoiceId
        //                    result.Client = null;
        //                    return Json(result , JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    if (!PayMethod(Source_Id, input.CustomerId, input.InvoiceId, input.Amount)) // Pay
        //                    {
        //                        SetErrorCode(ref result, 9); // Payment Failed!
        //                        result.Client = null;
        //                        return Json(result , JsonRequestBehavior.AllowGet);
        //                    }
        //                    else
        //                    {
        //                        SetErrorCode(ref result, 0); // Pay OK
        //                        result.Client = null;
        //                        return Json(result , JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                return Json(result , JsonRequestBehavior.AllowGet); // Pay's Verify Failed
        //            }
        //        }
        //        else
        //        {
        //            Result result = new Result();
        //            SetErrorCode(ref result, 4); // Invalid Input Parameters
        //            result.Client = null;
        //            return Json(result , JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch
        //    {
        //        Result result = new Result();
        //        SetErrorCode(ref result, 9); // Payment Failed!
        //        result.Client = null;
        //        return Json(result , JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public string TestRequest()
        //{
        //    return "Test Request";
        //}

        //private bool PayMethod(int Source_Id, string CardNo, string InvoiceId, decimal Amount)
        //{
        //    try
        //    {
        //        PaymentData pay_data = new PaymentData();
        //        List<int> cards = new List<int>();

        //        using (DataContext _db = new DataContext())
        //        {
        //            Card _card = _db.Cards.Where(c => c.CardNum == CardNo).FirstOrDefault();

        //            if (_card == null)
        //                return false;

        //            cards.Add(_card.Id);
        //        }

        //        pay_data.Amount = Amount;
        //        pay_data.TransactionId = Convert.ToString(InvoiceId);
        //        pay_data.Cards = cards;
        //        pay_data.ClientID = "PayBox";

        //        PaymentController _pay = new PaymentController();
        //        int res = _pay.SavePayment(pay_data, Source_Id, true);

        //        if (res == 1)
        //            return true;

        //        return false;

        //    }
        //    catch(Exception ex)
        //    {
        //        throw;
        //    }
        //    //SqlCommand cmd = new SqlCommand();
        //    //try
        //    //{
        //    //    SqlConnection con = new SqlConnection(connString);

        //    //    cmd.CommandText = "[dbo].[SP_Pay]";
        //    //    cmd.CommandType = CommandType.StoredProcedure;
        //    //    cmd.Connection = con;

        //    //    cmd.Parameters.AddWithValue("@Source_Id", Source_Id);
        //    //    cmd.Parameters.AddWithValue("@Custumer_Id", CardNo);
        //    //    cmd.Parameters.AddWithValue("@Invoice_Id", InvoiceId);
        //    //    cmd.Parameters.AddWithValue("@Amount", Amount);

        //    //    cmd.Connection.Open();
        //    //    int rowCount = cmd.ExecuteNonQuery(); // Insert 
        //    //    cmd.Connection.Close();

        //    //    if (rowCount > 0) return true;
        //    //    return false;
        //    //}
        //    //catch
        //    //{
        //    //    if (cmd.Connection.State == ConnectionState.Open)
        //    //        cmd.Connection.Close();
        //    //    throw;
        //    //}
        //}

        //private bool GetClient(string custumer_id, ref ClientInfo info)
        //{
        //    try
        //    {
        //        using (DataContext _db = new DataContext())
        //        {
        //            Card _card = _db.Cards.Where(c => c.CardNum == custumer_id).FirstOrDefault();

        //            if (_card != null)
        //            {
        //                PayData paydata = new PayApiController().getCardsInfo(_card.AbonentNum);

        //                if (paydata != null)
        //                {
        //                    info.Balance = paydata.Groups[0].PayDataCards[0].Balance;
        //                    info.Amount = (decimal)Math.Round(paydata.Groups[0].PayDataCards[0].RecomendedPrice, 2);
        //                    info.Customer = paydata.PayDataAbonentInfo.Name;
        //                    info.CardNo = _card.CardNum;
        //                    info.ActiveDate = _card.CloseDate;

        //                    return true;
        //                }
        //            }
        //        }

        //        return false;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    //try
        //    //{
        //    //    SqlDataAdapter adp = new SqlDataAdapter();
        //    //    adp.SelectCommand = new SqlCommand();
        //    //    adp.SelectCommand.Connection = new SqlConnection(connString);
        //    //    adp.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
        //    //    adp.SelectCommand.CommandText = "[dbo].[SP_CheckCustomer]";

        //        //    adp.SelectCommand.Parameters.AddWithValue("@CardNo", custumer_id);

        //        //    DataTable clientInfo = new DataTable("ClientInfo");
        //        //    adp.Fill(clientInfo);
        //        //    adp.SelectCommand.Connection.Close();

        //        //    if (clientInfo.Rows.Count > 0)
        //        //    {
        //        //        string CardNo = (string)clientInfo.Rows[0]["CardNo"];
        //        //        string Customer = (string)clientInfo.Rows[0]["Customer"];
        //        //        decimal Amount = (decimal)clientInfo.Rows[0]["Amount"];
        //        //        decimal Balance = (decimal)clientInfo.Rows[0]["Balance"];
        //        //        DateTime ActiveDate = (DateTime)clientInfo.Rows[0]["ActiveDate"];

        //        //        info.Balance = Balance;
        //        //        info.Amount = Amount;
        //        //        info.Customer = Customer;
        //        //        info.CardNo = CardNo;
        //        //        info.ActiveDate = ActiveDate;

        //        //        return true;
        //        //    }
        //        //    return false;
        //        //}
        //        //catch
        //        //{
        //        //    throw;
        //        //}
        //}

        //private bool CheckForDublicateTransaction(long InvoiceId)
        //{
        //    //SqlCommand cmd = new SqlCommand();
        //    //try
        //    //{
        //    //    SqlConnection con = new SqlConnection(connString);

        //    //    cmd.CommandText = "[CheckForDublicateTransaction]";
        //    //    cmd.CommandType = CommandType.StoredProcedure;
        //    //    cmd.Connection = con;

        //    //    cmd.Parameters.AddWithValue("@Invoice_ID", InvoiceId);
        //    //    cmd.Parameters.Add("@exists", SqlDbType.Int).Direction = ParameterDirection.Output;

        //    //    cmd.Connection.Open();
        //    //    cmd.ExecuteNonQuery();
        //    //    cmd.Connection.Close();

        //    //    return (int)cmd.Parameters["@exists"].Value == 0 ? false : true;
        //    //}
        //    //catch
        //    //{
        //    //    if (cmd.Connection.State == ConnectionState.Open)
        //    //        cmd.Connection.Close();
        //    //    throw;
        //    //}

        //    try
        //    {
        //        using (DataContext _db = new DataContext())
        //        {
        //            if (!_db.PayTransactions.Any(t => t.TransactionId == InvoiceId.ToString()))
        //                return false;

        //            return true;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //private bool CheckInputParameters(ClientInput input, bool verifyOrPay)
        //{
        //    if (verifyOrPay) // verify
        //    {
        //        if (input == null || (input.CustomerId == null) || input.Hash == null || input.Username == null || input.Password == null) return false;
        //        {

        //            return true;
        //        }
        //    }
        //    else // Pay
        //    {
        //        if (CheckInputParameters(input, true))
        //        {
        //            if (input.InvoiceId == null) return false;
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        [NonAction]
        public int CheckUsernameAndPassword(string user, string pass)
        {
            //SqlCommand cmd = new SqlCommand();
            //try
            //{
            //    SqlConnection con = new SqlConnection(connString);

            //    cmd.CommandText = "CheckUsernameAndPassword";
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Connection = con;

            //    cmd.Parameters.AddWithValue("@Username", user);
            //    cmd.Parameters.AddWithValue("@Password", pass);
            //    cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();

            //    return cmd.Parameters["@Id"].Value == System.DBNull.Value ? 0 : (int)cmd.Parameters["@Id"].Value;
            //}
            //catch
            //{
            //    if (cmd.Connection.State == ConnectionState.Open)
            //        cmd.Connection.Close();
            //    throw;
            //}

            using (DataContext _db = new DataContext())
            {
                User usr = _db.Users.Where(u => u.Login == user && u.Password == pass).FirstOrDefault();

                if(usr!= null)
                {
                    return usr.Id;
                }

                return 0;
            }
        }

        //private bool CheckHash(string client_hash, string hash)
        //{
        //    if (CalculateMD5Hash(client_hash) != hash) return true;
        //    return false;
        //}

        //private void SetErrorCode(ref Result result, int errorCode)
        //{
        //    switch (errorCode)
        //    {
        //        case 0:
        //            result.ErrorCode = 0;
        //            result.ErrorDescription = "OK";
        //            break;
        //        case 2:
        //            result.ErrorCode = 2;
        //            result.ErrorDescription = "Invalid Username OR Password";
        //            break;
        //        case 3:
        //            result.ErrorCode = 3;
        //            result.ErrorDescription = "Invalid Hash Code";
        //            break;
        //        case 4:
        //            result.ErrorCode = 4;
        //            result.ErrorDescription = "Ivalid Parameters";
        //            break;
        //        case 6:
        //            result.ErrorCode = 6;
        //            result.ErrorDescription = "User Not Found";
        //            break;
        //        case 7:
        //            result.ErrorCode = 7;
        //            result.ErrorDescription = "Invalid Amount";
        //            break;
        //        case 8:
        //            result.ErrorCode = 8;
        //            result.ErrorDescription = "Dublicate Transaction";
        //            break;
        //        case 9:
        //            result.ErrorCode = 9;
        //            result.ErrorDescription = "Payment Failed";
        //            break;
        //        default:
        //            result.ErrorCode = 99;
        //            result.ErrorDescription = "General Error";
        //            break;
        //    }
        //}

        [NonAction]
        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}