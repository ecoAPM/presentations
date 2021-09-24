---
canvasWidth: 1280
theme: default
colorSchema: dark
class: text-center
highlighter: prism
title: Profiling and Fixing Common Performance Bottlenecks
---

# Profiling and Fixing Common Performance Bottlenecks

<img src="/steve.jpg" alt="Steve" class="profile" />
<img src="/logo.png" alt="ecoAPM logo" class="logo" />

## Steve Desmond

### October 7, 2021

<!--
Setup
- reset to initial commit (keep git log open)
- delete DB (keep ADS open)
-->

---
class: 'contact'
---

<img src="/steve.jpg" alt="Steve" />

# Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa fa-globe"></i> [SteveDesmond.ca](https://SteveDesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa fa-github"></i> [SteveDesmond-ca](https://github.com/SteveDesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa fa-twitter"></i> [SteveDesmond_ca](https://twitter.com/SteveDesmond_ca) | [ecoAPM](https://twitter.com/ecoAPM)

<i class="fa fa-envelope-o"></i> [Steve @ ecoAPM .com](mailto:Steve@ecoAPM.com)

<!--
- a little about me
- building software professionally for almost 2 decades
- now leading an open source software company focusing on performance
-->

---

## Agenda

1. Latency & Bottlenecks
1. Profiling & Profilers
1. Hands-On Part 1
1. Load Testing
1. Hands-On Part 2 
1. Q & A

<!--
- "So what are we going to talk about today?"
- go through list
- can also ask questions throughout and I'll answer as we go
- also note while examples are all C#, a lot of this holds true anywhere
-->

---

## Why should we care about *performance*?

- user experience
- accessibility
- hosting costs
- planned obsolescence

<!--
- "But first" ask question, give 4 reasons
- UX: studies introducting artificial latency reduced sales, removing artificial latency increased sales
- accessibility: cognitive differences, but also economic
- bottom line: less scaling, hardware requirements
- planned obsolescence: big problem, planet has finite resources
- better performing software allows devices to be used longer
-->

---

## Computer Latency

| CPU instruction	| 0.5 ns |
|-|-:|
| L1 cache | 1 ns |
| L2 cache | 4 ns |
| L3 cache | 15 ns |
| RAM | 30 ns |
| SSD | 30 μs |
| LAN (1 Gbps) | 1 ms |
| Net (Azure BB) | 6 ms |

