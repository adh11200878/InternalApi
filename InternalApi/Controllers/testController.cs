using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using TESTAPI.Models;

namespace TESTAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class testController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public testController(IDbConnection connection)
        {
            _connection = connection;
        }


        [HttpGet]
        public async Task<IEnumerable<MenuItem>> Get()
        {
            // SQL 查詢語句: 從 MenuItems 資料表中選取所有欄位，並依照 ParentId 與 SortOrder 排序
            string sql = @"SELECT * FROM MenuItems ORDER BY ParentId, SortOrder";
            // 執行 SQL 查詢並將結果轉換成 List<MenuItem>
            var items = (await _connection.QueryAsync<MenuItem>(sql)).ToList();
            // 建立 Dictionary，key 為 MenuItem 的 Id，value 為 MenuItem 本身
            // 方便後續快速查找父項目
            var lookup = items.ToDictionary(x => x.id);
            // 建立樹狀結構：將每個項目加入其父項目的 Children 清單中
            foreach (var item in items)
            {
                if (item.parentId.HasValue && lookup.TryGetValue(item.parentId.Value, out var parent))
                {
                    parent.children.Add(item);
                }
            }
            // 回傳所有根節點（即 ParentId 為 null 的項目）
            return items.Where(x => !x.parentId.HasValue).ToList();
        }

        [HttpGet]
        public async Task<string> AAA()
        {
            return "1234";
        }



    }
}

