using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteApp.Server.Models;
using NoteApp.Server.Services;

namespace NoteApp.Tests
{
    [TestClass]
    public class RedisServiceTests
    {
        [TestMethod]
        public async Task Redis_SetObject_GetObject_Test()
        {
            Note note = new Note("My Title", "My Content", "Author", "/images/note1.png", "tag1,tag2");
            RedisService srv = new RedisService("localhost", "6666");

            srv.SetObjectValue<Note>("bschurkoNoteKey", note);

            Note cachedNote = srv.GetObjectData<Note>("bschurkoNoteKey");

            Assert.IsTrue(cachedNote != null);

        }
    }
}
