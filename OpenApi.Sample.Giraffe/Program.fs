module Program

open System.Net
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Giraffe.EndpointRouting
open Giraffe.Serialization
open OpenApi
open OpenApi.Expressions
open AspFeat.Builder

type Product = { Id: int32; Name: string }

let jsonOptions = AspFeat.JsonSerializer.createOptions ()
let v1Factory = OpenApiFactory.create jsonOptions "Products API" "v1"

let getProductsSpec endpoint =
    apiOperation {
        tags [ apiTag { name "Products" } ]
        summary "Get the list of products."
        responses [
            HttpStatusCode.OK, apiResponse {
                description "Success"
                jsonContent (v1Factory.MakeJsonContent [ { Id = 0; Name = "name" } ])
            } ]
        }
    |> GiraffeOpenApi.addOperation v1Factory endpoint

let getProducts : HttpHandler =
    fun _ ctx ->
        [ { Id = 1; Name = "Cat Food" } ]
        |> ctx.WriteJsonAsync

let endpoints =
    [
        GET => route "/products" getProducts |> getProductsSpec
        GET => route v1Factory.SpecificationUrl (v1Factory.Write text)
    ]

let addGiraffe (services: IServiceCollection) =
    services
        .AddGiraffe()
        .AddSingleton<IJsonSerializer>(SystemTextJsonSerializer jsonOptions)

let useSwaggerUi (app: IApplicationBuilder) =
    app.UseSwaggerUI(fun options ->
        options.SwaggerEndpoint(v1Factory.SpecificationUrl, v1Factory.Version))

[<EntryPoint>]
let main _ =
    [ Endpoints.featWith (fun b -> b.MapGiraffeEndpoints endpoints)
      (addGiraffe, useSwaggerUi) ]
    |> WebHost.run
