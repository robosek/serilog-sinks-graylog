using System.Threading.Tasks;
using FluentAssertions;
using Serilog.Debugging;
using Serilog.Sinks.Graylog.Core.Transport.Http;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Transport.Http
{
    public class HttpTransportClientFixture
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task WhenSendJson_ThenResultShouldNotThrows()
        {
            var target = new HttpTransportClient("http://logs.aeroclub.int:12201/gelf", certificatePath: null);

            await target.Send("{\"facility\":\"VolkovTestFacility\",\"full_message\":\"SomeComplexTestEntry TestClass { Id: 1, TestPropertyOne: \\\"1\\\", Bar: Bar { Id: 2, Prop: \\\"123\\\" }, TestPropertyTwo: \\\"2\\\", TestPropertyThree: \\\"3\\\" }\",\"host\":\"N68-MSK\",\"level\":6,\"short_message\":\"SomeComplexTestEntry TestClass { Id: 1, TestProper\",\"timestamp\":\"2017-03-24T11:18:54.1850651\",\"version\":\"1.1\",\"_stringLevel\":\"Information\",\"_test.Id\":1,\"_test.TestPropertyOne\":\"1\",\"_test.Bar.Id\":2,\"_test.Bar.Prop\":\"123\",\"_test.TestPropertyTwo\":\"2\",\"_test.TestPropertyThree\":\"3\"}");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task WhenSendJson_ThenResultShouldThrowException()
        {
            var target = new HttpTransportClient("http://logs.aeroclub.int:12201", certificatePath: null);

            LoggingFailedException exception = await Assert.ThrowsAsync<LoggingFailedException>(() => target.Send("{\"facility\":\"VolkovTestFacility\",\"full_message\":\"SomeComplexTestEntry TestClass { Id: 1, TestPropertyOne: \\\"1\\\", Bar: Bar { Id: 2, Prop: \\\"123\\\" }, TestPropertyTwo: \\\"2\\\", TestPropertyThree: \\\"3\\\" }\",\"host\":\"N68-MSK\",\"level\":6,\"short_message\":\"SomeComplexTestEntry TestClass { Id: 1, TestProper\",\"timestamp\":\"2017-03-24T11:18:54.1850651\",\"version\":\"1.1\",\"_stringLevel\":\"Information\",\"_test.Id\":1,\"_test.TestPropertyOne\":\"1\",\"_test.Bar.Id\":2,\"_test.Bar.Prop\":\"123\",\"_test.TestPropertyTwo\":\"2\",\"_test.TestPropertyThree\":\"3\"}"));

            exception.Message.Should().Be("Unable send log message to graylog via HTTP transport");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void WhenSendJsonWithIncorrectCertificatePath_ThenShouldThrowException()
        {
            var exception = Assert.Throws<LoggingFailedException>(() => new HttpTransportClient("https://logs.aeroclub.int:12201/gelf", "C:\\x.cert"));

            exception.Message.Should().Be("The system cannot find the file specified");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task WhenSendJsonWithCorrectCertificatePath_ThenShouldThrowException()
        {
            var target = new HttpTransportClient("https://logs.aeroclub.int:12201/gelf", "C:\\certificate.pfx");

            await target.Send("{\"facility\":\"VolkovTestFacility\",\"full_message\":\"SomeComplexTestEntry TestClass { Id: 1, TestPropertyOne: \\\"1\\\", Bar: Bar { Id: 2, Prop: \\\"123\\\" }, TestPropertyTwo: \\\"2\\\", TestPropertyThree: \\\"3\\\" }\",\"host\":\"N68-MSK\",\"level\":6,\"short_message\":\"SomeComplexTestEntry TestClass { Id: 1, TestProper\",\"timestamp\":\"2017-03-24T11:18:54.1850651\",\"version\":\"1.1\",\"_stringLevel\":\"Information\",\"_test.Id\":1,\"_test.TestPropertyOne\":\"1\",\"_test.Bar.Id\":2,\"_test.Bar.Prop\":\"123\",\"_test.TestPropertyTwo\":\"2\",\"_test.TestPropertyThree\":\"3\"}");
        }
    }
}