Sources
: Intel ([1](https://software.intel.com/content/www/us/en/develop/articles/memory-performance-in-a-nutshell.html))
; AnandTech ([1](https://www.anandtech.com/show/8423/intel-xeon-e5-version-3-up-to-18-haswell-ep-cores-/11), [2](https://www.anandtech.com/bench/SSD21/2994))
; Microsoft ([1](https://docs.microsoft.com/en-us/azure/networking/azure-network-latency))

<!--
- need to get into some numbers to understand the sizes of certain things
- go through list, point out jumps in magnitude
- explain LAN setup
- note global internet ping times (up to 500ms)
-->

---

## On a Human Time Scale

| CPU instruction	| 1 sec |
|-|-:|
| L1 cache | 2 sec |
| L2 cache | 8 sec |
| L3 cache | 30 sec |
| RAM | 1 min | 
| SSD | 9 hours |
| LAN (1 Gbps) | 12 days |
| Net (Azure BB) | 10 weeks |

<!--
- "Since humans have a hard time with very small and very large scales"
- note everything multiplied by 2 billion
- go through list
- easier to see jumps between latter items
-->

---

## Types of Bottlenecks

- CPU bound
  - algorithms
  - memory efficiency
- I/O bound
  - disk
  - network

<!--
- "And so, based on this divide, we have 2 types of bottlenecks:"
- CPU about algorithms and memory, especially in .NET and other managed runtimes
- I/O about efficient offloading of async tasks, so CPU can keep working while it waits for data
- example of container ship across ocean, what to do in meantime 
- different solutions for each, which we'll go through
-->

---

## Profiling 101

### What?
> The act of measuring the CPU time and/or memory usage of running code

### Why?
> Easy access to detailed performance data about our application

### How?
> .NET CLR (COM) provides profiling API hooks, for third-party tools to aggregate

<!--
- "How do we know what's slow?"
- lots of ways, profilers are one tool in our toolbox
- after how: "dotTrace is my .NET profiler of choice"
-->

---
class: 'small'
---

## Profiling Methods

### Sampling
> Least instrusive, least detail; can connect to an already-running application

### Tracing
> Slightly slower, more information

### Line-by-Line
> Most overhead, most detail

### Timeline
> Similar to sampling, but also captures event order and therefore can best show async/await details

<!--
- dotTrace has 4 methods (go through list)
- usually start w/timeline to get overall picture, then dive into LxL
-->

---

## Hands-On Part 1

1. app overview
1. profile a slow page
1. improve its performance
1. rinse & repeat

<!--
- go over list
- demo app (main page & phone page)
- give MVP & Wegmans examples for realism
- open dotTrace, explain main menu
- check TPL box, start timeline profile, refresh demo app phone page
- load profile, explain window, introduce parallelization
(Firefox: slides)
-->

---

## Serial vs Parallel

```yml
- 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32
```
vs
```yml
- 01 02 03 04 05 06 07 08
- 09 10 11 12 13 14 15 16
- 17 18 19 20 21 22 23 24
- 25 26 27 28 29 30 31 32
```
(or)
```yml
- 01 05 09 13 17 21 25 29
- 02 06 10 14 18 22 26 30
- 03 07 11 15 19 23 27 31
- 04 08 12 16 20 24 28 32
```

<!--
- here's example w/32 tasks
- we could do them all in order, vs break them up into 4 groups
- could split them more
-->

---
class: 'sxs'
---

## async / await

- C#
- F#
- Haskell
- Python
- TS
- JS
- Rust
- C++
- Swift
- Perl

Great for simplifying code and hiding complexity

Great for easily leaving perf on the table

<!--
- very familiar w/async/await in .NET, but the keywords exist in many languages now
- while great for #1, also leads to #2
- especially true when "async-ifying" 
-->

---

## `Task` parallelization

```csharp {all|3-7|5}
var results = new List<int>();

for (var x = 1; x <= 5; x++)
{
  var result = await GetResult(x);
  var results.Add(result);
}

return results;
```

vs

```csharp {all|1|1-2}
var tasks = Enumerable.Range(1, 5).Select(GetResult);
var results = await Task.WhenAll(tasks);
return results;
```

.NET 6: `Parallel.ForEachAsync()`

<!--
- simplified example of what we found
- explain comparison
(Rider: HomeController)
- Extract GetPriceViewModel(), parallelize usage
-->

---

## Asynchronous "pre-fetch"

```csharp {all|3-4}
return new Result
{
  X = await GetX(),
  Y = await GetY()
};
```

vs

```csharp {all|1-2|1-2,6-7}
var x = GetX();
var y = GetY();

return new Result
{
  X = await x,
  Y = await y
};
```

<!--
(Rider: HomeController)
- extract logo and price task variables
- re-run, show improvements
- re-profile, compare timelines
-->

---
class: 'sxs'
---

## Eliminate Repeat Actions

```csharp {all|1-2,6-7}
var x = GetX();
var y = GetY();

return new Result
{
  X = await x,
  Y = await y
};
```

←

```csharp {all|3,9}
async Task<int> GetX()
{
  var value = await GetValue();
  return value.X;
}

async Task<int> GetY()
{
  var value = await GetValue();
  return value.Y;
}
```

### vs

```csharp {all|1,5-6}
var value = await GetValue();

return new Result
{
  X = value.X,
  Y = value.Y
};
```

<!--
- "But what happens when tasks use the same underlying data?"
- getting this information twice, let's fix that
(Rider: HomeController)
- move GetPriceViewModel to PriceDisplayService
- explain static + sync improvements as we go
-->

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Use The Best Tools For The Job

<!--
- ready for a LxL profile
(Rider)
- start LxL profile, profile info page
(dotTrace)
- explain new layout, LxL tab, total time vs own
- note heavy AngleSharp
(Rider: Startup)
- add HttpClient, Factory in PriceDisplayService
-->

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Profile Your Unit Tests

<!--
(Rider)
- profile unit tests (LxL), open in dotTrace
- sort by calls to show DB inefficiency
(Rider: PhoneInfoService)
- fix queries
(Firefox: slides)
-->

---

## Database Query Optimization

```csharp
var results = await db.QueryAsync<Result>("SELECT * FROM Results");
return results.FirstOrDefault(r => r.ID == 123);
```

vs

```csharp
return await db.QuerySingleAsync<Result>("SELECT * FROM Results WHERE ID = @ID", new { ID = 123 });
```

<hr/>

```csharp
var results = await db.QueryAsync<Result>("SELECT * FROM Results");
return results.Select(r => r.Name).Distinct();
```

vs

```csharp
return await db.QueryAsync<string>("SELECT DISTINCT Name FROM Results");
```

<!--
- may seem obvious to some, but actually quite common
- especially in .NET w/LINQ & EF, lines start to blur where things run
- "SQL is built for this, let it do its thing"
-->

---

## Caching

```csharp
public class Slow
{
  public async Task<ulong> GetDouble()
    => await SlowMethod() * 2;
}
```

vs

```csharp
public class NotAsSlow
{
  private IMemoryCache _cache;

  public async Task<ulong> GetDouble()
    => _cache.GetOrCreate("ValueDoubled",
        async entry => {
          entry.AbsoluteExpiration = TimeSpan.FromMinutes(1);
          return await SlowMethod() * 2;
        });
}
```

<!--
- "Which brings us to caching, which as we all know, is one of the 3 hardest problems in computer science, along with naming things, off-by-one errors, and not using this joke"
- why save till later (still fast on cache miss)
- explain types (time-based vs static)
(Rider: PriceDisplayService)
- extract HTTP GET to GetResponse()
- add AddMemoryCache() to Startup
- add IMemoryCache to PriceDisplayService
- wrap GetResponse() in _cache.GetOrCreateAsync()
-->

---

## Load Testing

### What?
> The act of putting demand on a software system and measuring its response 

### Why?
> To observe how the system reacts to certain volumes of requests

### How?
> `for` loops, `wrk`, [`aspnet/Benchmarks`](https://github.com/aspnet/Benchmarks), [`LoadTestToolbox`](https://github.com/ecoAPM/LoadTestToolbox)

<!--
- "now that we're caching our HTTP responses..."
-->

---

## Hands-On Part 2

1. run a load test
1. analyze the output
1. profile a load test
1. improve app performance
1. rinse & repeat

---

![async all the things!](/async.jpg)

<style>
  img {
    width: 100%;
    aspect-ratio: 1.64;
    object-fit: contain;
  }
</style>

---
class: 'sxs'
---

## Variable scope in loops

```csharp
var results = new List<int>();

for (var x = 1; x <= 5; x++)
{
  var y = GetY();
  var result = GetResult(x, y);
}

return results;
```

vs

```csharp
var results = new List<int>();
var y = GetY();

for (var x = 1; x <= 5; x++)
{
  var result = GetResult(x, y);
}

return results;
```

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Stay Up-To-Date

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

<i class="fa fa-globe"></i> [SteveDesmond.ca](https://stevedesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa fa-github"></i> [SteveDesmond-ca](https://github.com/stevedesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa fa-twitter"></i> [SteveDesmond_ca](https://twitter.com/stevedesmond_ca) | [ecoAPM](https://twitter.com/ecoAPM)

<i class="fa fa-envelope-o"></i> [Steve @ ecoAPM .com](mailto:Steve@ecoAPM.com)
