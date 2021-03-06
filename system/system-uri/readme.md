---
name: "Класс System.Uri"
description: "Изучаем возможности класса System.Uri"
page_type: sample
languages:
  - csharp
products:
  - dotnet-core
---
# Изучаем возможности класса System.Uri

## Термины

**URI (Uniform Resource Identifiers)** - универсальный идентификатор ресурса. URI нужны, чтобы идентифицировать и запросить некий вид ресурса.

**URL (Uniform Resource Locator)** - унифицированный указатель ресурса, термин который часто используется вместо URI, так как URI - общий термин, используемый для ссылок на ресурсы. URL - это URI, связанный с такими популярными схемами URI, как http, ftp и mailto. В технической документации термин URL больше не употребляется.


**URN (Uniform Resource Name)** - унифицированное имя ресурса, стандартизированный URI, используемый для указания ресурса независимо от его расположения в сети.

Проанализируем части URI, на примере ссылки на веб-страницу сайта:

```text
http://www.dzmuh.com:80/portfolio/generic.asp?pageid=666&section=Details#Links
```

* Первая часть URI называется **схемой (scheme)** которая определяет пространство имен URI и может сузить синтаксис следующего за схемой выражения. Многие схемы названы по соответствующим протоколам (как *http*, *ftp*), которые они используют. Ограничитель схемы (`//` в этом примере) отделяет схему от остальной части URL.
* После ограничителя схемы следует имя сервера или IP-адрес в десятичной записи с точками, например `www.dzmuh.com`.
* За именем сервера или IP-адресом находится **номер порта**, определяющий соединение с конкретным приложением на сервере. Если номер порта не задан, используется номер порта, устанавливаемый для этого протокола по умолчанию.
* Путь определяет каталог/страницу запрошенного ресурса. В анализируемом случае путь имеет вид `/portfolio/generic.asp`.
* За символом `?` находится часть URI которая называется **запросом (query)**. В анализируемом случае запрос имеет вид `pageid=666&section=Details`. Запрос может состоять из нескольких компонентов, каждый из которых задает переменную и значение, объединенные символом `&`. В нашем примере первый компонент — `pageid=666` с переменной `pageid` и значением `666`, а второй компонент - `section=Details`.
* Разделы внутри ресурса можно отождествить с фрагментами. Фрагменты, например, используются для ссылок на разделы внутри HTML-страницы. Символ # отделяет идентификатор фрагмента от пути. В анализируемом случае фрагмент пути имеет вид `#Links`.

Если символ `#` добавлен в строку запроса, то это уже не фрагмент. В URL может присутствовать строка запроса или фрагмент, но не то и другое одновременно.

В URI зарезервировано использование нескольких символов — они не могут входить в имена хостов или путь, поскольку представляют собой специальные символы-разделители. В URI зарезервированы следующие символы:

```text
; / ? : @ & = + $ ,
```

## Класс System.Uri

Класс `System.Uri` инкапсулирует универсальный идентификатор ресурсов. Он содержит свойства и методы для анализа, сравнения и комбинирования URI. Создать объект можно передав конструктору строку URI: 

```cs --region UriIntro --source-file .\ExploreSystemUri\Program.cs --project .\ExploreSystemUri\ExploreSystemUri.csproj
```

## ===

```cs --region UrlParser --source-file .\ExploreSystemUri\Program.cs --project .\ExploreSystemUri\ExploreSystemUri.csproj
```