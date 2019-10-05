using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using System.GraphQL;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.GraphQL.Api.Controllers
{
    [Route("graphql")]
    public class GraphQLController : Controller
    {
        private readonly ISchema _schema;
        private readonly IDocumentExecuter _documentExecuter;

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter)
        {
            _documentExecuter = documentExecuter;
            _schema = schema;
        }

        public async Task<IActionResult> Post([FromBody] GraphQLQuery query, CancellationToken cancellationToken = default)
        {
            var inputs = query.Variables.ToInputs();

            var executeOptions = new ExecutionOptions
            {
                Inputs = inputs,
                Schema = _schema,
                Query = query.Query,
                OperationName = query.OperationName,
                CancellationToken = cancellationToken
            };

            var result = await _documentExecuter.ExecuteAsync(executeOptions).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
                return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}