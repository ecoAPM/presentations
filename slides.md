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

---

## Agenda

1. Latency & Bottlenecks
1. Profiling & Profilers
1. Hands-On Part 1
1. Load Testing
1. Hands-On Part 2 
1. Q & A

---

## Disclaimers / Disclosures

- JetBrains relationship
- "real-world" examples

---

## Why should we care about *performance*?

- user experience
- accessibility
- hosting costs
- planned obsolescence

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

---

## Types of Bottlenecks

- CPU bound
  - algorithms
  - memory efficiency
- I/O bound
  - disk
  - network

---

## Profiling 101

### What?
> The act of measuring the CPU time and/or memory usage of running code

### Why?
> Easy access to detailed performance data about our application

### How?
> .NET CLR (COM) provides profiling API hooks, for third-party tools to aggregate

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
> Similar to sampling, but also captures event order

---

## Hands-On Part 1

1. app overview
1. profile a slow page
1. improve its performance
1. rinse & repeat

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

---

## `async` / `await`

- great for simplifying code and hiding complexity
- great for easily leaving perf on the table

---

## `Task` parallelization

```csharp
var results = new List<int>();

for (var x = 1; x <= 5; x++)
{
  var result = await GetResult(x);
  var results.Add(result);
}

return results;
```

vs

```csharp
var tasks = Enumerable.Range(1, 5).Select(GetResult);
var results = await Task.WhenAll(tasks);
return results;
```

.NET 6: `Parallel.ForEachAsync()`

---

## Asynchronous "pre-fetch"

```csharp
return new Result
{
  X = await GetX(),
  Y = await GetY()
};
```

vs

```csharp
var x = GetX();
var y = GetY();

return new Result
{
  X = await x,
  Y = await y
};
```

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Profile Your Unit Tests

---

---
class: 'sxs'
---

## Eliminate Repeat Actions

```csharp
var x = GetX();
var y = GetY();

return new Result
{
  X = await x,
  Y = await y
};
```

←

```csharp
async Task<int> GetX()
{
  var value = await GetValue();
  return value.X;

async Task<int> GetY()
{
  var value = await GetValue();
  return value.Y;
}
```

### vs

```csharp
var value = await GetValue();

return new Result
{
  X = value.X,
  Y = value.Y
};
```

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

---

## Caching

> One of the two hardest problems in computer science, along with naming things and off-by-one errors

### Types (in-memory)
- static
- time-based expiration

---
class: 'sxs'
---

## Static In-Memory Caching

### For when a value never changes during runtime

```csharp
public class Slow
{
  public async Task<ulong> GetDouble()
    => await LongAction() * 2;
}
```

vs

```csharp
public class NotAsSlow
{
  private static int _value;

  public async Task<ulong> GetDouble()
    => _value ??= await LongAction() * 2;
}
```

---

## Caching with Expiration

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
        entry => {
          entry.AbsoluteExpiration = TimeSpan.FromMinutes(1);
          return await SlowMethod() * 2;
        });
}
```

---
layout: cover
class: 'text-center'
---

# Interlude

---

## Load Testing

### What?
> The act of putting demand on a software system and measuring its response 

### Why?
> To observe how the system reacts to certain volumes of requests

### How?
> `for` loops, `wrk`, [aspnet/Benchmarks](https://github.com/aspnet/Benchmarks), [LoadTestToolbox](https://github.com/ecoAPM/LoadTestToolbox)

---

## Hands-On Part 2

1. run a load test
1. analyze the output
1. profile a load test
1. improve app performance
1. rinse & repeat

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

## Use the Best Tool for the Job

- .NET Core 3.1 => .NET 6
- `WebRequest` => `HttpClient`
- `NSubstitute` => `Rocks`

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
