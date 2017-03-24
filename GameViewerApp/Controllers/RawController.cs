using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GameViewerApp.Entries;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace GameViewerApp.Controllers
{
    [Route("raw")]
    public class RawController : Controller
    {
        private readonly GameData m_gameData;

        public RawController(GameData gameData)
        {
            m_gameData = gameData;
        }

        [HttpGet("{*path}")]
        public async Task Get(string path)
        {
            try
            {
                var entry = m_gameData.GetEntryAtPath(path);

                if (entry is IGameDataFileEntry fileEntry)
                {
                    await fileEntry.ExportAsync(HttpContext.Response.Body);
                    return;
                }
            }
            catch (FileNotFoundException)
            {

            }

            HttpContext.Response.StatusCode = 404;
            await HttpContext.Response.WriteAsync("Not found");
        }
    }
}
