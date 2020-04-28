# Статические локальные функции

Теперь вы можете добавить модификатор `static` для локальных функций, чтобы убедиться, что локальная функция не захватывает (не ссылается на) какие-либо переменные из области видимости. Это приводит к возникновению ошибки `CS8421`: "A static local function can't contain a reference to \<variable>" (Статическая локальная функция не может содержать ссылку на \<переменная>).

Рассмотрим следующий код. Локальная функция `LocalFunction` обращается к переменной `y`, объявленной в области видимости (метод `M`). Таким образом `LocalFunction` не может объявляться с помощью модификатора `static`:

```cs --project ./ExploreCsharpEight/ExploreCsharpEight.csproj --source-file ./ExploreCsharpEight/StaticLocalFunctions.cs --region LocalFunction_Counting
```

The local iterator method *captures* the parameters `start` and `end`. Add the `static` modifier to see the compiler generated warning. You'll need to declare arguments to the local function so that those values aren't captured. Make the changes shown below to get the warning removed:

```csharp
static IEnumerable<int> localCounter(int first, int endLocation)
{
    for (int i = first; i < endLocation; i++)
        yield return i;
}
```

The sample should compile and run correctly.

#### Next: [disposable ref structs and using declarations &raquo;](using-declarations-ref-structs.md)    Previous: [Add peak pricing  &laquo;](./patterns-peakpricing.md)    Home: [Home](readme.md)
