using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplicationStoreAutoOffice
{
    internal class ActiveDirectory
    {
        private static string path = @ConfigurationManager.AppSettings["IP_ACTIVE_DIRECTORY"].ToString();
        private static string domain = @ConfigurationManager.AppSettings["DOMAIN_ACTIVE_DIRECTORY"].ToString();

        public static Tuple<bool, string> AuthenticateActiveDirectory(string userAlias, string userPassword)
        {
            string domainUser = domain + @"\" + userAlias;               //CADENA DE DOMINIO + USUARIO A COMPROBAR
            return autenticaUsuario(path, domainUser, userPassword);
        }

        private static Tuple<bool, string> autenticaUsuario(string path, string user, string pass)
        {
            //Los datos que hemos pasado los 'convertimos' en una entrada de Active Directory para hacer la consulta
            DirectoryEntry directoryEntry = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
            try
            {
                //Inicia el chequeo con las credenciales que le hemos pasado
                //Si devuelve algo significa que ha autenticado las credenciales
                DirectorySearcher ds = new DirectorySearcher(directoryEntry);
                ds.FindOne();
                return new Tuple<bool, string>(true, "Usuario existe en directoio activo.");
            }
            catch(Exception ex)
            {
                //Si no devuelve nada es que no ha podido autenticar las credenciales
                //ya sea porque no existe el usuario o por que no son correctas
                return new Tuple<bool, string>(false, ex.Message);
            }
        }
    }
}
