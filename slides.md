---
canvasWidth: 1280
aspectRatio: '16/9'
theme: default
colorSchema: dark
class: text-center
highlighter: prism
title: Not Your Parents' .NET
---

`Console.WriteLine("Hello, world!");`

---
layout: cover
class: text-center
---

# Not Your Parents' .NET

<img src="/steve.jpg" alt="Steve" class="profile" />
<img src="/logo.png" alt="ecoAPM logo" class="logo" />

## Steve Desmond

### June 5, 2023

#### Ithaca Web People

---

## Agenda

1. What is .NET?
1. A Brief History of Time
1. Live demo some features!

---
class: 'contact'
---

<img src="/steve.jpg" alt="Steve" />

## Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa-solid fa-earth-america"></i> [SteveDesmond.ca](https://SteveDesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa-brands fa-github"></i> [SteveDesmond-ca](https://github.com/SteveDesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa-brands fa-linkedin"></i> [Steve-Desmond](https://linkedin.com/in/Steve-Desmond) | [ecoAPM](https://linkedin.com/company/ecoAPM)

<i class="fa-brands fa-mastodon"></i> [@ecoSteve @ mastodon.social](https://mastodon.social/ecoSteve)
<span style="float:right;"> | [@ecoAPM @ fosstodon.org](https://fosstodon.org/@ecoAPM)</span>

---

<style>
    blockquote p {
        font-size: 200% !important;
    }
</style>

## What is .NET?

> free, open-source, cross-platform framework for building modern apps and powerful cloud services

- CLI
- Web
- Mobile
- Games

---

## What is .NET not?

<v-clicks>

- programming language
  - C#
  - F#
  - Visual Basic

- like JVM (Java, Kotlin, Scala, Closure, Groovy)

</v-clicks>

---

## What else is .NET not?

<v-clicks>

- proprietary [<i class="fa-solid fa-external-link"></i>](https://github.com/dotnet/runtime)
- Microsoft [<i class="fa-solid fa-external-link"></i>](https://dotnetfoundation.org)
- Windows only [<i class="fa-solid fa-external-link"></i>](https://dotnet.microsoft.com/download)
- Visual Studio [<i class="fa-solid fa-external-link"></i>](https://github.com/dotnet/sdk)
- enterprise [<i class="fa-solid fa-external-link"></i>](https://try.dot.net)
- slow [<i class="fa-solid fa-external-link"></i>](https://www.techempower.com/benchmarks/#section=data-r21&c=c&o=c)

</v-clicks>

---

## A Brief History of Time

```mermaid
flowchart LR
  subgraph Microsoft
	direction LR
    NET10(2002 \n .NET Framework 1.0)
    NET20(2005 \n .NET Framework 2.0)
    NET35(2007 \n .NET Framework 3.5)
    NET40(2010 \n .NET Framework 4.0)
    NET45(2012 \n .NET Framework 4.5)
    NET46(2015 \n .NET Framework 4.6)
    NET47(2017 \n .NET Framework 4.7)
    NET48(2019 \n .NET Framework 4.8)
    NET10 --> NET20
    NET20 --> NET35
    NET35 --> NET40
    NET40 --> NET45
    NET45 --> NET46
    NET46 --> NET47
    NET47 --> NET48
  end
```

---

## A Brief History of Time

```mermaid
flowchart LR
  subgraph Microsoft
	direction LR
    NET10(2002 \n .NET Framework 1.0)
    NET20(2005 \n .NET Framework 2.0)
    NET35(2007 \n .NET Framework 3.5)
    NET40(2010 \n .NET Framework 4.0)
    NET45(2012 \n .NET Framework 4.5)
    NET46(2015 \n .NET Framework 4.6)
    NET47(2017 \n .NET Framework 4.7)
    NET48(2019 \n .NET Framework 4.8)
    NET10 --> NET20
    NET20 --> NET35
    NET35 --> NET40
    NET40 --> NET45
    NET45 --> NET46
    NET46 --> NET47
    NET47 --> NET48
  end

  subgraph Ximian
	direction LR
    Mono1(2004 \n Mono 1.0)
    Mono2(2008 \n Mono 2.0)
    Mono3(2012 \n Mono 3.0)
    Mono4(2015 \n Mono 4.0)
    Mono5(2017 \n Mono 5.0)
    Mono6(2019 \n Mono 6.0)

    MT(2009 \n MonoTouch)
    Xamarin(2011 \n Xamarin)

    Mono1 --> Mono2
    Mono2 --> Mono3
    Mono3 --> Mono4
    Mono4 --> Mono5
    Mono5 --> Mono6
    MT --> Xamarin
  end

  subgraph .NET_Foundation
	direction LR
    Core10(2016 \n .NET Core 1.0)
    Core31(2019 \n .NET Core 3.1)
    NET5(2020 \n .NET 5)
    NET6(2021 \n .NET 6)
    NET7(2022 \n .NET 7)
    Core10 --> Core31
    Core31 --> NET5
    NET5 --> NET6
    NET6 --> NET7
  end
  
  Microsoft ~~~ .NET_Foundation
  Ximian ~~~ .NET_Foundation
  
  Mono3 --> Core10
  NET48 --> Mono6
  Core31 --> Mono6
  NET48 --> NET5
  Xamarin -->|MAUI| NET6
```

---

## What does it look like?

> Let's create some new projects

---
layout: cover
class: 'text-center'
---

# Thanks!

---
class: 'contact'
---

<img src="/steve.jpg" alt="Steve" />

# Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa-solid fa-earth-america"></i> [SteveDesmond.ca](https://SteveDesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa-brands fa-github"></i> [SteveDesmond-ca](https://github.com/SteveDesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa-brands fa-linkedin"></i> [Steve-Desmond](https://linkedin.com/in/Steve-Desmond) | [ecoAPM](https://linkedin.com/company/ecoAPM)

<i class="fa-brands fa-mastodon"></i> [@ecoSteve @ mastodon.social](https://mastodon.social/ecoSteve)
<span style="float:right;"> | [@ecoAPM @ fosstodon.org](https://fosstodon.org/@ecoAPM)</span>
