using NoteApp.Server.Data;
using NoteApp.Server.Services;

namespace NoteApp.Tests
{
    [TestClass]
    public sealed class NoteServiceTests
    {

        [TestMethod]
        public async Task Run_Integration_Test_On_NoteService_TEST()
        {
            var context = new AppDbContextFactory().CreateDbContext(null);
            NoteService service = new NoteService(new AppDbContextFactory());
            var notes = await service.GetAllAsync();
        }
    }
}
