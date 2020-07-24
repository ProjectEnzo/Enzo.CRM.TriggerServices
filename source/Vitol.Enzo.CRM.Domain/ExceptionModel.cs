using System;
using System.Collections.Generic;
using System.Text;

namespace Vitol.Enzo.CRM.Domain
{
    public class ExceptionModel
    {
        public string ComponentName { get; set; }
        public string ActionName { get; set; }
        public string CustomerName { get; set; }
        public int Id { get; set; }
        public string getExceptionFormat(string ex)
        {
            return $"{"Component:"}{this.ComponentName}{'|'}{"Action:"}{this.ActionName}{'|'}{'|'}{"CustomerName:"}{this.CustomerName}{'|'}{"Id:"}{this.Id}{'|'}{"CreatedDate:"}{DateTime.Now}{'|'}{"Exception:"}{ex}";
        }

        public string getExceptionFormat(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
