
using System.Linq.Expressions;

namespace MovieAPI.Model;

/// <summary>
/// En hjelpeklasse som mapper potensielle Query Parametere fra URL, og kan lage en lamda funksjon basert på disse.
/// Denne vil automatisk generere spørringer mot parametere basert på hva parametere den får inn. 
/// </summary>
/// <typeparam name="T">Dette er en generisk type, men i dette tilfellet vil representere enten en Movies eller TVShows class.</typeparam>
public class QueryBody<T>
{
    public string? Title { get; set; }
    public string? Director { get; set; }
    public int? From { get; set; }
    public int? To { get; set; }
    public Expression<Func<T, bool>> ToPredicate()
    {
        //Her definerer vi vår hovedparameter, det som vi skal sammenligne querien vår med.
        //I en Where(t => t.Id == val) er denne verdien vår referanse til den abstrakte verdien t. 
        var parameter = Expression.Parameter(typeof(T), "t");
        //Her lager vi en nullable expression som skal representere vår predicate, predicaten er det som vil være vår "statement", det som tilslutt skal returnere en bool.
        //I Where(t => t.Id == val) er det t.Id == val som er vår "predicate".
        Expression? predicate = null;
        if (!string.IsNullOrEmpty(Title))
        {
            //Her sier vi at vi er interesert å bruke propertien "Title" på vårt parameter.
            //Så langt kan man se for seg at vår predicate ut slik: Where(t=>t.Title)
            var titleProp = Expression.Property(parameter, "Title");
            //Vi sier vi skal calle en funksjon som heter string.ToLower på titleProp for å normalisere stringdata.
            //Så langt kan man se for seg at vår predicate ut slik: Where(t=>t.Title.ToLower())
            var titleToLower = Expression.Call(titleProp, nameof(string.ToLower), null);
            //Her definerer vi en referanse til verdien vi vil sammenligne med vår titleprop for å generere en boolean. I dette tilfellet er det
            //en refereanse til QueryBody sin Title prop ToLower.
            var titleVal = Expression.Constant(Title.ToLower());
            //Her definerer vi hvilken operasjon som skal gjøres for å returnere en bool.
            //Siden vi skal skjekke string mot substring, bruker vi Call() for å definere selv hva method som skal kjøres, og på hvilken verdi.
            //Så langt kan man se for seg at vår predicate ser slik ut: Where(t=>t.Title.ToLower.Contains(QueryBody.Title))
            var contains = Expression.Call(titleToLower, nameof(String.Contains), null, titleVal);
            //Her passer vi på at vi enten appender, eller starter, vår predicate. 
            //Hvis vi antar alle properties er spurt etter, kan vi se for oss at vår predicate hittil ser slik ut:
            //Where(t=>t.Title.ToLower.Contains(QueryBody.Title))
            predicate = predicate == null ? contains : Expression.AndAlso(predicate, contains);
        }
        if (!string.IsNullOrEmpty(Director))
        {
            //Her gjennomfører vi mange de samme opperasjonene som ovenfor, bare rettet mot Director feltet.
            var dirProp = Expression.Property(parameter, "Director");
            var dirVal = Expression.Constant(Director.ToLower());
            var dirToLower = Expression.Call(dirProp, nameof(string.ToLower), null);
            var contains = Expression.Call(dirToLower, nameof(String.Contains), null, dirVal);
            //Igjen i bunn passer vi på å appende hvis en predicate allerede blir bygget.
            //Her kommer Expression.AndAlso methoden inn. AndAlso representerer && i en LinQ spørring.
            //Hvis vi ser for oss at denne spørringen skal bli appendet til spørringen over, vil vår 
            //predicate se slik ut etter denne blokken er kjørt: Where(t=>t.Title.ToLower.Contains(QueryBody.Title) && t.Director.ToLower.Contains(QueryBody.Director))
            predicate = predicate == null ? contains : Expression.AndAlso(predicate, contains);
        }
        if (From.HasValue)
        {
            //Vår Query har evnen til å ta imot en From for å representere en "Fra og med" verdi.
            //Her definerer vi hva vi skal sammenligne med, og hva vi skal bruke til sammenligning.
            var releaseProp = Expression.Property(parameter, "ReleaseYear");
            var fromVal = Expression.Constant(From.Value);
            //I steden for å bruke en datatype spesific method, kan vi her bruke en innebygget Expression method
            //for å representere hvordan vi skal sammenligne, bruker vi GreaterThanOrEqual. 
            //dvs vi sier vi vil sammenligne releaseProp >= fromVal.
            var greaterThan = Expression.GreaterThanOrEqual(releaseProp, fromVal);
            //Her også må vi passe på å potensielt appende.
            //Hvis alle parameterene har kjørt hittil, kan vi ser for oss at vår predicate ser slik ut her:
            //Where(t=>t.Title.ToLower.Contains(QueryBody.Title) && t.Director.ToLower.Contains(QueryBody.Director) && t.ReleaseYear >= QueryBody.From)
            predicate = predicate == null ? greaterThan : Expression.AndAlso(predicate, greaterThan);
        }
        if (To.HasValue)
        {
            //Her gjør vi det samme som over, bare med en potensiell maksimalverdi for ReleaseYear
            var releaseProp = Expression.Property(parameter, "ReleaseYear");
            var toVal = Expression.Constant(To.Value);
            //Her bruker vi LessThanOrEqual methoden for å injecte en mindre eller lik sammenligning mellom våre to verdier releaseProp <= toVal.
            var lessThan = Expression.LessThanOrEqual(releaseProp, toVal);
            //Igjen passer vi på å potensielt appende. Hvis alle parameterene har kjørt hittil, ser vår predicate slik ut:
            //Where(t=>t.Title.ToLower.Contains(QueryBody.Title) && t.Director.ToLower.Contains(QueryBody.Director) && t.ReleaseYear >= QueryBody.From && t.ReleaseYear <= QueryBody.To)
            predicate = predicate == null ? lessThan : Expression.AndAlso(predicate, lessThan);
        }
        //I vår returnstatement skjekker vi om vår predicate fremdeles er null.
        //Hvis predicate er null returnerer vi funksjonen t => true. Det vil si at vi vil at alle verdier av T er gyldig. 
        //Ellers returnerer vi den ferdigbygde Lamdafunksjonen basert på vår predicate og vårt parameter. 
        //Hvis alle parameterene er brukt, vil vår ferdige parameter se slik ut:
        //t=>t.Title.ToLower.Contains(QueryBody.Title) && t.Director.ToLower.Contains(QueryBody.Director) && t.ReleaseYear >= QueryBody.From && t.ReleaseYear <= QueryBody.To
        return predicate == null ? t => true : Expression.Lambda<Func<T, bool>>(predicate, parameter);
    }
}
