using System.Data;
using System.Text;
using Dapper;
using InternalApi.Models;

namespace InternalApi.Service
{
    public class UserService
    {
        private readonly IDbConnection _db;
        public UserService(IDbConnection db)
        {
            _db = db;
        }

        // R：查全部
        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            var sql = @"SELECT * 
                                    FROM Users 
                                    ORDER BY Id DESC";
            return await _db.QueryAsync<UserModel>(sql);
        }

        // R：查單筆
        public async Task<UserModel?> GetByIdAsync(int id)
        {
            var sql = @"SELECT * 
                                    FROM Users 
                                    WHERE Id = @Id";

            var parameters = new
            {
                Id = id
            };

            return await _db.QueryFirstOrDefaultAsync<UserModel>(sql, parameters);
        }

        //依據查詢資料
        public async Task<IEnumerable<UserModel>> GetBySearchAsync(SearchUserModel searchUserModel)
        {
            var sql = @"SELECT * 
            FROM Users 
            WHERE UserName LIKE '%'+  @UserName + '%'
            AND Email LIKE '%' + @Email + '%'";
            var parameters = new
            {
                UserName = searchUserModel.UserName,
                Email = searchUserModel.Email
            };
            return await _db.QueryAsync<UserModel>(sql.ToString(), parameters);
        }


        // C：新增（含交易）
        private static readonly object _idLock = new object(); //上鎖
        public async Task<bool> CreateAsync(UserModel userModel)
        {
            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }

            using var transaction = _db.BeginTransaction();
            try
            {
                string newId;
                lock (_idLock)//上鎖
                {
                    int taiwanYear = DateTime.Now.Year - 1911;
                    string prefix = taiwanYear.ToString();

                    string query = @"SELECT MAX(Id) FROM Users WHERE Id LIKE @Prefix + '%'";
                    string? maxId = _db.QueryFirstOrDefault<string>(query, new { Prefix = prefix }, transaction);

                    int nextSerial = 1;
                    if (!string.IsNullOrEmpty(maxId) && maxId.Length == 9)
                    {
                        nextSerial = int.Parse(maxId.Substring(3)) + 1;
                    }
                    newId = $"{prefix}{nextSerial.ToString("D6")}";
                }

                var parameters = new
                {
                    Id = newId,
                    userModel.UserName,
                    PassWord = BCrypt.Net.BCrypt.HashPassword(userModel.PassWord),
                    userModel.Email,
                    userModel.Img,
                    userModel.CreatedAt
                };

                var sql = @"INSERT INTO Users 
                    (Id, UserName, PassWord, Email, Img, CreatedAt) 
                    VALUES 
                    (@Id, @UserName, @PassWord, @Email, @Img, @CreatedAt);";

                var rows = await _db.ExecuteAsync(sql, parameters, transaction);
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        // U：更新（含交易）
        public async Task<bool> UpdateAsync(UserModel userModel)
        {
            var sql = @"UPDATE Users SET UserName = @UserName, Email = @Email WHERE Id = @Id";

            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
            using var transaction = _db.BeginTransaction();
            try
            {
                //var rows = await _db.ExecuteAsync(sql, user, transaction);
                var parameters = new
                {
                    userModel.Id,
                    userModel.UserName,
                    userModel.Email
                };
                var rows = await _db.ExecuteAsync(sql, parameters, transaction);
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // D：刪除（含交易）
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"DELETE FROM Users WHERE Id = @Id";

            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
            using var transaction = _db.BeginTransaction();
            try
            {
                var rows = await _db.ExecuteAsync(sql, new { Id = id }, transaction);
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}
