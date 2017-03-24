using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GameViewerApp.Entries;
using System.IO;

namespace GameViewerApp.Controllers
{
    [Route("api/[controller]")]
    public class GameDataController : Controller
    {
        private readonly GameData m_gameData;

        public GameDataController(GameData gameData)
        {
            m_gameData = gameData;
        }

        [HttpGet("{*path}")]
        public JsonResult Get(string path)
        {
            try
            {
                var entry = m_gameData.GetEntryAtPath(path ?? "");

                if (entry is IGameDataFileEntry fileEntry)
                {
                    return Json(new
                    {
                        Type = "File",
                        Name = fileEntry.Name,
                        Size = fileEntry.Size,
                        FullEntry = path
                    });
                }
                else if (entry is IGameDataDirectoryEntry dirEntry)
                {
                    var entries = dirEntry.GetEntries();

                    return Json(new
                    {
                        Type = "Directory",
                        Name = dirEntry.Name,
                        Directories = entries.Where(a => a is IGameDataDirectoryEntry).Select(a => a.Name).OrderBy(a => a),
                        Files = entries.Where(a => a is IGameDataFileEntry).Select(a => new
                        {
                            Name = a.Name,
                            Size = ((IGameDataFileEntry)a).Size
                        }).OrderBy(a => a.Name),
                        FullEntry = path
                    });
                }
            }
            catch (FileNotFoundException)
            {
                
            }
            
            return Json(null);
        }
    }
}
