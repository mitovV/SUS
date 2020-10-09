namespace SUS.MvcFramework.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using ViewEngine;

    using Xunit;

    public class SusViewEngineTests
    {
        [Theory]
        [InlineData("CleanHtml")]
        [InlineData("Foreach")]
        [InlineData("IfElseFor")]
        [InlineData("ViewModel")]
        public void TestGetHtml(string fileName)
        {

            var viewModel = new TestViewModel
            {
                DateOfBirth = new DateTime(2019, 6, 1),
                Name = "Doggo Arghentino",
                Price = 12345.67M,
            };

            IViewEngine viewEngine = new SusViewEngine();
            var view = File.ReadAllText($"ViewTests/{fileName}.html");
            var result = viewEngine.GetHtml(view, viewModel);
            var expectedResult = File.ReadAllText($"ViewTests/{fileName}Result.html");
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TestTemplateViewModel()
        {
            var viewEngine = new SusViewEngine();
          var actual =  viewEngine.GetHtml(@"@foreach(var num in Model)
{
<span>@num</span>
}", new List<int> { 1, 2, 3 });

            var expected = @"<span>1</span>
<span>2</span>
<span>3</span>
";

            Assert.Equal(expected,actual);
        }
    }
}
