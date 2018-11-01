using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Juridical
{
    public class JuridicalPresentation
    {
        public async System.Threading.Tasks.Task<JuridicalModel> EndJuridical(JuridicalFilters juridicalFilter)
        {
            JuridicalLogic juridicalLogic = new JuridicalLogic();
            return await juridicalLogic.ReturnResult(juridicalFilter);
        }

        public ReturnJson EndSaveStatus(StatusInfo statusInfo, int user_id)
        {
            JuridicalLogic juridicalLogic = new JuridicalLogic();
            return juridicalLogic.SaveStatusLogic(statusInfo, user_id);
        }

        public List<JuridicalStatus> EndStatusInfo(int card_id)
        {
            JuridicalLogic StatusLogic = new JuridicalLogic();
            return StatusLogic.StatusInfoLogic(card_id);
        }
        public List<JuridicalLogging> EndJuridicalLoggings(int card_id)
        {
            JuridicalLogic juridicalLogicLogging = new JuridicalLogic();
            return juridicalLogicLogging.JuridicalLoggingsLogic(card_id);
        }
    }
}