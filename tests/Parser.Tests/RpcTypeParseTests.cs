using FluentAssertions;

using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class RpcTypeParseTests
{
    [Fact]
    public void RpcTypeFile_CanParse()
    {
        const string fileContent = 
            """
            ###############################
            # RPC_CheckEnrolmentResponse TYPE
            ###############################
            rpc_type RPC_CheckEnrolmentResponse : description("Check if Government enrolment exists response")
            {
                MessageID :
                    string(max=50),
                    description("Unique Identifier")
            
                EnrolmentStatusResponse :
                    isRPCType(name=RPC_CheckEnrolmentResponseBody),
                    description("The NULLABLE response object for an enrolment response if it exists")
            }
            """;
        var rpcTypeFile = (TypeBuilder)EntityParser.ParseRpcType(fileContent);
        rpcTypeFile.Name.Value.Value.Should().Be("RPCCheckEnrolmentResponse");
        rpcTypeFile.Summary.Value.Should().Be("Check if Government enrolment exists response");
        rpcTypeFile.Fields.Should().HaveCount(2);
    }
}