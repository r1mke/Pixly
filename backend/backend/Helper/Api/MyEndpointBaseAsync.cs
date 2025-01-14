using CloudinaryDotNet;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Heleper.Api
{
    public static class MyEndpointBaseAsync
    {
        public static class WithRequest<TRequest>
        {
            public abstract class WithResult<TResponse> : MyEndpointBase
            {
                public abstract Task<TResponse> HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithoutResult : MyEndpointBase
            {
                public abstract Task HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult<TResponse> : MyEndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }
            public abstract class WithActionResult : MyEndpointBase
            {
                public abstract Task<ActionResult> HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }
            //public abstract class WithAsyncEnumerableResult<T> : MyEndpointBase
            //{
            //    public abstract IAsyncEnumerable<T> HandleAsync(
            //      TRequest request,
            //      CancellationToken cancellationToken = default
            //    );
            //}
        }

        public static class WithoutRequest
        {
            public abstract class WithResult<TResponse> : MyEndpointBase
            {
                public abstract Task<TResponse> HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithoutResult : MyEndpointBase
            {
                public abstract Task HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult<TResponse> : MyEndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult : MyEndpointBase
            {
                public abstract Task<ActionResult> HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }

            //public abstract class WithAsyncEnumerableResult<T> : MyEndpointBase
            //{
            //    public abstract IAsyncEnumerable<T> HandleAsync(
            //      CancellationToken cancellationToken = default
            //    );
            //}
        }

        //ActionResult koristi se kada nije potrebno vraćati podatke već samo statusnu poruku
        //OkResult, NotFoundResult,BadRequestResult



        //ActionResult<T> predstavlja kombinaciju rezultata akcije i objekta određenog tipa T. return Ok(photo);

        //Statusni odgovori:

        //Odgovori uspjesnosti

        //Ok() - HTTP 200: Vraća se kada je zahtev uspešno obrađen i često se koristi s podacima u odgovoru.
        //Created() - HTTP 201: Vraća se kada je resurs uspešno kreiran.Najčešće se koristi pri kreiranju novih zapisa.
        //NoContent() - HTTP 204: Označava uspešan odgovor bez sadržaja.Koristi se kada nije potrebno vratiti podatke(npr.nakon uspešnog brisanja).

        //Preusmeravanje
        //Redirect() - HTTP 302: Preusmerava zahtev na drugu URL adresu.

        //Greske klijenta
        //BadRequest() - HTTP 400: Vraća se kada je zahtev neispravan, obično zbog nedostajućih ili nevalidnih podataka.
        //Unauthorized() - HTTP 401: Oznaka za zahtev bez autorizacije.Koristi se kada korisnik nije prijavljen ili nema pristup.
        //Forbidden() - HTTP 403: Označava da je korisnik prijavljen, ali nema dozvolu za određenu akciju.
        //NotFound() - HTTP 404: Vraća se kada traženi resurs nije pronađen.
        //        Conflict() - HTTP 409: Koristi se kada postoji konflikt u zahtevu, npr.duplikat resursa.
        //UnprocessableEntity() - HTTP 422: Vraća se kada je server primio zahtev, ali nije u stanju da ga obradi(obično zbog grešaka u validaciji).


        //Greske servera
        //StatusCode(500) - HTTP 500: Označava internu grešku servera.Obično se koristi kada postoji neočekivani problem.
        //StatusCode(502) - HTTP 502: Bad Gateway, server je dobio nevažeći odgovor od ulaznog servera.
        //StatusCode(503) - HTTP 503: Service Unavailable, server trenutno nije u mogućnosti da obradi zahtev.
        //
    }
}
