using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace DockerGui.Test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void Test1()
        {
            _output.WriteLine("123");
        }
    }
}
