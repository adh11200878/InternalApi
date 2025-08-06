using System.Data;
using Dapper;
using InternalApi.Helper;
using InternalApi.Models;

namespace InternalApi.Service
{
    public class AuthService
    {
        private readonly IDbConnection _db;
        private readonly JwtHelper _jwtHelper;
        public AuthService(IDbConnection db, JwtHelper jwtHelper)
        {
            _db = db;
            _jwtHelper = jwtHelper;
        }

        // 登入
        public async Task<string>Login(string userName,string password)
        {
            var sql = @"SELECT Id,UserName,PassWord 
                                    FROM Users 
                                    WHERE UserName = @UserName";

            var parameters = new
            {
                UserName = userName
            };
            var user = await _db.QueryFirstOrDefaultAsync<UserModel>(sql, parameters);

            if (user != null) {
                if(BCrypt.Net.BCrypt.Verify(password, user.PassWord))
                {
                    return _jwtHelper.GenerateJwtToken(user.Id);
                }   
            }
            return "";
        }

    }
}
