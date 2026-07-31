using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrogueriaPOS.Domain.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPasswordHasher
    {
        ///<summary>
        /// Hashea un password en texto plano
        ///</summary>
        ///<param name="password">Password en texto plano</param>
        ///<returns>Hash del password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si un password coincide con un hash
        /// </summary>
        /// <param name="password">Password en texto plano</param>
        /// <param name="hash">Hash almacenado</param>
        /// <returns>True si coinciden</returns>
        bool VerifyPassword(string password, string hash);
    }
}
