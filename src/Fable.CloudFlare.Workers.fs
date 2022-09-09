module Fable.CloudFlare.Workers

open Browser.Types
open Fable.Core

[<Global>]
type ExecutionContext =
    member _.waitUntil(promise: JS.Promise<'a>) : unit = jsNative
    member _.passThroughOnException() : unit = jsNative

[<Global>]
type KVNamespace =
    [<Emit "$0.get($1, 'json')">]
    member _.getJson<'a>(key: string) : JS.Promise<'a option> = jsNative

    member _.put<'a>(key: string, value: 'a) : JS.Promise<unit> = jsNative

[<Global "Headers">]
type Headers(headers: (string * string) seq) =
    new() = Headers([])

    member _.get(name: string) : string option = jsNative
    member _.getAll(name: string) : string seq = jsNative
    member _.has(name: string) : bool = jsNative
    member _.set(name: string, value: string) : unit = jsNative
    member _.append(name: string, value: string) : unit = jsNative
    member _.delete(name: string) : unit = jsNative
    member _.values() : string seq = jsNative
    member _.entries() : (string * string) seq = jsNative
    member _.keys() : string seq = jsNative

module Request =
    [<AllowNullLiteral>]
    type Options() =
        [<DefaultValue>]
        val mutable method: string

        [<DefaultValue>]
        val mutable url: string

        [<DefaultValue>]
        val mutable headers: Headers

[<Global "Request">]
[<AllowNullLiteral>]
type Request(request: Request, options: Request.Options) =
    new(request: Request) = Request(request, null)

    new() = Request(null, null)

    member _.method: string = jsNative
    member _.url: string = jsNative
    member _.headers: Headers = jsNative

    member _.json<'a>() : JS.Promise<'a> = jsNative
    member _.text() : JS.Promise<string> = jsNative

module Response =
    [<AllowNullLiteral>]
    type Options() =
        [<DefaultValue>]
        val mutable status: int

        [<DefaultValue>]
        val mutable statusText: string

        [<DefaultValue>]
        val mutable headers: Headers

[<Global "Response">]
type Response =
    [<Emit "new Response()">]
    static member Create() : Response = jsNative

    [<Emit "new Response($0)">]
    static member Create(body: string) : Response = jsNative

    [<Emit "new Response($0)">]
    static member Create(body: Blob) : Response = jsNative

    [<Emit "new Response($0)">]
    static member Create(body: JS.ArrayBuffer) : Response = jsNative

    [<Emit "new Response($0, $1)">]
    static member Create(body: string, options: Response.Options) : Response = jsNative

    [<Emit "new Response($0, $1)">]
    static member Create(body: Blob, options: Response.Options) : Response = jsNative

    [<Emit "new Response($0, $1)">]
    static member Create(body: JS.ArrayBuffer, options: Response.Options) : Response = jsNative


    static member redirect(url: string) : Response = jsNative
    static member redirect(url: string, status: int) : Response = jsNative

type IFetchable<'environment> =
    abstract fetch: Request * 'environment * ExecutionContext -> JS.Promise<Response>
