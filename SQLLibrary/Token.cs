using System;
using System.Linq;

namespace SQL
{
    public class Token
    {
        public static string GetGeneratedNewToken(int minutesAlive)
        {
            DateTime timeAux = DateTime.Now;
            timeAux = timeAux.AddMinutes(minutesAlive);//Añadido el tiempo de vida en mminutos del token
            byte[] time = BitConverter.GetBytes(timeAux.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }
    }
}
