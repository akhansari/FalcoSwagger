﻿namespace OpenApi.Builders

open Microsoft.OpenApi.Models

type DiscriminatorBuilder () =

    member _.Yield _ =
        OpenApiDiscriminator ()

    /// REQUIRED. The name of the property in the payload that will hold the discriminator value.
    [<CustomOperation "propertyName">]
    member _.PropertyName (state: OpenApiDiscriminator, value) =
        state.PropertyName <- value
        state

    /// An object to hold mappings between payload values and schema names or references.
    [<CustomOperation "addMapping">]
    member _.Mapping (state: OpenApiDiscriminator, key, value) =
        state.Mapping.Add (key, value)
        state