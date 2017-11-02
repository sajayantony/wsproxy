using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

static class HttpExtensions
{
    static IEnumerable<KeyValuePair<string, StringValues>> GetForwardingHeaders(this IHeaderDictionary headers)
    {
         foreach(var v in headers)
         {
             switch(v.Key)
             {
                 case "Host":
                    break;
                default:
                    yield return new KeyValuePair<string, StringValues>(v.Key, v.Value);
                    break;

             }
         }
    }

    public static void CloneHeaders(this HttpRequest request, ClientWebSocketOptions options)
    {
        foreach(var h in GetForwardingHeaders(request.Headers))
        {
            options.SetRequestHeader(h.Key, h.Value);
        }
    }
}