using MerkezBankasıAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace MerkezBankasıAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentRateController : ControllerBase
    {
        [HttpPost]
        public ResponseData Run(RequestData request)
        {
            ResponseData result = new ResponseData();

            try
            {
                string tcmbLink = string.Format("http://www.tcmb.gov.tr/kurlar/{0}.xml", request.IsToday ? "today" : string.Format("{2}{1}/{0}{1}{2}", request.Day.ToString().PadLeft(2, '0'), request.Month.ToString().PadLeft(2, '0'), request.Year));

                result.Data = new List<ResponseDataForex>();
                XmlDocument document = new XmlDocument();
                document.Load(tcmbLink);

                if (document.SelectNodes("Tarih_Date").Count < 1)
                {
                    result.Error = "currency information not found!";
                    return result;
                }
                foreach (XmlNode node in document.SelectNodes("Tarih_Date")[0].ChildNodes)
                {
                    ResponseDataForex dataForex = new ResponseDataForex();

                    dataForex.CurrencyCode = node.Attributes["CurrencyCode"].Value;
                    dataForex.Currency = node["Currency"].InnerText;
                    dataForex.Unit = int.Parse(node["Unit"].InnerText);
                    dataForex.ForexBuying = Convert.ToDecimal("0" + node["ForexBuying"].InnerText.Replace(".",","));
                    dataForex.ForexSelling = Convert.ToDecimal("0" + node["ForexSelling"].InnerText.Replace(".",","));
                    dataForex.BanknoteBuying = Convert.ToDecimal("0" + node["BanknoteBuying"].InnerText.Replace(".",","));
                    dataForex.BanknoteSelling = Convert.ToDecimal("0" + node["BanknoteSelling"].InnerText.Replace(".",","));

                    result.Data.Add(dataForex);
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }
    }
}
