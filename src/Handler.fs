module Handler

open Browser.Url
open Fable.Core.JsInterop

open Fable.CloudFlare.Workers

type Environment = { HIT_COUNTER: KVNamespace }

let getIndex (req: Request) (env: Environment) =
    promise {
        let url = URL.Create(req.url)

        return!
            match url.searchParams.get "tag" with
            | Some tag ->
                promise {
                    let! count =
                        tag
                        |> env.HIT_COUNTER.getJson<int>
                        |> Promise.map (Option.defaultValue 0)

                    let count = count + 1
                    do! env.HIT_COUNTER.put (tag, count)

                    return
                        Response.Create(
                            Pixel.buffer,
                            Response.Options(
                                status = 200,
                                headers =
                                    Headers(
                                        [ ("Content-Type", "image/png")
                                          ("Cache-Control", "no-cache") ]
                                    )
                            )
                        )
                }
            | None -> promise { return Response.Create("Missing tag", Response.Options(status = 200)) }
    }

let notFound () =
    promise { return Response.Create("Not found", Response.Options(status = 404)) }

exportDefault
    { new IFetchable<Environment> with
        member _.fetch(req, env, _) =
            promise {
                let url = URL.Create(req.url)

                return!
                    match req.method, url.pathname with
                    | "GET", "/" -> getIndex req env
                    | _ -> notFound ()
            } }
