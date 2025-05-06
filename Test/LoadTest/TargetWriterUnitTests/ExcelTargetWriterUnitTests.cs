using ETL.Domain.NewFolder;
using ETL.Domain.Targets;

namespace Test.LoadTest.TargetWriterUnitTests
{
    public class ExcelTargetWriterUnitTests : IDisposable
    {
        private readonly string _testDirectory;

        public ExcelTargetWriterUnitTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"ExcelTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }


        [Fact]
        public async Task WriteAsync_WhenTargetInfoIsInvalid_ThenThrowsArgumentException()
        {
            var context = new LoadContext
            {
                TargetInfo = new FakeTargetInfo(),
                Data = new Dictionary<string, object> { ["A"] = 1 }
            };

            var writer = new ExcelTargetWriter();

            await Assert.ThrowsAsync<ArgumentException>(() => writer.WriteAsync(context));
        }


        public void Dispose()
        {
            try { Directory.Delete(_testDirectory, true); } catch { }
        }

        private class FakeTargetInfo : TargetInfoBase { }
    }
}
